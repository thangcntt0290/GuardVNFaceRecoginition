using System.Windows;
using GuardVNFaceRecoginition.Services;
using GuardVNFaceRecoginition.ViewModels;

namespace GuardVNFaceRecoginition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            // Create services - Use updated real camera service
            var cameraService = new Services.CameraService();
            var messageService = new Services.MessageService();

            // Create and set ViewModel
            _viewModel = new MainWindowViewModel(cameraService, messageService);
            DataContext = _viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            // Cleanup ViewModel
            _viewModel?.Dispose();
            base.OnClosed(e);
        }
    }
}