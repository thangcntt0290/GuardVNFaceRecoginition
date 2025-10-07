using System.Windows.Media;

namespace GuardVNFaceRecoginition.Models
{
    /// <summary>
    /// Model for camera status information
    /// </summary>
    public class CameraStatus
    {
        public string Message { get; set; } = string.Empty;
        public Color StatusColor { get; set; } = Colors.Gray;
        public bool IsRunning { get; set; } = false;

        public static CameraStatus Success(string message) =>
            new() { Message = message, StatusColor = Colors.Green, IsRunning = true };

        public static CameraStatus Error(string message) =>
            new() { Message = message, StatusColor = Colors.Red, IsRunning = false };

        public static CameraStatus Info(string message) =>
            new() { Message = message, StatusColor = Colors.Blue, IsRunning = false };

        public static CameraStatus Default(string message) =>
            new() { Message = message, StatusColor = Colors.Gray, IsRunning = false };
    }
}