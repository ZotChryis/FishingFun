using System;
using System.IO;

namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for statistics tracking and reporting.
    /// </summary>
    public class StatisticsSettings
    {
        /// <summary>
        /// Whether statistics tracking is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Path to the SQLite database file for statistics.
        /// </summary>
        public string DatabasePath { get; set; } = GetDefaultDatabasePath();

        /// <summary>
        /// Gets the default path for the statistics database.
        /// </summary>
        private static string GetDefaultDatabasePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDir = Path.Combine(appDataPath, Constants.AppDataFolder);
            return Path.Combine(appDir, Constants.StatisticsDbFileName);
        }
    }
}
