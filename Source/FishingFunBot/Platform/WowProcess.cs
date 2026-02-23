using FishingFun.Configuration;
using log4net;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#nullable enable

namespace FishingFun
{
    public static class WowProcess
    {
        public static ILog logger = LogManager.GetLogger("Fishbot");

        private const UInt32 WM_KEYDOWN = 0x0100;
        private const UInt32 WM_KEYUP = 0x0101;
        private static ConsoleKey lastKey;
        private static Random random = new Random();


        public static bool IsWowClassic()
        {
            var wowProcess = Get();
            return wowProcess != null ? wowProcess.ProcessName.ToLower().Contains("classic") : false; ;
        }

        //Get the wow-process, if success returns the process else null
        public static Process? Get(string name = "")
        {
            var names = string.IsNullOrEmpty(name) ? new List<string> { "Wow", "WowClassic", "Wow-64" } : new List<string> { name };

            var processList = Process.GetProcesses();
            foreach (var p in processList)
            {
                if (names.Select(s => s.ToLower()).Contains(p.ProcessName.ToLower()))
                {
                    return p;
                }
            }

            logger.Error($"Failed to find the wow process, tried: {string.Join(", ", names)}");

            return null;
        }

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        private static Process GetActiveProcess()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            return Process.GetProcessById((int)pid);
        }

        private static void KeyDown(ConsoleKey key)
        {
            lastKey = key;
            var wowProcess = Get();
            if (wowProcess != null)
            {
                PostMessage(wowProcess.MainWindowHandle, WM_KEYDOWN, (int)key, 0);
            }
        }

        private static void KeyUp()
        {
            KeyUp(lastKey);
        }

        public static void PressKey(ConsoleKey key)
        {
            KeyDown(key);
            Thread.Sleep(50 + random.Next(0, 75));
            KeyUp(key);
        }

        public static void KeyUp(ConsoleKey key)
        {
            var wowProcess = Get();
            if (wowProcess != null)
            {
                PostMessage(wowProcess.MainWindowHandle, WM_KEYUP, (int)key, 0);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// Enumeration for mouse button selection.
        /// </summary>
        public enum MouseButton
        {
            Left,
            Right
        }

        /// <summary>
        /// Consolidated mouse click method supporting different mouse buttons and animation options.
        /// </summary>
        /// <param name="logger">Logger instance for error reporting.</param>
        /// <param name="button">The mouse button to click.</param>
        /// <param name="position">The screen position to click at.</param>
        /// <param name="animate">Whether to animate cursor movement to position.</param>
        public static void ClickMouse(ILog logger, MouseButton button, System.Drawing.Point position, bool animate = true)
        {
            var activeProcess = GetActiveProcess();
            var wowProcess = WowProcess.Get();
            if (wowProcess != null)
            {
                var config = ConfigurationManager.Instance.Current;
                var lootDelay = config.Timing.LootDelay;

                var oldPosition = System.Windows.Forms.Cursor.Position;

                // Release button first to ensure clean state
                var upFlag = button == MouseButton.Right ? MouseEventFlags.RightUp : MouseEventFlags.LeftUp;
                mouse_event((int)upFlag, position.X, position.Y, 0, 0);

                Thread.Sleep(200);

                if (animate)
                {
                    System.Windows.Forms.Cursor.Position = position;
                }

                Thread.Sleep(lootDelay);

                // Click the mouse button
                var downFlag = button == MouseButton.Right ? MouseEventFlags.RightDown : MouseEventFlags.LeftDown;
                mouse_event((int)downFlag, position.X, position.Y, 0, 0);
                Thread.Sleep(30 + random.Next(0, 47));
                mouse_event((int)upFlag, position.X, position.Y, 0, 0);

                RefocusOnOldScreen(logger, activeProcess, wowProcess, oldPosition);
                Thread.Sleep(lootDelay / 2);
            }
        }

        /// <summary>
        /// Right-clicks the mouse at the specified position (legacy method for compatibility).
        /// </summary>
        public static void RightClickMouse(ILog logger, System.Drawing.Point position)
        {
            ClickMouse(logger, MouseButton.Right, position, animate: true);
        }

        /// <summary>
        /// Right-clicks the mouse at current position.
        /// </summary>
        public static void RightClickMouse()
        {
            ClickMouse(logger, MouseButton.Right, System.Windows.Forms.Cursor.Position, animate: false);
        }

        /// <summary>
        /// Left-clicks the mouse at current position.
        /// </summary>
        public static void LeftClickMouse()
        {
            ClickMouse(logger, MouseButton.Left, System.Windows.Forms.Cursor.Position, animate: false);
        }

        private static void RefocusOnOldScreen(ILog logger, Process activeProcess, Process wowProcess, System.Drawing.Point oldPosition)
        {
            try
            {
                if (activeProcess.MainWindowTitle != wowProcess.MainWindowTitle)
                {
                    // get focus back on this screen
                    PostMessage(activeProcess.MainWindowHandle, Keys.WM_RBUTTONDOWN, Keys.VK_RMB, 0);
                    Thread.Sleep(30);
                    PostMessage(activeProcess.MainWindowHandle, Keys.WM_RBUTTONUP, Keys.VK_RMB, 0);

                    KeyDown(ConsoleKey.Escape);
                    Thread.Sleep(30);
                    KeyUp(ConsoleKey.Escape);

                    System.Windows.Forms.Cursor.Position = oldPosition;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }
    }
}