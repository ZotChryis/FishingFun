namespace FishingFun.Configuration
{
    /// <summary>
    /// Main configuration class for the FishingFun bot containing all settings.
    /// </summary>
    public class BotConfiguration
    {
        /// <summary>
        /// Keybinding settings for cast and macro keys.
        /// </summary>
        public KeyBindSettings KeyBinds { get; set; } = new KeyBindSettings();

        /// <summary>
        /// Detection algorithm settings.
        /// </summary>
        public DetectionSettings Detection { get; set; } = new DetectionSettings();

        /// <summary>
        /// Timing and delay settings.
        /// </summary>
        public TimingSettings Timing { get; set; } = new TimingSettings();

        /// <summary>
        /// Color-based detection settings.
        /// </summary>
        public ColorSettings Color { get; set; } = new ColorSettings();

        /// <summary>
        /// Multi-monitor and screen capture settings.
        /// </summary>
        public MonitorSettings Monitor { get; set; } = new MonitorSettings();

        /// <summary>
        /// Machine learning detection settings.
        /// </summary>
        public MLSettings ML { get; set; } = new MLSettings();

        /// <summary>
        /// Audio-based detection settings.
        /// </summary>
        public AudioSettings Audio { get; set; } = new AudioSettings();

        /// <summary>
        /// Statistics tracking settings.
        /// </summary>
        public StatisticsSettings Statistics { get; set; } = new StatisticsSettings();

        /// <summary>
        /// Creates a new BotConfiguration with default values.
        /// </summary>
        public BotConfiguration()
        {
        }

        /// <summary>
        /// Creates a deep clone of this configuration.
        /// </summary>
        public BotConfiguration Clone()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<BotConfiguration>(json) ?? new BotConfiguration();
        }
    }
}
