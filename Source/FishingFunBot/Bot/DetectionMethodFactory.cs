using FishingFun.Configuration;
using log4net;
using System.Collections.Generic;

namespace FishingFun
{
    /// <summary>
    /// Factory for creating configured detection method chains.
    /// </summary>
    public static class DetectionMethodFactory
    {
        private static readonly ILog logger = LogManager.GetLogger("Fishbot");

        /// <summary>
        /// Creates a bobber finder based on current configuration.
        /// Returns a fallback chain if multiple methods are available, otherwise a single finder.
        /// </summary>
        public static IBobberFinder CreateBobberFinder(BotConfiguration config)
        {
            var finders = new List<IBobberFinder>();

            // ML detection (highest priority, will be added in Phase 5)
            // if (config.ML.Enabled && MLModelExists())
            // {
            //     finders.Add(new MLBobberFinder(config.ML));
            // }

            // Color clustering detection (SearchBobberFinder)
            var pixelClassifier = new PixelClassifier();
            finders.Add(new SearchBobberFinder(pixelClassifier));

            // Exact color detection could be added here as fallback
            // if (config.Color.EnableExactColorFallback)
            // {
            //     finders.Add(new BobberColourPointFinder(config.Color.TargetColor));
            // }

            logger.Info($"Created detection chain with {finders.Count} method(s)");

            // If only one finder, return it directly for performance
            if (finders.Count == 1)
            {
                return finders[0];
            }

            // Return fallback chain if multiple methods
            return new FallbackBobberFinder(finders);
        }

        /// <summary>
        /// Creates a bobber finder with a specific pixel classifier.
        /// This is for backward compatibility with existing UI code.
        /// </summary>
        public static IBobberFinder CreateBobberFinder(IPixelClassifier pixelClassifier)
        {
            logger.Info("Created SearchBobberFinder with custom pixel classifier");
            return new SearchBobberFinder(pixelClassifier);
        }
    }
}
