using GuardVNFaceRecoginition.Models;

namespace GuardVNFaceRecoginition.Services
{
    /// <summary>
    /// Interface for message/dialog service
    /// </summary>
    public interface IMessageService
    {
        void ShowMessage(string message, string title = "GuardVN");
        void ShowWarning(string message, string title = "GuardVN");
        void ShowError(string message, string title = "GuardVN");
        bool ShowConfirmation(string message, string title = "GuardVN");
    }
}