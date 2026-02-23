namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for audio-based splash detection.
    /// </summary>
    public class AudioSettings
    {
        /// <summary>
        /// Whether audio detection is enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Sensitivity threshold for splash detection (0.0 to 1.0).
        /// </summary>
        public float SensitivityThreshold { get; set; } = Constants.DefaultAudioSensitivity;

        /// <summary>
        /// Audio buffer size for capture.
        /// </summary>
        public int BufferSize { get; set; } = Constants.DefaultAudioBufferSize;
    }
}
