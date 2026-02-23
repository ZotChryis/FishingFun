using FishingFun.Configuration;
using log4net;
using System;

namespace FishingFun.Statistics
{
    /// <summary>
    /// Collects fishing statistics in real-time.
    /// Stub implementation - requires SQLite package for persistence.
    /// </summary>
    public class StatisticsCollector
    {
        private static readonly ILog logger = LogManager.GetLogger("Fishbot");
        private readonly StatisticsSettings config;
        private FishingSession? currentSession;

        public StatisticsCollector(StatisticsSettings config)
        {
            this.config = config;
        }

        public void StartSession()
        {
            if (!config.Enabled)
            {
                return;
            }

            currentSession = new FishingSession
            {
                StartTime = DateTime.Now
            };

            logger.Info("Statistics session started");
        }

        public void EndSession()
        {
            if (currentSession == null)
            {
                return;
            }

            currentSession.EndTime = DateTime.Now;
            logger.Info($"Statistics session ended. Catches: {currentSession.CatchCount}, Success rate: {currentSession.SuccessRate:F1}%");

            // TODO: Save to SQLite database when package is added
            currentSession = null;
        }

        public void RecordCatch(bool success, int biteTimeMs, string detectionMethod)
        {
            if (currentSession == null)
            {
                return;
            }

            if (success)
            {
                currentSession.CatchCount++;
            }
            else
            {
                currentSession.MissCount++;
            }

            // TODO: Save catch record to database
        }

        public void RecordTimeout()
        {
            if (currentSession == null)
            {
                return;
            }

            currentSession.TimeoutCount++;
        }
    }
}
