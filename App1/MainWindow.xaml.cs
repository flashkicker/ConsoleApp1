using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    using App1.PhotoStoreDemo;
    using Microsoft.Windows.AppLifecycle;
    using System;
    using System.Windows;

    namespace PhotoStoreDemo
    {
    }


    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";

            string installerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "installers", "HelloWorldChildApp.msix");

            var cmdArgs = Environment.GetCommandLineArgs();
            // If app isn't running with identity, register its sparse package
            if (!IsRunningWithIdentity())
            {
                // Attempt registration
                if (RegisterSparsePackage(installerPath))
                {
                    // Registration succeeded, restart the app to run with identity
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName, arguments: cmdArgs?.ToString());
                    return;
                }
                else
                {
                    // Registration failed, run without identity
                    Debug.WriteLine("Package Registration failed, running WITHOUT Identity");
                }
            }
            else
            {
                // App is registered and running with identity, handle launch and activation
                var activationArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetInstances().First().GetActivatedEventArgs();
                if (activationArgs != null)
                {
                    switch (activationArgs.Kind)
                    {
                        // Handle other activation scenarios as needed
                        case ExtendedActivationKind.Launch:
                            HandleLaunch(activationArgs);
                            break;
                        default:
                            HandleLaunch(null);
                            break;
                    }
                }
                else
                {
                    // Direct exe-based launch, e.g., double click on app .exe or desktop shortcut
                    SingleInstanceManager singleInstanceManager = new SingleInstanceManager();
                    singleInstanceManager.Run(cmdArgs);
                }
            }


        }
        private static bool IsRunningWithIdentity()
        {
            try
            {
                return Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().IsCurrent;
            }
            catch
            {
                return false;
            }
        }

        static void HandleLaunch(AppActivationArguments args)
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            //Debug.AutoFlush = true;
            //Debug.Indent();
            //Debug.WriteLine("WPF App using a Sparse Package");

            SingleInstanceManager singleInstanceManager = new SingleInstanceManager();
            singleInstanceManager.Run(Environment.GetCommandLineArgs());
        }

        private static bool RegisterSparsePackage(string installerPath)
        {
            bool registration = false;
            try
            {
                Uri packageUri = new Uri(installerPath);

                // Initialize PackageManager
                PackageManager packageManager = new PackageManager();

                // Declare the use of an external location
                var options = new AddPackageOptions();

                // Attempt to register the Sparse Package
                var deploymentOperation = packageManager.AddPackageByUriAsync(packageUri, options);

                // Wait for the completion of the deployment operation
                deploymentOperation.AsTask().Wait();

                // Check the status of the deployment
                if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    registration = true;
                    Debug.WriteLine("Package Registration succeeded!");
                }
                else
                {
                    Windows.Management.Deployment.DeploymentResult deploymentResult = deploymentOperation.GetResults();
                    Debug.WriteLine("Installation Error: {0}", deploymentOperation.ErrorCode);
                    Debug.WriteLine("Detailed Error Text: {0}", deploymentResult.ErrorText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddPackageSample failed, error message: {0}", ex.Message);
                Console.WriteLine("Full Stacktrace: {0}", ex.ToString());
            }

            return registration;
        }
    }
}

