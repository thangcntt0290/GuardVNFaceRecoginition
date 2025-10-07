using System.Windows;

namespace GuardVNFaceRecoginition.Services
{
    /// <summary>
    /// Implementation of message service using WPF MessageBox
    /// </summary>
    public class MessageService : IMessageService
    {
        public void ShowMessage(string message, string title = "GuardVN")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarning(string message, string title = "GuardVN")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowError(string message, string title = "GuardVN")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirmation(string message, string title = "GuardVN")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}