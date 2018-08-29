/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Serilog;
using Serilog.Events;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Mamesaver
{
    public class Program
    {
        private static Container _container;
        private static bool _debug;

        [STAThread]
        public static void Main(string[] args)
        {
            ConfigureErrorHandling();

            try
            {
                // Initialise the DI container
                _container = ContainerFactory.NewContainer();

                SetDebugFlag();
                ConfigureLogging();

                // Perform a sanity check on component installation
                _container.Verify(VerificationOption.VerifyAndDiagnose);

                // Default to config if no options passed
                var arguments = args.Length != 0 ? args : new[] { "/c" };

                Log.Information("Mamesaver started with args {arguments}", string.Join(",", args));

                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var saver = _container.GetInstance<Mamesaver>();

                    switch (arguments[0].Trim().Substring(0, 2).ToLower())
                    {
                        case "/c":
                            //TODO: Catch display properties window handle and set it as parent
                            ShowConfig();
                            break;

                        case "/s":
                            saver.Run();
                            break;

                        case "/p":
                            // do nothing
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Main");
                DisplayError();
            }
        }

        /// <summary>
        ///     Configures global error handling.
        /// </summary>
        private static void ConfigureErrorHandling()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject as Exception, "Thread exception");
            DisplayError();

            // Screensaver is in an unhandled state; force exit
            Environment.Exit(-1);
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Thread exception");
            DisplayError();
            Application.Exit();
        }

        private static void DisplayError()
        {
            MessageBox.Show(@"Error running screensaver. Verify that your MAME path and and arguments are correct.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        ///     Configures event log and filesystem logging. 
        /// </summary>
        /// <remarks>
        ///     Logging is written to the filesystem in debug builds and when enabled by the user.
        /// </remarks>
        public static void ConfigureLogging()
        {
            // Configure debug logging if requested by the user or if we are running a debug build
            var advancedSettings = _container.GetInstance<AdvancedSettings>();
            if (!advancedSettings.DebugLogging && !_debug) return;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.EventLog("Mamesaver", restrictedToMinimumLevel: LogEventLevel.Warning)
                .WriteTo.File(Path.Combine(Path.GetTempPath(), "MameSaver", "Logs", "MameSaver-.txt"),
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 100000,
                    retainedFileCountLimit: 5)
                .CreateLogger();
        }

        public static void ShowConfig()
        {
            Application.EnableVisualStyles();
            Application.Run(_container.GetInstance<ConfigForm>());
        }

        [Conditional("DEBUG")]
        private static void SetDebugFlag() => _debug = true;
    }
}