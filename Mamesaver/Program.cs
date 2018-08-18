/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.IO;
using System.Windows.Forms;
using Serilog;

namespace Mamesaver
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ConfigureLogging();

            Application.ThreadException += (sender, eventArgs) => Log.Error(eventArgs.Exception, "Thread exception");

            try
            {
                //default to config if no options passed
                var arguments = args.Length != 0 ? args : new[] {"/c"};

                Log.Information("Mamesaver started with args " + string.Join(",", args));

                var saver = new Mamesaver();

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

                saver.Dispose();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Main");
            }
        }

        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Path.GetTempPath(), "MameSaver-.txt"),
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 100000,
                    retainedFileCountLimit: 5)
                .CreateLogger();
        }

        public static void ShowConfig()
        {
            var configForm = new ConfigForm();
            Application.EnableVisualStyles();
            Application.Run(configForm);
        }
    }
}