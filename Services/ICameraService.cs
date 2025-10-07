using GuardVNFaceRecoginition.Models;
using System.Windows.Media.Imaging;

namespace GuardVNFaceRecoginition.Services
{
    /// <summary>
    /// Interface for camera service operations
    /// </summary>
    public interface ICameraService
    {
        bool IsRunning { get; }
        
        event Action<WriteableBitmap>? FrameReady;
        event Action<string>? StatusChanged;

        Task<bool> StartAsync(Camera camera);
        void Stop();
        void CaptureFrame();
        void Dispose();
    }
}