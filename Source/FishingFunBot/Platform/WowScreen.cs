using FishingFun.Configuration;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FishingFun
{
    /// <summary>
    /// Provides screen capture functionality with multi-monitor support.
    /// </summary>
    public static class WowScreen
    {
        /// <summary>
        /// Gets the color at a specific position in a bitmap.
        /// </summary>
        public static Color GetColorAt(Point pos, Bitmap bmp)
        {
            return bmp.GetPixel(pos.X, pos.Y);
        }

        /// <summary>
        /// Gets the selected screen from configuration.
        /// </summary>
        private static Screen GetSelectedScreen()
        {
            var config = ConfigurationManager.Instance.Current;
            var monitorIndex = config.Monitor.MonitorIndex;
            var screens = Screen.AllScreens;

            // Validate monitor index
            if (monitorIndex < 0 || monitorIndex >= screens.Length)
            {
                return Screen.PrimaryScreen;
            }

            return screens[monitorIndex];
        }

        /// <summary>
        /// Captures a bitmap from the configured monitor.
        /// </summary>
        public static Bitmap GetBitmap()
        {
            var config = ConfigurationManager.Instance.Current;
            var screen = GetSelectedScreen();

            var width = screen.Bounds.Width / Constants.DefaultScreenWidthDivisor;
            var height = (screen.Bounds.Height / Constants.DefaultScreenHeightDivisor) - Constants.DefaultScreenHeightOffset;

            var offsetX = screen.Bounds.X + (screen.Bounds.Width / Constants.DefaultScreenWidthDivisorForOffset) + config.Monitor.CaptureOffsetX;
            var offsetY = screen.Bounds.Y + (screen.Bounds.Height / Constants.DefaultScreenHeightDivisorForOffset) + config.Monitor.CaptureOffsetY;

            var bmpScreen = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bmpScreen))
            {
                graphics.CopyFromScreen(offsetX, offsetY, 0, 0, bmpScreen.Size);
            }

            return bmpScreen;
        }

        /// <summary>
        /// Converts a bitmap position to screen coordinates.
        /// </summary>
        public static Point GetScreenPositionFromBitmapPostion(Point pos)
        {
            var config = ConfigurationManager.Instance.Current;
            var screen = GetSelectedScreen();

            var offsetX = screen.Bounds.X + (screen.Bounds.Width / Constants.DefaultScreenWidthDivisorForOffset) + config.Monitor.CaptureOffsetX;
            var offsetY = screen.Bounds.Y + (screen.Bounds.Height / Constants.DefaultScreenHeightDivisorForOffset) + config.Monitor.CaptureOffsetY;

            return new Point(pos.X + offsetX, pos.Y + offsetY);
        }

        /// <summary>
        /// Gets the number of available monitors.
        /// </summary>
        public static int GetMonitorCount()
        {
            return Screen.AllScreens.Length;
        }

        /// <summary>
        /// Gets information about a specific monitor.
        /// </summary>
        public static string GetMonitorInfo(int index)
        {
            var screens = Screen.AllScreens;
            if (index < 0 || index >= screens.Length)
            {
                return "Invalid monitor index";
            }

            var screen = screens[index];
            var isPrimary = screen.Primary ? " (Primary)" : "";
            return $"Monitor {index + 1}: {screen.Bounds.Width}x{screen.Bounds.Height}{isPrimary}";
        }
    }
}