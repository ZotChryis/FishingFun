namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for bobber detection and bite watching.
    /// </summary>
    public class DetectionSettings
    {
        /// <summary>
        /// Minimum movement threshold to detect a bite.
        /// </summary>
        public int StrikeThreshold { get; set; } = Constants.DefaultStrikeThreshold;

        /// <summary>
        /// Search radius for bobber detection algorithms.
        /// </summary>
        public int SearchRadius { get; set; } = Constants.DefaultSearchRadius;

        /// <summary>
        /// Timeout for searching for the bobber (milliseconds).
        /// </summary>
        public int BobberSearchTimeout { get; set; } = Constants.DefaultBobberSearchTimeout;

        /// <summary>
        /// Interval between bobber search attempts (milliseconds).
        /// </summary>
        public int BobberSearchInterval { get; set; } = Constants.DefaultBobberSearchInterval;
    }
}
