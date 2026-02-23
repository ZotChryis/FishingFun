namespace FishingFun.Configuration
{
    /// <summary>
    /// Centralized constants for the FishingFun bot to eliminate magic numbers.
    /// </summary>
    public static class Constants
    {
        // Detection thresholds
        public const int DefaultStrikeThreshold = 7;
        public const int DefaultSearchRadius = 40;
        public const int DefaultClusterTolerance = 10;

        // Timing constants (milliseconds)
        public const int DefaultLootDelay = 2000;
        public const int DefaultCastWatchDelay = 2000;
        public const int DefaultFishingTimeout = 25000;
        public const int DefaultBobberSearchInterval = 1000;
        public const int DefaultBobberSearchTimeout = 5000;
        public const int DefaultSleepMaxRandomness = 225;
        public const int DefaultSleepCheckInterval = 100;

        // Macro timing (milliseconds)
        public const int DefaultMacroInterval = 600000; // 10 minutes
        public const int DefaultMacroExecutionDelay = 10; // seconds grace period

        // Color detection defaults
        public const double DefaultColourMultiplier = 0.5;
        public const double DefaultColourClosenessMultiplier = 2.0;
        public const double ClassicColourMultiplier = 1.0;
        public const double ClassicColourClosenessMultiplier = 1.0;
        public const int DefaultColorClosenessTolerance = 20;

        // ML detection defaults
        public const float DefaultMLConfidenceThreshold = 0.7f;
        public const int DefaultMLInputWidth = 640;
        public const int DefaultMLInputHeight = 640;

        // Audio detection defaults
        public const float DefaultAudioSensitivity = 0.5f;
        public const int DefaultAudioBufferSize = 4096;

        // Configuration paths
        public const string AppDataFolder = "FishingFun";
        public const string ConfigFileName = "config.json";
        public const string StatisticsDbFileName = "statistics.db";
        public const string ModelsFolder = "models";
        public const string DefaultModelFileName = "bobber_detection.onnx";
        public const string LegacyKeybindFileName = "keybind.txt";

        // Screen capture defaults
        public const int DefaultScreenWidthDivisor = 2;
        public const int DefaultScreenHeightDivisor = 2;
        public const int DefaultScreenHeightOffset = 100;
        public const int DefaultScreenWidthDivisorForOffset = 4;
        public const int DefaultScreenHeightDivisorForOffset = 4;
    }
}
