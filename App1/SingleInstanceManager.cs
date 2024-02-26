// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    using System;
    using System.Threading;

    namespace PhotoStoreDemo
    {
        public class SingleInstanceManager : IDisposable
        {
            private Mutex mutex;

            public SingleInstanceManager()
            {
                // Create a unique named mutex to ensure single instance
                mutex = new Mutex(true, "{Your-Unique-Guid-Here}");
            }

            public void Run(string[] args)
            {
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    try
                    {
                        // Start the application
                        App app = new App();
                        app.InitializeComponent();
                        app.Run();
                    }
                    finally
                    {
                        // Release the mutex on application exit
                        mutex.ReleaseMutex();
                    }
                }
                else
                {
                    // Another instance is already running, handle accordingly
                    // For example, bring the existing instance to the foreground
                    // or show a message to the user
                }
            }

            public void Dispose()
            {
                mutex.Close();
            }
        }
    }

}
