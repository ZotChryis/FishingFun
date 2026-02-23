using System;
using System.Windows;
using System.Windows.Threading;

namespace FishingFun.Utilities
{
    /// <summary>
    /// Helper class for dispatcher operations to eliminate code duplication.
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Dispatches an action on the UI thread and waits for background priority operations to complete.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        public static void Dispatch(Action action)
        {
            Application.Current?.Dispatcher.BeginInvoke((Action)(() => action()));
            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }
    }
}
