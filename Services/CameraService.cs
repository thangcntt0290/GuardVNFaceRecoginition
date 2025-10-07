using System.Windows.Media.Imaging;
using System.Windows;
using GuardVNFaceRecoginition.Models;
using OpenCvSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace GuardVNFaceRecoginition.Services
{
    /// <summary>
    /// Helper class to store bitmap data for camera service
    /// </summary>
    public class CameraBitmapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] Pixels { get; set; } = Array.Empty<byte>();
    }

    /// <summary>
    /// Real camera service implementation using OpenCV for webcam and RTSP streaming
    /// </summary>
    public class CameraService : ICameraService, IDisposable
    {
        #region Private Fields
        private VideoCapture? capture;
        private Mat frame;
        private bool isRunning = false;
        private Task? captureTask;
        private CancellationTokenSource? cancellationTokenSource;
        private readonly object lockObject = new();
        private Camera? currentCamera;
        #endregion

        #region Properties
        public bool IsRunning 
        { 
            get 
            { 
                lock (lockObject) 
                { 
                    return isRunning; 
                } 
            } 
        }
        #endregion

        #region Events
        public event Action<WriteableBitmap>? FrameReady;
        public event Action<string>? StatusChanged;
        #endregion

        #region Constructor
        public CameraService()
        {
            frame = new Mat();
        }
        #endregion

        #region Public Methods
        public async Task<bool> StartAsync(Camera camera)
        {
            if (IsRunning)
            {
                Stop();
                await Task.Delay(500); // Give time for cleanup
            }

            try
            {
                currentCamera = camera;
                OnStatusChanged($"Connecting to {camera.Name}...");
                
                // Create VideoCapture based on camera type
                if (camera.SourceType.Equals("Webcam", StringComparison.OrdinalIgnoreCase))
                {
                    OnStatusChanged($"Opening webcam device {camera.DeviceIndex}...");
                    
                    // Try DirectShow first (better for Windows webcams)
                    capture = new VideoCapture(camera.DeviceIndex, VideoCaptureAPIs.DSHOW);
                    
                    // If DirectShow fails, try default API
                    if (!capture.IsOpened())
                    {
                        capture?.Dispose();
                        capture = new VideoCapture(camera.DeviceIndex);
                        OnStatusChanged("DirectShow failed, trying default API...");
                    }
                }
                else if (camera.SourceType.Equals("RTSP", StringComparison.OrdinalIgnoreCase))
                {
                    OnStatusChanged($"Connecting to RTSP stream: {camera.Source}");
                    capture = new VideoCapture(camera.Source);
                }
                else
                {
                    OnStatusChanged("Error: Unsupported camera type");
                    return false;
                }

                // Check if camera opened successfully
                if (capture == null || !capture.IsOpened())
                {
                    OnStatusChanged($"Failed to open {camera.SourceType}: {camera.Name}");
                    return false;
                }

                // Configure camera properties for optimal performance
                await ConfigureCameraAsync(camera);
                
                // Test reading a frame to verify camera works
                Mat testFrame = new Mat();
                bool canRead = false;
                
                // Try multiple times for RTSP streams (may take time to connect)
                for (int i = 0; i < 5; i++)
                {
                    canRead = capture.Read(testFrame);
                    if (canRead && !testFrame.Empty())
                    {
                        OnStatusChanged($"Camera active at {testFrame.Width}x{testFrame.Height}");
                        break;
                    }
                    
                    if (camera.SourceType.Equals("RTSP", StringComparison.OrdinalIgnoreCase))
                    {
                        OnStatusChanged($"Waiting for RTSP connection... ({i + 1}/5)");
                        await Task.Delay(1000);
                    }
                }
                
                testFrame.Dispose();
                
                if (!canRead)
                {
                    OnStatusChanged($"Cannot read frames from {camera.Name}");
                    capture?.Release();
                    capture?.Dispose();
                    capture = null;
                    return false;
                }

                // Start camera capture loop
                lock (lockObject)
                {
                    isRunning = true;
                }

                cancellationTokenSource = new CancellationTokenSource();
                captureTask = Task.Run(() => CaptureLoop(cancellationTokenSource.Token));

                OnStatusChanged($"? Connected to {camera.Name}");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Error starting camera: {ex.Message}");
                
                // Cleanup on error
                capture?.Release();
                capture?.Dispose();
                capture = null;
                
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                bool wasRunning = false;
                lock (lockObject)
                {
                    wasRunning = isRunning;
                    if (!isRunning) 
                    {
                        OnStatusChanged("? Camera is already stopped");
                        return;
                    }
                    isRunning = false;
                }

                if (!wasRunning) return;

                OnStatusChanged("? Stopping camera...");

                // Cancel capture task first
                cancellationTokenSource?.Cancel();
                
                // Wait for task to complete (with timeout)
                try
                {
                    if (captureTask != null && !captureTask.IsCompleted)
                    {
                        bool completed = captureTask.Wait(TimeSpan.FromSeconds(3));
                        if (!completed)
                        {
                            OnStatusChanged("? Capture task timeout, forcing stop...");
                        }
                    }
                }
                catch (AggregateException ex)
                {
                    // Expected for cancellation
                    System.Diagnostics.Debug.WriteLine($"Capture task cancelled: {ex.InnerException?.Message}");
                }
                
                // Cleanup OpenCV resources
                try
                {
                    capture?.Release();
                    capture?.Dispose();
                    capture = null;
                    System.Diagnostics.Debug.WriteLine("OpenCV resources released");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error releasing OpenCV: {ex.Message}");
                }
                
                // Cleanup other resources
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                captureTask = null;

                OnStatusChanged("? Camera stopped");
                System.Diagnostics.Debug.WriteLine("Camera service stopped successfully");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Error stopping camera: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stop error: {ex}");
                
                // Force cleanup even if there's an error
                try
                {
                    lock (lockObject)
                    {
                        isRunning = false;
                    }
                    
                    capture?.Release();
                    capture?.Dispose();
                    capture = null;
                    
                    cancellationTokenSource?.Dispose();
                    cancellationTokenSource = null;
                    captureTask = null;
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        public void CaptureFrame()
        {
            if (!IsRunning || capture == null)
            {
                OnStatusChanged("? Camera is not running");
                return;
            }

            try
            {
                // Capture frame in background to avoid blocking UI
                Task.Run(() =>
                {
                    try
                    {
                        Mat capturedFrame = new Mat();
                        bool success = capture.Read(capturedFrame);
                        
                        if (success && !capturedFrame.Empty())
                        {
                            // Save frame to desktop with timestamp
                            string fileName = $"GuardVN_Capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            string filePath = Path.Combine(desktopPath, fileName);
                            
                            // Save with high quality
                            var saveParams = new int[] { (int)ImwriteFlags.JpegQuality, 95 };
                            Cv2.ImWrite(filePath, capturedFrame, saveParams);
                            
                            OnStatusChanged($"?? Image saved: {fileName} ({capturedFrame.Width}x{capturedFrame.Height})");
                        }
                        else
                        {
                            OnStatusChanged("? Failed to capture frame");
                        }
                        
                        capturedFrame.Dispose();
                    }
                    catch (Exception ex)
                    {
                        OnStatusChanged($"? Capture error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Error capturing frame: {ex.Message}");
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Configure camera properties for optimal performance
        /// </summary>
        private async Task ConfigureCameraAsync(Camera camera)
        {
            try
            {
                if (camera.SourceType.Equals("Webcam", StringComparison.OrdinalIgnoreCase))
                {
                    OnStatusChanged("?? Configuring webcam settings...");
                    
                    // Set maximum resolution
                    SetMaximumResolution();
                    
                    // Set optimal frame rate
                    capture?.Set(VideoCaptureProperties.Fps, 30);
                    
                    // Reduce buffer size for lower latency
                    try
                    {
                        capture?.Set(VideoCaptureProperties.BufferSize, 1);
                    }
                    catch
                    {
                        // Some cameras don't support buffer size setting
                    }
                    
                    // Auto exposure and white balance
                    try
                    {
                        capture?.Set(VideoCaptureProperties.AutoExposure, 0.25);
                        // AutoWb is not available in this OpenCV version, skip it
                        // capture?.Set(VideoCaptureProperties.AutoWb, 1);
                    }
                    catch
                    {
                        // Not all cameras support these properties
                    }
                }
                else if (camera.SourceType.Equals("RTSP", StringComparison.OrdinalIgnoreCase))
                {
                    OnStatusChanged("?? Configuring RTSP stream...");
                    
                    // RTSP specific settings for better streaming
                    try
                    {
                        capture?.Set(VideoCaptureProperties.BufferSize, 3);
                        capture?.Set(VideoCaptureProperties.FourCC, VideoWriter.FourCC('H', '2', '6', '4'));
                    }
                    catch
                    {
                        // RTSP settings may not be supported by all streams
                    }
                }
                
                // Small delay to let camera stabilize
                await Task.Delay(100);
                
                OnStatusChanged("? Camera configured successfully");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Warning: Camera configuration failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Set camera to maximum supported resolution
        /// </summary>
        private void SetMaximumResolution()
        {
            if (capture == null) return;
            
            try
            {
                // Get current resolution
                double currentWidth = capture.Get(VideoCaptureProperties.FrameWidth);
                double currentHeight = capture.Get(VideoCaptureProperties.FrameHeight);
                
                // Common high resolutions to try (in priority order)
                var resolutions = new[]
                {
                    (3840, 2160), // 4K UHD
                    (2560, 1440), // 1440p QHD
                    (1920, 1080), // 1080p Full HD
                    (1600, 1200), // UXGA
                    (1280, 720),  // 720p HD
                    (1024, 768),  // XGA
                    (800, 600),   // SVGA
                    (640, 480)    // VGA (fallback)
                };
                
                foreach (var (width, height) in resolutions)
                {
                    // Try to set this resolution
                    capture.Set(VideoCaptureProperties.FrameWidth, width);
                    capture.Set(VideoCaptureProperties.FrameHeight, height);
                    
                    // Small delay for camera to adjust
                    Thread.Sleep(50);
                    
                    // Check what resolution was actually set
                    double actualWidth = capture.Get(VideoCaptureProperties.FrameWidth);
                    double actualHeight = capture.Get(VideoCaptureProperties.FrameHeight);
                    
                    // If we got close to the target resolution, we're done
                    if (Math.Abs(actualWidth - width) <= 1 && Math.Abs(actualHeight - height) <= 1)
                    {
                        OnStatusChanged($"?? Resolution set to {(int)actualWidth}x{(int)actualHeight}");
                        return;
                    }
                }
                
                // If no specific resolution worked, report what we have
                double finalWidth = capture.Get(VideoCaptureProperties.FrameWidth);
                double finalHeight = capture.Get(VideoCaptureProperties.FrameHeight);
                OnStatusChanged($"?? Using resolution: {(int)finalWidth}x{(int)finalHeight}");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Resolution setting failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Main capture loop running in background thread
        /// </summary>
        private async Task CaptureLoop(CancellationToken cancellationToken)
        {
            OnStatusChanged("?? Camera streaming started");
            
            int frameCount = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                while (!cancellationToken.IsCancellationRequested && IsRunning && capture != null)
                {
                    if (capture.Read(frame) && !frame.Empty())
                    {
                        frameCount++;
                        
                        // Convert frame to bitmap data on background thread
                        var bitmapData = MatToBitmapData(frame);
                        if (bitmapData != null)
                        {
                            // Create WriteableBitmap on UI thread
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                try
                                {
                                    var bitmap = CreateWriteableBitmapFromData(bitmapData);
                                    if (bitmap != null)
                                    {
                                        FrameReady?.Invoke(bitmap);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"UI thread bitmap error: {ex.Message}");
                                }
                            });
                        }
                        
                        // Report FPS every 5 seconds
                        if (stopwatch.ElapsedMilliseconds >= 5000)
                        {
                            double fps = frameCount / stopwatch.Elapsed.TotalSeconds;
                            OnStatusChanged($"?? Streaming at {fps:F1} FPS ({currentCamera?.Name})");
                            frameCount = 0;
                            stopwatch.Restart();
                        }
                    }
                    else
                    {
                        OnStatusChanged("? Failed to read frame, retrying...");
                        await Task.Delay(100, cancellationToken);
                        continue;
                    }

                    // Control frame rate (target ~30 FPS)
                    await Task.Delay(33, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                OnStatusChanged("?? Capture cancelled");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Capture loop error: {ex.Message}");
            }
        }

        /// <summary>
        /// Convert OpenCV Mat to bitmap data
        /// </summary>
        private CameraBitmapData? MatToBitmapData(Mat mat)
        {
            try
            {
                if (mat.Empty()) return null;
                
                int width = mat.Width;
                int height = mat.Height;
                
                // Convert BGR to RGB for WPF display
                Mat rgbMat = new Mat();
                if (mat.Channels() == 3)
                {
                    Cv2.CvtColor(mat, rgbMat, ColorConversionCodes.BGR2RGB);
                }
                else if (mat.Channels() == 1)
                {
                    Cv2.CvtColor(mat, rgbMat, ColorConversionCodes.GRAY2RGB);
                }
                else
                {
                    mat.CopyTo(rgbMat);
                }
                
                // Extract pixel data
                byte[] pixels = new byte[rgbMat.Total() * rgbMat.ElemSize()];
                Marshal.Copy(rgbMat.Data, pixels, 0, pixels.Length);
                
                rgbMat.Dispose();
                
                return new CameraBitmapData 
                { 
                    Width = width, 
                    Height = height, 
                    Pixels = pixels 
                };
            }
            catch (Exception ex)
            {
                OnStatusChanged($"? Frame conversion error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create WPF WriteableBitmap from camera data (must run on UI thread)
        /// </summary>
        private WriteableBitmap? CreateWriteableBitmapFromData(CameraBitmapData bitmapData)
        {
            try
            {
                WriteableBitmap bitmap = new WriteableBitmap(
                    bitmapData.Width, 
                    bitmapData.Height, 
                    96, 96, 
                    System.Windows.Media.PixelFormats.Rgb24, 
                    null);
                
                // Write pixels to bitmap
                Int32Rect rect = new Int32Rect(0, 0, bitmapData.Width, bitmapData.Height);
                bitmap.WritePixels(rect, bitmapData.Pixels, bitmapData.Width * 3, 0);
                
                return bitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WriteableBitmap creation error: {ex.Message}");
                return null;
            }
        }

        private void OnFrameReady(WriteableBitmap bitmap)
        {
            try
            {
                FrameReady?.Invoke(bitmap);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FrameReady event error: {ex.Message}");
            }
        }

        private void OnStatusChanged(string status)
        {
            try
            {
                StatusChanged?.Invoke(status);
                System.Diagnostics.Debug.WriteLine($"Camera Status: {status}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"StatusChanged event error: {ex.Message}");
            }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Stop();
            frame?.Dispose();
        }
        #endregion
    }
}