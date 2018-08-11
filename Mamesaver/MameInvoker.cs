using System;
using System.Diagnostics;
using System.IO;

namespace Mamesaver
{
    internal static class MameInvoker
    {
        /// <summary>
        ///     Invokes Mame, returning the created process
        /// </summary>
        /// <param name="arguments">arguments to pass to Mame</param>
        public static Process Run(params string[] arguments)
        {
            var execPath = Settings.ExecutablePath;
            var psi = new ProcessStartInfo(execPath)
            {
                Arguments = string.Join(" ", arguments),
                WorkingDirectory = Directory.GetParent(execPath).ToString(),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var p = Process.Start(psi);
            if (p == null) throw new InvalidOperationException($"Mame process not created: {psi.FileName} {psi.Arguments}");

            return p;
        }

        /// <summary>
        ///     Invokes Mame, returning the standard output stream
        /// </summary>
        /// <param name="arguments">raguments to pass to Mame</param>
        public static StreamReader GetOutput(params string[] arguments)
        {
            return Run(arguments).StandardOutput;
        }
    }
}
