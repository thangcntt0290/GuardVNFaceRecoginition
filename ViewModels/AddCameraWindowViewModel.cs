using System.Collections.Generic;
using System.Windows.Input;
using GuardVNFaceRecoginition.Commands;
using GuardVNFaceRecoginition.Models;
using GuardVNFaceRecoginition.Services;

namespace GuardVNFaceRecoginition.ViewModels
{
    /// <summary>
    /// ViewModel for AddCameraWindow
    /// </summary>
    public class AddCameraWindowViewModel : ViewModelBase
    {
        #region Private Fields
        private readonly IMessageService _messageService;
        private string _cameraName = string.Empty;
        private string _sourceType = "Webcam";
        private string _source = string.Empty;
        private bool _isConfirmed = false;
        #endregion

        #region Properties
        public string CameraName
        {
            get => _cameraName;
            set
            {
                if (SetProperty(ref _cameraName, value))
                {
                    (AddCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string SourceType
        {
            get => _sourceType;
            set
            {
                if (SetProperty(ref _sourceType, value))
                {
                    OnSourceTypeChanged();
                }
            }
        }

        public string Source
        {
            get => _source;
            set
            {
                if (SetProperty(ref _source, value))
                {
                    (AddCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsConfirmed
        {
            get => _isConfirmed;
            private set => SetProperty(ref _isConfirmed, value);
        }

        public Camera? Camera { get; private set; }

        public List<string> SourceTypes { get; } = new() { "Webcam", "RTSP" };

        public string PlaceholderText => SourceType == "Webcam" ? "Enter webcam ID (e.g., 0)" : "Enter RTSP URL (e.g., rtsp://...)";
        #endregion

        #region Commands
        public ICommand AddCameraCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Events
        public event Action? CloseRequested;
        #endregion

        #region Constructor
        public AddCameraWindowViewModel(IMessageService messageService)
        {
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

            AddCameraCommand = new RelayCommand(AddCamera, CanAddCamera);
            CancelCommand = new RelayCommand(Cancel);
        }
        #endregion

        #region Private Methods
        private bool CanAddCamera()
        {
            return !string.IsNullOrWhiteSpace(CameraName) && !string.IsNullOrWhiteSpace(Source);
        }

        private void AddCamera()
        {
            if (!ValidateInput()) return;

            try
            {
                Camera = new Camera
                {
                    Name = CameraName.Trim(),
                    SourceType = SourceType,
                    Source = Source.Trim(),
                    DeviceIndex = SourceType == "Webcam" && int.TryParse(Source.Trim(), out int idx) ? idx : 0
                };

                IsConfirmed = true;
                CloseRequested?.Invoke();
            }
            catch (Exception ex)
            {
                _messageService.ShowError($"Error creating camera: {ex.Message}");
            }
        }

        private void Cancel()
        {
            IsConfirmed = false;
            CloseRequested?.Invoke();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(CameraName))
            {
                _messageService.ShowWarning("Please enter a camera name.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Source))
            {
                _messageService.ShowWarning("Please enter the link stream or webcam ID.");
                return false;
            }

            if (SourceType == "Webcam" && !int.TryParse(Source.Trim(), out _))
            {
                _messageService.ShowWarning("Please enter a valid webcam ID (number).");
                return false;
            }

            if (SourceType == "RTSP" && !Source.Trim().StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase))
            {
                _messageService.ShowWarning("Please enter a valid RTSP URL starting with 'rtsp://'.");
                return false;
            }

            return true;
        }

        private void OnSourceTypeChanged()
        {
            // Clear source when type changes
            Source = string.Empty;
            OnPropertyChanged(nameof(PlaceholderText));
        }
        #endregion
    }
}