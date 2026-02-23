namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for various timing parameters in the fishing bot.
    /// </summary>
    public class TimingSettings
    {
        /// <summary>
        /// Delay after right-clicking to loot (milliseconds).
        /// </summary>
        public int LootDelay { get; set; } = Constants.DefaultLootDelay;

        /// <summary>
        /// Time to watch for bobber after casting (milliseconds).
        /// </summary>
        public int CastWatchDelay { get; set; } = Constants.DefaultCastWatchDelay;

        /// <summary>
        /// Maximum time to wait for a bite before timing out (milliseconds).
        /// </summary>
        public int FishingTimeout { get; set; } = Constants.DefaultFishingTimeout;

        /// <summary>
        /// Interval between macro executions (milliseconds). Default is 10 minutes.
        /// </summary>
        public int MacroInterval { get; set; } = Constants.DefaultMacroInterval;

        /// <summary>
        /// Grace period added to macro interval (seconds).
        /// </summary>
        public int MacroExecutionDelay { get; set; } = Constants.DefaultMacroExecutionDelay;

        /// <summary>
        /// Maximum random delay added to sleep operations (milliseconds).
        /// </summary>
        public int SleepMaxRandomness { get; set; } = Constants.DefaultSleepMaxRandomness;
    }
}
