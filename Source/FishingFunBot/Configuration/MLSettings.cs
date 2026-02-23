using System;
using System.IO;

namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for machine learning based bobber detection.
    /// </summary>
    public class MLSettings
    {
        /// <summary>
        /// Whether ML detection is enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Minimum confidence threshold for ML detections (0.0 to 1.0).
        /// </summary>
        public float ConfidenceThreshold { get; set; } = Constants.DefaultMLConfidenceThreshold;

        /// <summary>
        /// Path to the ONNX model file.
        /// </summary>
        public string ModelPath { get; set; } = GetDefaultModelPath();

        /// <summary>
        /// Gets the default path for the ONNX model file.
        /// </summary>
        private static string GetDefaultModelPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var modelsDir = Path.Combine(appDataPath, Constants.AppDataFolder, Constants.ModelsFolder);
            return Path.Combine(modelsDir, Constants.DefaultModelFileName);
        }
    }
}
