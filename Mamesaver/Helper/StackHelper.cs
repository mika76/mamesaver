using System.Diagnostics;

namespace Mamesaver.Helper
{
    public static class StackHelper
    {
        /// <summary>
        ///     Logs a stack trace. This is useful during development and debugging to identify where a method
        ///     is being invoked from.
        /// </summary>
        public static void Log()
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            if (frames == null)
            {
                Serilog.Log.Warning("No stack trace");
                return;
            }

            foreach (var frame in frames)
            {
                Serilog.Log.Information("Filename: {filename} Method: {method} Line: {line} Column: {column}  ",
                    frame.GetFileName(), frame.GetMethod(), frame.GetFileLineNumber(),
                    frame.GetFileColumnNumber());
            }
        }
    }
}