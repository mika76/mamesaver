/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;

namespace Mamesaver
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.ThreadException += Application_ThreadException;

            try
            {
                string[] arguments = {"/c"};

                if (args.Length != 0) //default to config if no options passed
                    arguments = args;

#if DEBUG
                Log("Mamesaver started with args " + string.Join(",", args));
#endif

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
            catch(Exception x)
            {
                Log(x);
            }
        }

        public static void ShowConfig()
        {
            var frmConfig = new ConfigForm();
            Application.EnableVisualStyles();
            Application.Run(frmConfig);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log(e.Exception);
            MessageBox.Show("There was an error running Mamesaver. Please see your error log for more details", "Mamesaver error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        /// <summary>
        /// Write a message to the event log
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if ( !EventLog.SourceExists("Mamesaver") )
                EventLog.CreateEventSource("Mamesaver", "Application");

            EventLog.WriteEntry("Mamesaver", message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Write an exceptions details to the event log
        /// </summary>
        /// <param name="exception"></param>
        public static void Log(Exception exception)
        {
            if (!EventLog.SourceExists("Mamesaver"))
                EventLog.CreateEventSource("Mamesaver", "Application");

            StringBuilder log = new StringBuilder();

            log.AppendLine("Exception Message:");
            log.AppendLine(exception.Message);
            log.AppendLine("");

            log.AppendLine("Stack Trace:");
            log.AppendLine(exception.StackTrace);
            log.AppendLine("");

            Exception e = exception;
            int depth = 1;
            while ( e.InnerException != null )
            {
                log.AppendFormat("Inner Message {0}:\n", depth);
                log.AppendLine(e.InnerException.Message);
                log.AppendLine("");
                
                e = e.InnerException;
                depth++;
            }

            EventLog.WriteEntry("Mamesaver", log.ToString(), EventLogEntryType.Error);
        }
    }
}