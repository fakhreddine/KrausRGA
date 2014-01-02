using KrausRGA.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace KrausRGA.Views
{
    public static class WindowThread
    {
        public static Thread newWindowThread;
        /// <summary>
        /// Start new thread of the window in the application for wait screen.
        /// </summary>
        public static void start()
        {
            newWindowThread = new Thread(new ThreadStart(() =>
            {
                // Create and show the Window
                wndWait tempWindow = new wndWait();
                tempWindow.Activate();
                tempWindow.Topmost = false;
                tempWindow.Focus();
                tempWindow.ShowActivated = true;
                tempWindow.Show();

                // Start the Dispatcher Processing
                System.Windows.Threading.Dispatcher.Run();
            }));
            // Set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);
            // Make the thread a background thread
            newWindowThread.IsBackground = true;
            // Start the thread
            newWindowThread.Start();

        }

        public static void Stop()
        {
            if (newWindowThread.IsAlive)
            {
                newWindowThread.Abort();
            }
        }
    }
}
