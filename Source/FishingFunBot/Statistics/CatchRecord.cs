using System;

namespace FishingFun.Statistics
{
    /// <summary>
    /// Represents an individual catch attempt.
    /// </summary>
    public class CatchRecord
    {
        public int RecordId { get; set; }
        public int SessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public int BiteTimeMs { get; set; }
        public string DetectionMethod { get; set; } = string.Empty;
        public int BobberFindTimeMs { get; set; }
    }
}
