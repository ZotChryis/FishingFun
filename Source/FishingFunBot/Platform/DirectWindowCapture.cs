using log4net;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FishingFun.Platform
{
    /// <summary>
    /// Provides direct window capture using PrintWindow API for minimized/background capture.
    /// Note: PrintWindow may not work with DirectX games like WoW in all cases.
    /// Windowed mode is recommended for best compatibility.
    /// </summary>
    public static class DirectWindowCapture
    {
        private static readonly ILog logger = LogManager.GetLogger("Fishbot");

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Attempts to capture a window even when minimized.
        /// </summary>
        /// <param name="process">The WoW process to capture.</param>
        /// <returns>Bitmap of the window, or null if capture failed.</returns>
        public static Bitmap? CaptureWindow(Process process)
        {
            try
            {
                var handle = process.MainWindowHandle;
                if (handle == IntPtr.Zero)
                {
                    logger.Warn("Cannot capture window: Invalid handle");
                    return null;
                }

                RECT rect = new RECT();
                if (!GetWindowRect(handle, ref rect))
                {
                    logger.Warn("Cannot get window rect");
                    return null;
                }

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width <= 0 || height <= 0)
                {
                    logger.Warn("Invalid window dimensions");
                    return null;
                }

                var bitmap = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr hdc = graphics.GetHdc();
                    try
                    {
                        if (!PrintWindow(handle, hdc, 0))
                        {
                            logger.Warn("PrintWindow failed - window may be using DirectX rendering");
                            return null;
                        }
                    }
                    finally
                    {
                        graphics.ReleaseHdc(hdc);
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                logger.Error($"DirectWindowCapture error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if direct window capture is likely to work for the current process.
        /// </summary>
        public static bool IsCompatible(Process process)
        {
            // PrintWindow typically doesn't work with DirectX exclusive fullscreen
            // It may work with windowed or borderless windowed modes
            return process.MainWindowHandle != IntPtr.Zero;
        }
    }
}
