using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using GuardVNFaceRecoginition.Commands;
using GuardVNFaceRecoginition.Models;
using GuardVNFaceRecoginition.Services;

namespace GuardVNFaceRecoginition.ViewModels
{
    /// <summary>
    /// ViewModel for the MainWindow
    /// </summary>
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region Private Fields
        private readonly ICameraService _cameraService;
        private readonly IMessageService _messageService;
        private bool _isCameraRunning = false;
        private Camera? _selectedCamera;
        private NavigationPage _currentPage = NavigationPage.LiveCamera;
        private WriteableBitmap? _cameraFrame;
        private string _cameraStatusText = "Initialized";
        private Color _cameraStatusColor = Colors.Gray;
        private Visibility _cameraPlaceholderVisibility = Visibility.Visible;
        private Visibility _mainContentVisibility = Visibility.Visible;
        private Visibility _contentAreaVisibility = Visibility.Collapsed;
        private bool _isLiveCameraSelected = true;
        private bool _isImageSearchSelected = false;
        private bool _isGallerySelected = false;
        private bool _isStatisticsSelected = false;
        #endregion

        #region Properties
        public ObservableCollection<Camera> Cameras { get; }

        public Camera? SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                if (SetProperty(ref _selectedCamera, value))
                {
                    OnCameraSelectionChanged();
                }
            }
        }

        public bool IsCameraRunning
        {
            get => _isCameraRunning;
            private set
            {
                if (SetProperty(ref _isCameraRunning, value))
                {
                    // Update command can execute states
                    (StartCameraCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                    (StopCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (CaptureCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public NavigationPage CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    UpdateNavigationProperties();
                }
            }
        }

        public WriteableBitmap? CameraFrame
        {
            get => _cameraFrame;
            private set => SetProperty(ref _cameraFrame, value);
        }

        public string CameraStatusText
        {
            get => _cameraStatusText;
            private set => SetProperty(ref _cameraStatusText, value);
        }

        public Color CameraStatusColor
        {
            get => _cameraStatusColor;
            private set => SetProperty(ref _cameraStatusColor, value);
        }

        public Visibility CameraPlaceholderVisibility
        {
            get => _cameraPlaceholderVisibility;
            private set => SetProperty(ref _cameraPlaceholderVisibility, value);
        }

        public Visibility MainContentVisibility
        {
            get => _mainContentVisibility;
            private set => SetProperty(ref _mainContentVisibility, value);
        }

        public Visibility ContentAreaVisibility
        {
            get => _contentAreaVisibility;
            private set => SetProperty(ref _contentAreaVisibility, value);
        }

        // Navigation button selection states
        public bool IsLiveCameraSelected
        {
            get => _isLiveCameraSelected;
            private set => SetProperty(ref _isLiveCameraSelected, value);
        }

        public bool IsImageSearchSelected
        {
            get => _isImageSearchSelected;
            private set => SetProperty(ref _isImageSearchSelected, value);
        }

        public bool IsGallerySelected
        {
            get => _isGallerySelected;
            private set => SetProperty(ref _isGallerySelected, value);
        }

        public bool IsStatisticsSelected
        {
            get => _isStatisticsSelected;
            private set => SetProperty(ref _isStatisticsSelected, value);
        }
        #endregion

        #region Commands
        public ICommand NavigateToLiveCameraCommand { get; }
        public ICommand NavigateToImageSearchCommand { get; }
        public ICommand NavigateToGalleryCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand StartCameraCommand { get; }
        public ICommand StopCameraCommand { get; }
        public ICommand CaptureCameraCommand { get; }
        public ICommand AddCameraCommand { get; }
        public ICommand EditCameraCommand { get; }
        public ICommand RemoveCameraCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel(ICameraService cameraService, IMessageService messageService)
        {
            _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

            // Initialize cameras collection with detected cameras
            var detectedCameras = CameraDetector.GetDefaultCameras();
            Cameras = new ObservableCollection<Camera>(detectedCameras);
            
            System.Diagnostics.Debug.WriteLine($"Detected {Cameras.Count} cameras");
            foreach (var camera in Cameras)
            {
                System.Diagnostics.Debug.WriteLine($"- {camera.Name} ({camera.SourceType})");
            }

            // Set initial selected camera
            if (Cameras.Count > 0)
            {
                SelectedCamera = Cameras[0];
            }

            // Initialize commands
            NavigateToLiveCameraCommand = new RelayCommand(() => NavigateToPage(NavigationPage.LiveCamera));
            NavigateToImageSearchCommand = new RelayCommand(() => NavigateToPage(NavigationPage.ImageSearch));
            NavigateToGalleryCommand = new RelayCommand(() => NavigateToPage(NavigationPage.Gallery));
            NavigateToStatisticsCommand = new RelayCommand(() => NavigateToPage(NavigationPage.Statistics));

            StartCameraCommand = new AsyncRelayCommand(StartCameraAsync, CanStartCamera);
            StopCameraCommand = new RelayCommand(StopCamera, () => IsCameraRunning);
            CaptureCameraCommand = new RelayCommand(CaptureFrame, () => IsCameraRunning);
            AddCameraCommand = new RelayCommand(AddCamera);
            EditCameraCommand = new RelayCommand(EditCamera, () => SelectedCamera != null);
            RemoveCameraCommand = new RelayCommand(RemoveCamera, () => SelectedCamera != null);

            // Subscribe to camera service events
            _cameraService.FrameReady += OnFrameReady;
            _cameraService.StatusChanged += OnCameraStatusChanged;

            // Set initial status
            UpdateCameraStatus(CameraStatus.Default($"Detected {Cameras.Count} cameras"));
        }
        #endregion

        #region Navigation Methods
        private void NavigateToPage(NavigationPage page)
        {
            CurrentPage = page;

            switch (page)
            {
                case NavigationPage.LiveCamera:
                    ShowLiveCameraView();
                    break;
                case NavigationPage.ImageSearch:
                case NavigationPage.Gallery:
                case NavigationPage.Statistics:
                    ShowContentPage();
                    break;
            }
        }

        private void ShowLiveCameraView()
        {
            ContentAreaVisibility = Visibility.Collapsed;
            MainContentVisibility = Visibility.Visible;
        }

        private void ShowContentPage()
        {
            MainContentVisibility = Visibility.Collapsed;
            ContentAreaVisibility = Visibility.Visible;
        }

        private void UpdateNavigationProperties()
        {
            IsLiveCameraSelected = CurrentPage == NavigationPage.LiveCamera;
            IsImageSearchSelected = CurrentPage == NavigationPage.ImageSearch;
            IsGallerySelected = CurrentPage == NavigationPage.Gallery;
            IsStatisticsSelected = CurrentPage == NavigationPage.Statistics;
        }
        #endregion

        #region Camera Control Methods
        private bool CanStartCamera() => SelectedCamera != null && !IsCameraRunning;

        private async Task StartCameraAsync()
        {
            if (SelectedCamera == null) return;

            try
            {
                // Reset UI state first
                CameraFrame = null;
                CameraPlaceholderVisibility = Visibility.Visible;
                
                UpdateCameraStatus(CameraStatus.Info($"Starting {SelectedCamera.Name}..."));
                System.Diagnostics.Debug.WriteLine($"Starting camera: {SelectedCamera.Name}");
                
                bool success = await _cameraService.StartAsync(SelectedCamera);
                
                if (success)
                {
                    // Verify the camera is actually running
                    await Task.Delay(100); // Small delay to let service stabilize
                    
                    if (_cameraService.IsRunning)
                    {
                        IsCameraRunning = true;
                        UpdateCameraStatus(CameraStatus.Success($"? Camera running: {SelectedCamera.Name}"));
                        System.Diagnostics.Debug.WriteLine($"Camera started successfully: {SelectedCamera.Name}");
                    }
                    else
                    {
                        IsCameraRunning = false;
                        UpdateCameraStatus(CameraStatus.Error($"? Camera failed to start: {SelectedCamera.Name}"));
                        System.Diagnostics.Debug.WriteLine($"Camera service reports not running after start");
                    }
                }
                else
                {
                    IsCameraRunning = false;
                    UpdateCameraStatus(CameraStatus.Error($"? Failed to start {SelectedCamera.Name}"));
                    System.Diagnostics.Debug.WriteLine($"Camera start returned false: {SelectedCamera.Name}");
                    
                    // Show error message on UI thread
                    if (Application.Current?.Dispatcher.CheckAccess() == true)
                    {
                        _messageService.ShowError($"Failed to start camera: {SelectedCamera.Name}");
                    }
                    else
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                            _messageService.ShowError($"Failed to start camera: {SelectedCamera.Name}"));
                    }
                }
            }
            catch (Exception ex)
            {
                IsCameraRunning = false;
                UpdateCameraStatus(CameraStatus.Error($"? Error: {ex.Message}"));
                
                System.Diagnostics.Debug.WriteLine($"StartCameraAsync Exception: {ex}");
                
                // Show error message on UI thread
                try
                {
                    if (Application.Current?.Dispatcher.CheckAccess() == true)
                    {
                        _messageService.ShowError($"Error starting camera: {ex.Message}");
                    }
                    else
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                            _messageService.ShowError($"Error starting camera: {ex.Message}"));
                    }
                }
                catch (Exception dispatcherEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Dispatcher error: {dispatcherEx.Message}");
                }
            }
        }

        private void StopCamera()
        {
            if (!IsCameraRunning) return;

            try
            {
                UpdateCameraStatus(CameraStatus.Info("Stopping camera..."));
                System.Diagnostics.Debug.WriteLine("Stopping camera...");
                
                _cameraService.Stop();
                
                // Force update the running state immediately
                IsCameraRunning = false;
                
                // Reset display
                CameraFrame = null;
                CameraPlaceholderVisibility = Visibility.Visible;
                
                UpdateCameraStatus(CameraStatus.Default("Camera stopped"));
                
                // Force command state update
                (StartCameraCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (StopCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CaptureCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                
                System.Diagnostics.Debug.WriteLine("Camera stopped successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping camera: {ex.Message}");
                
                // Force reset state even if there's an error
                IsCameraRunning = false;
                CameraFrame = null;
                CameraPlaceholderVisibility = Visibility.Visible;
                
                // Force command state update
                (StartCameraCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (StopCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CaptureCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                
                UpdateCameraStatus(CameraStatus.Error($"Error stopping camera: {ex.Message}"));
                _messageService.ShowError($"Error stopping camera: {ex.Message}");
            }
        }

        private void CaptureFrame()
        {
            if (!IsCameraRunning) return;

            try
            {
                _cameraService.CaptureFrame();
                _messageService.ShowMessage($"Frame captured from {SelectedCamera?.Name}!");
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error capturing frame: {ex.Message}");
            }
        }
        #endregion

        #region Camera Management Methods
        private void AddCamera()
        {
            try
            {
                var addCameraWindow = new AddCameraWindow();
                if (Application.Current.MainWindow != null)
                {
                    addCameraWindow.Owner = Application.Current.MainWindow;
                }
                
                if (addCameraWindow.ShowDialog() == true || addCameraWindow.IsConfirmed)
                {
                    if (addCameraWindow.Camera != null)
                    {
                        if (IsCameraNameExists(addCameraWindow.Camera.Name))
                        {
                            _messageService.ShowWarning("A camera with this name already exists. Please choose a different name.");
                            return;
                        }

                        // Stop current camera if running
                        if (IsCameraRunning)
                        {
                            StopCamera();
                        }

                        // Add new camera to collection
                        var newCamera = addCameraWindow.Camera;
                        System.Diagnostics.Debug.WriteLine($"Adding camera: {newCamera.Name}, Type: {newCamera.SourceType}, Source: {newCamera.Source}");
                        
                        Cameras.Add(newCamera);
                        
                        // Set as selected camera
                        SelectedCamera = newCamera;
                        
                        System.Diagnostics.Debug.WriteLine($"Total cameras after add: {Cameras.Count}");
                        
                        _messageService.ShowMessage($"Camera '{newCamera.Name}' added successfully!\nTotal cameras: {Cameras.Count}");
                        
                        // Update camera display with new selection
                        UpdateCameraStatus(CameraStatus.Info($"Camera selected: {SelectedCamera.Name}"));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Camera object is null!");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Dialog was cancelled or not confirmed");
                }
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error adding camera: {ex.Message}");
            }
        }

        private void EditCamera()
        {
            if (SelectedCamera == null) return;
            _messageService.ShowMessage($"Edit functionality for '{SelectedCamera.Name}' - Coming soon!");
        }

        private void RemoveCamera()
        {
            if (SelectedCamera == null) return;

            bool confirmed = _messageService.ShowConfirmation($"Are you sure you want to remove camera '{SelectedCamera.Name}'?");
            
            if (confirmed)
            {
                if (IsCameraRunning) StopCamera();

                var cameraToRemove = SelectedCamera;
                Cameras.Remove(cameraToRemove);
                
                if (Cameras.Count > 0)
                {
                    SelectedCamera = Cameras[0];
                }
                else
                {
                    SelectedCamera = null;
                }
                
                _messageService.ShowMessage("Camera removed successfully!");
            }
        }

        private void OnCameraSelectionChanged()
        {
            if (SelectedCamera == null) return;

            if (IsCameraRunning)
            {
                _messageService.ShowMessage($"Switching to camera: {SelectedCamera.Name}\nCurrent camera will be stopped.");
                StopCamera();
            }
            
            UpdateCameraStatus(CameraStatus.Info($"Selected: {SelectedCamera.Name}"));
        }

        private bool IsCameraNameExists(string name)
        {
            return Cameras.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region Event Handlers
        private void OnFrameReady(WriteableBitmap bitmap)
        {
            try
            {
                if (bitmap != null)
                {
                    // Ensure we're on UI thread
                    if (Application.Current?.Dispatcher.CheckAccess() == true)
                    {
                        CameraFrame = bitmap;
                        CameraPlaceholderVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Application.Current?.Dispatcher.BeginInvoke(() =>
                        {
                            CameraFrame = bitmap;
                            CameraPlaceholderVisibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnFrameReady: {ex.Message}");
                UpdateCameraStatus(CameraStatus.Error($"Frame display error: {ex.Message}"));
            }
        }

        private void OnCameraStatusChanged(string status)
        {
            try
            {
                // Ensure we're on UI thread for status updates
                if (Application.Current?.Dispatcher.CheckAccess() == true)
                {
                    ProcessStatusChange(status);
                }
                else
                {
                    Application.Current?.Dispatcher.BeginInvoke(() => ProcessStatusChange(status));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnCameraStatusChanged: {ex.Message}");
            }
        }

        private void ProcessStatusChange(string status)
        {
            System.Diagnostics.Debug.WriteLine($"Camera Status: {status}");
            
            // Update running state based on status messages
            if (status.Contains("? Connected to") || status.Contains("?? Streaming") || status.Contains("Camera active"))
            {
                if (!IsCameraRunning)
                {
                    IsCameraRunning = true;
                }
                UpdateCameraStatus(CameraStatus.Success(status));
            }
            else if (status.Contains("? Camera stopped") || status.Contains("?? Capture cancelled"))
            {
                if (IsCameraRunning)
                {
                    IsCameraRunning = false;
                    CameraFrame = null;
                    CameraPlaceholderVisibility = Visibility.Visible;
                }
                UpdateCameraStatus(CameraStatus.Default(status));
            }
            else if (status.Contains("?") || status.StartsWith("Failed") || status.StartsWith("Error"))
            {
                UpdateCameraStatus(CameraStatus.Error(status));
            }
            else
            {
                UpdateCameraStatus(CameraStatus.Info(status));
            }
        }

        private void UpdateCameraStatus(CameraStatus status)
        {
            CameraStatusText = status.Message;
            CameraStatusColor = status.StatusColor;
            
            // Don't override IsCameraRunning from status - let it be controlled explicitly
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            // Unsubscribe from events
            if (_cameraService != null)
            {
                _cameraService.FrameReady -= OnFrameReady;
                _cameraService.StatusChanged -= OnCameraStatusChanged;
                _cameraService.Dispose();
            }
        }
        #endregion
    }
}