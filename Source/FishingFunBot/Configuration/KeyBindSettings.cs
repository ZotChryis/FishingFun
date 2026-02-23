using System;

namespace FishingFun.Configuration
{
    /// <summary>
    /// Configuration for keybindings used by the bot.
    /// </summary>
    public class KeyBindSettings
    {
        /// <summary>
        /// The key used to cast the fishing line.
        /// </summary>
        public ConsoleKey CastKey { get; set; } = ConsoleKey.D1;

        /// <summary>
        /// First macro key (e.g., apply lure).
        /// </summary>
        public ConsoleKey Macro1Key { get; set; } = ConsoleKey.D2;

        /// <summary>
        /// Second macro key (e.g., delete junk).
        /// </summary>
        public ConsoleKey Macro2Key { get; set; } = ConsoleKey.D3;
    }
}
