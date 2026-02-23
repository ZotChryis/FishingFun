using log4net;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FishingFun.Configuration
{
    /// <summary>
    /// Singleton manager for loading, saving, and migrating bot configuration.
    /// </summary>
    public sealed class ConfigurationManager
    {
        private static readonly ILog logger = LogManager.GetLogger("Fishbot");
        private static readonly Lazy<ConfigurationManager> instance = new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        private readonly string configDirectory;
        private readonly string configFilePath;
        private readonly string legacyKeybindPath;

        private BotConfiguration? currentConfiguration;

        /// <summary>
        /// Gets the singleton instance of the ConfigurationManager.
        /// </summary>
        public static ConfigurationManager Instance => instance.Value;

        /// <summary>
        /// Gets the current bot configuration.
        /// </summary>
        public BotConfiguration Current
        {
            get
            {
                if (currentConfiguration == null)
                {
                    Load();
                }
                return currentConfiguration!;
            }
        }

        private ConfigurationManager()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            configDirectory = Path.Combine(appDataPath, Constants.AppDataFolder);
            configFilePath = Path.Combine(configDirectory, Constants.ConfigFileName);
            legacyKeybindPath = Path.Combine(configDirectory, Constants.LegacyKeybindFileName);

            EnsureDirectoryExists();
        }

        /// <summary>
        /// Loads configuration from disk. Creates default configuration if none exists.
        /// Migrates legacy keybind.txt if present.
        /// </summary>
        public void Load()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    logger.Info($"Loading configuration from {configFilePath}");
                    var json = File.ReadAllText(configFilePath);
                    currentConfiguration = JsonConvert.DeserializeObject<BotConfiguration>(json);

                    if (currentConfiguration == null)
                    {
                        logger.Warn("Failed to deserialize configuration, using defaults");
                        currentConfiguration = new BotConfiguration();
                    }
                    else
                    {
                        logger.Info("Configuration loaded successfully");
                    }
                }
                else
                {
                    logger.Info("No configuration file found, creating default configuration");
                    currentConfiguration = new BotConfiguration();

                    // Attempt to migrate legacy keybind.txt
                    MigrateLegacyKeybinds();

                    // Save the new configuration
                    Save();
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error loading configuration: {ex.Message}", ex);
                logger.Info("Using default configuration");
                currentConfiguration = new BotConfiguration();
            }
        }

        /// <summary>
        /// Saves the current configuration to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                if (currentConfiguration == null)
                {
                    logger.Warn("Attempted to save null configuration");
                    return;
                }

                EnsureDirectoryExists();

                var json = JsonConvert.SerializeObject(currentConfiguration, Formatting.Indented);
                File.WriteAllText(configFilePath, json);

                logger.Info($"Configuration saved to {configFilePath}");
            }
            catch (Exception ex)
            {
                logger.Error($"Error saving configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates the current configuration and saves it to disk.
        /// </summary>
        public void Update(BotConfiguration newConfiguration)
        {
            currentConfiguration = newConfiguration;
            Save();
        }

        /// <summary>
        /// Resets configuration to defaults.
        /// </summary>
        public void Reset()
        {
            logger.Info("Resetting configuration to defaults");
            currentConfiguration = new BotConfiguration();
            Save();
        }

        /// <summary>
        /// Migrates legacy keybind.txt file to new configuration system.
        /// </summary>
        private void MigrateLegacyKeybinds()
        {
            if (!File.Exists(legacyKeybindPath))
            {
                logger.Info("No legacy keybind.txt file found, skipping migration");
                return;
            }

            try
            {
                logger.Info($"Migrating legacy keybinds from {legacyKeybindPath}");
                var lines = File.ReadAllLines(legacyKeybindPath);

                if (lines.Length >= 1 && Enum.TryParse<ConsoleKey>(lines[0].Trim(), true, out var castKey))
                {
                    currentConfiguration!.KeyBinds.CastKey = castKey;
                    logger.Info($"Migrated CastKey: {castKey}");
                }

                if (lines.Length >= 2 && Enum.TryParse<ConsoleKey>(lines[1].Trim(), true, out var macro1Key))
                {
                    currentConfiguration!.KeyBinds.Macro1Key = macro1Key;
                    logger.Info($"Migrated Macro1Key: {macro1Key}");
                }

                if (lines.Length >= 3 && Enum.TryParse<ConsoleKey>(lines[2].Trim(), true, out var macro2Key))
                {
                    currentConfiguration!.KeyBinds.Macro2Key = macro2Key;
                    logger.Info($"Migrated Macro2Key: {macro2Key}");
                }

                logger.Info("Legacy keybind migration completed successfully");
            }
            catch (Exception ex)
            {
                logger.Error($"Error migrating legacy keybinds: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ensures the configuration directory exists.
        /// </summary>
        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
                logger.Info($"Created configuration directory: {configDirectory}");
            }
        }

        /// <summary>
        /// Gets the path to the configuration directory.
        /// </summary>
        public string GetConfigDirectory() => configDirectory;

        /// <summary>
        /// Gets the path to the configuration file.
        /// </summary>
        public string GetConfigFilePath() => configFilePath;
    }
}
