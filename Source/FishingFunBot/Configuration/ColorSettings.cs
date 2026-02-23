using FishingFun;

namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for color-based bobber detection.
    /// </summary>
    public class ColorSettings
    {
        /// <summary>
        /// The color detection mode (Red or Blue).
        /// </summary>
        public PixelClassifier.ClassifierMode Mode { get; set; } = PixelClassifier.ClassifierMode.Red;

        /// <summary>
        /// Multiplier for determining if one color component is significantly larger than another.
        /// </summary>
        public double ColourMultiplier { get; set; } = Constants.DefaultColourMultiplier;

        /// <summary>
        /// Multiplier for determining if two color components are close in value.
        /// </summary>
        public double ColourClosenessMultiplier { get; set; } = Constants.DefaultColourClosenessMultiplier;

        /// <summary>
        /// Whether to use WoW Classic detection parameters.
        /// </summary>
        public bool IsWowClassic { get; set; } = false;

        /// <summary>
        /// Tolerance for color clustering algorithm.
        /// </summary>
        public int ClusterTolerance { get; set; } = Constants.DefaultClusterTolerance;
    }
}
