using OpenCvSharp;
using GuardVNFaceRecoginition.Models;

namespace GuardVNFaceRecoginition.Services
{
    /// <summary>
    /// Helper to detect available cameras
    /// </summary>
    public static class CameraDetector
    {
        /// <summary>
        /// Detect available webcam devices
        /// </summary>
        public static List<Camera> DetectWebcams()
        {
            var cameras = new List<Camera>();
            
            // Try to detect webcams (typically 0-3 are common)
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    using var capture = new VideoCapture(i);
                    if (capture.IsOpened())
                    {
                        // Try to read a frame to confirm the camera works
                        using var testFrame = new Mat();
                        if (capture.Read(testFrame) && !testFrame.Empty())
                        {
                            cameras.Add(new Camera
                            {
                                Name = $"Webcam {i}",
                                SourceType = "Webcam",
                                Source = i.ToString(),
                                DeviceIndex = i
                            });
                        }
                    }
                }
                catch
                {
                    // Camera not available, continue to next index
                }
            }
            
            return cameras;
        }

        /// <summary>
        /// Test if a specific camera is available
        /// </summary>
        public static bool IsCameraAvailable(Camera camera)
        {
            try
            {
                if (camera.SourceType.Equals("Webcam", StringComparison.OrdinalIgnoreCase))
                {
                    using var capture = new VideoCapture(camera.DeviceIndex);
                    return capture.IsOpened();
                }
                else if (camera.SourceType.Equals("RTSP", StringComparison.OrdinalIgnoreCase))
                {
                    using var capture = new VideoCapture(camera.Source);
                    return capture.IsOpened();
                }
            }
            catch
            {
                // Camera not available
            }
            
            return false;
        }

        /// <summary>
        /// Get default cameras including detected webcams
        /// </summary>
        public static List<Camera> GetDefaultCameras()
        {
            var cameras = new List<Camera>();
            
            // Add detected webcams
            cameras.AddRange(DetectWebcams());
            
            // Add default RTSP camera if no webcams found
            if (cameras.Count == 0)
            {
                cameras.Add(new Camera 
                { 
                    Name = "Default Webcam", 
                    SourceType = "Webcam", 
                    Source = "0", 
                    DeviceIndex = 0 
                });
            }
            
            // Add sample RTSP camera
            cameras.Add(new Camera 
            { 
                Name = "RTSP Camera", 
                SourceType = "RTSP", 
                Source = "rtsp://admin:Admin123!@10.235.3.229:554/stream1" 
            });
            
            return cameras;
        }
    }
}