using System;

namespace FishingFun.Statistics
{
    /// <summary>
    /// Represents a fishing bot session with performance metrics.
    /// </summary>
    public class FishingSession
    {
        public int SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CatchCount { get; set; }
        public int MissCount { get; set; }
        public int TimeoutCount { get; set; }
        public string DetectionMethodUsed { get; set; } = string.Empty;

        public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
        public double SuccessRate => CatchCount + MissCount > 0 ? (double)CatchCount / (CatchCount + MissCount) * 100 : 0;
    }
}
