using System;
using System.Diagnostics;
using System.IO;
using Mamesaver.Configuration.Models;
using Serilog;

namespace Mamesaver
{
    /// <summary>
    ///     Invookes the MAME executable configured in <see cref="Settings"/>.
    /// </summary>
    internal class MameInvoker
    {
        private readonly Settings _settings;

        public MameInvoker(Settings settings) => _settings = settings;

        /// <summary>
        ///     Invokes MAME, returning the created process
        /// </summary>
        /// <param name="arguments">arguments to pass to Mame</param>
        public Process Run(params string[] arguments)
        {
            Log.Information("Invoking MAME with arguments: {arguments}", string.Join(" ", arguments));

            var execPath = _settings.ExecutablePath;
            var psi = new ProcessStartInfo(execPath)
            {
                Arguments = string.Join(" ", arguments),
                WorkingDirectory = Directory.GetParent(execPath).ToString(),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                var process = Process.Start(psi);
                if (process == null) throw new InvalidOperationException($"MAME process not created: {psi.FileName} {psi.Arguments}");

                return process;
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to run MAME: {filename} {arguments}", psi.FileName, psi.Arguments);
                throw;
            }
        }

        /// <summary>
        ///     Invokes MAME, returning the standard output stream
        /// </summary>
        /// <param name="arguments">arguments to pass to MAME</param>
        public StreamReader GetOutput(params string[] arguments) => Run(arguments).StandardOutput;
    }
}