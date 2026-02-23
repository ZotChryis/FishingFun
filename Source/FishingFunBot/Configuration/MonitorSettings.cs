namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for multi-monitor support and screen capture.
    /// </summary>
    public class MonitorSettings
    {
        /// <summary>
        /// Index of the monitor to capture (0-based).
        /// </summary>
        public int MonitorIndex { get; set; } = 0;

        /// <summary>
        /// Horizontal offset for capture area (pixels).
        /// </summary>
        public int CaptureOffsetX { get; set; } = 0;

        /// <summary>
        /// Vertical offset for capture area (pixels).
        /// </summary>
        public int CaptureOffsetY { get; set; } = 0;
    }
}
