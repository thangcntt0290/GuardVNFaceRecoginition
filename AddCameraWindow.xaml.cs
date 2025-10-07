using System.Windows;
using GuardVNFaceRecoginition.Models;
using GuardVNFaceRecoginition.Services;
using GuardVNFaceRecoginition.ViewModels;

namespace GuardVNFaceRecoginition
{
    /// <summary>
    /// Interaction logic for AddCameraWindow.xaml
    /// </summary>
    public partial class AddCameraWindow : Window
    {
        private AddCameraWindowViewModel? _viewModel;

        public Camera? Camera => _viewModel?.Camera;
        public bool IsConfirmed => _viewModel?.IsConfirmed ?? false;

        public AddCameraWindow()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            var messageService = new MessageService();
            _viewModel = new AddCameraWindowViewModel(messageService);
            _viewModel.CloseRequested += OnCloseRequested;
            DataContext = _viewModel;
        }

        private void OnCloseRequested()
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CloseRequested -= OnCloseRequested;
            }
            base.OnClosed(e);
        }
    }
}