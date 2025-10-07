namespace GuardVNFaceRecoginition.Models
{
    /// <summary>
    /// Camera model class
    /// </summary>
    public class Camera
    {
        public string Name { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty; // "Webcam" or "RTSP"
        public string Source { get; set; } = string.Empty; // RTSP link or webcam ID
        public int DeviceIndex { get; set; } = 0;

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Helper property for display
        /// </summary>
        public string DisplayInfo => $"{Name} ({SourceType})";
    }
}