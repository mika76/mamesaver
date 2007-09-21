/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Diagnostics;

namespace Mamesaver
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                string[] arguments = new string[] {"/c"};

                if (args.Length != 0) //default to config if no options passed
                    arguments = args;

                Log("Mamesaver started with args " + string.Join(",", args));

                Mamesaver saver = new Mamesaver();

                switch (arguments[0].Trim().Substring(0, 2).ToLower())
                {
                    case "/c":
                        //TODO: Catch display properties window handle and set it as parent
                        saver.ShowConfig();
                        break;

                    case "/s":
                        saver.Run();
                        break;

                    case "/p":
                        // do nothing
                        break;
                }
            }
            catch(Exception x)
            {
                Log(x);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject as Exception);
        }

        public static void Log(string message)
        {
            if ( !EventLog.SourceExists("Mamesaver") )
                EventLog.CreateEventSource("Mamesaver", "Application");

            EventLog.WriteEntry("Mamesaver", message, EventLogEntryType.Information);
        }

        public static void Log(Exception exception)
        {
            if (!EventLog.SourceExists("Mamesaver"))
                EventLog.CreateEventSource("Mamesaver", "Application");

            EventLog.WriteEntry("Mamesaver", exception.Message, EventLogEntryType.Error);
        }
    }
}