using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace FishingFun
{
    /// <summary>
    /// Composite bobber finder that attempts multiple detection methods in sequence.
    /// Implements chain of responsibility pattern for fallback detection.
    /// </summary>
    public class FallbackBobberFinder : IBobberFinder, IImageProvider
    {
        private static readonly ILog logger = LogManager.GetLogger("Fishbot");
        private readonly List<IBobberFinder> finders;
        private IBobberFinder? lastSuccessfulFinder;

        public event EventHandler<BobberBitmapEvent> BitmapEvent;

        public FallbackBobberFinder(List<IBobberFinder> finders)
        {
            if (finders == null || finders.Count == 0)
            {
                throw new ArgumentException("At least one finder must be provided", nameof(finders));
            }

            this.finders = finders;
            BitmapEvent += (s, e) => { };

            // Subscribe to bitmap events from child finders
            foreach (var finder in finders)
            {
                if (finder is IImageProvider imageProvider)
                {
                    imageProvider.BitmapEvent += (s, e) => BitmapEvent?.Invoke(this, e);
                }
            }

            logger.Info($"FallbackBobberFinder created with {finders.Count} detection methods");
        }

        public void Reset()
        {
            foreach (var finder in finders)
            {
                finder.Reset();
            }
            lastSuccessfulFinder = null;
        }

        public Point Find(CancellationToken cancellationToken = default)
        {
            // Try last successful finder first for performance
            if (lastSuccessfulFinder != null && !cancellationToken.IsCancellationRequested)
            {
                var quickResult = lastSuccessfulFinder.Find(cancellationToken);
                if (quickResult != Point.Empty)
                {
                    return quickResult;
                }
            }

            // Fall back to trying all finders in order
            foreach (var finder in finders)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Point.Empty;
                }

                try
                {
                    var result = finder.Find(cancellationToken);
                    if (result != Point.Empty)
                    {
                        logger.Info($"Bobber found using {finder.GetType().Name}");
                        lastSuccessfulFinder = finder;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn($"Detection method {finder.GetType().Name} failed: {ex.Message}");
                    // Continue to next finder
                }
            }

            logger.Debug("No detection method found the bobber");
            return Point.Empty;
        }
    }
}
