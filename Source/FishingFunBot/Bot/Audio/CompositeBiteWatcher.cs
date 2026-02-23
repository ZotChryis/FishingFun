using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FishingFun.Audio
{
    /// <summary>
    /// Combines multiple bite watchers (visual + audio) using OR logic.
    /// Returns true if ANY watcher detects a bite.
    /// </summary>
    public class CompositeBiteWatcher : IBiteWatcher
    {
        private readonly List<IBiteWatcher> watchers;

        public Action<FishingEvent> FishingEventHandler { get; set; }

        public CompositeBiteWatcher(params IBiteWatcher[] watchers)
        {
            this.watchers = watchers.ToList();
            FishingEventHandler = (e) => { }; // Initialize with no-op to avoid null
        }

        public void AddWatcher(IBiteWatcher watcher)
        {
            watchers.Add(watcher);
        }

        public bool IsBite(Point currentBobberPosition)
        {
            // Return true if ANY watcher detects a bite
            return watchers.Any(w => w.IsBite(currentBobberPosition));
        }

        public void Reset(Point bobberPosition)
        {
            foreach (var watcher in watchers)
            {
                watcher.Reset(bobberPosition);
            }
        }
    }
}
