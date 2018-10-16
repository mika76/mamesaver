using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mamesaver.Models.Configuration;

namespace Mamesaver.Services.Mame
{
   public class MamePathManager
    {
        /// <summary>
        ///     String which is surrounded by double quotes
        /// </summary>
        private readonly Regex _quotedString = new Regex(@"^\""(.*)\""$");

        private readonly Settings _settings;

        public MamePathManager(Settings settings) => _settings = settings;

        /// <summary>
        ///     Parses a MAME configuration line into a collection of directories.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="line">Entire configuration line to parse</param>
        /// <returns>Absolute directories extracted from <see cref="line"/>, or <c>null</c> if no match</returns>
        public List<string> ExtractConfigPaths(string key, string line)
        {
            // Skip lines which don't match the configuration key we are after
            if (!Regex.IsMatch(line, ConfigurationLineRegex(key))) return null;

            // Extract path string from configuration line
            var directoryRegex = new Regex(ConfigurationLineRegex(key));
            var match = directoryRegex.Match(line);

            // Remove any speech marks around entire path collection and split into individual paths
            var rawPaths = CleansePath(match.Groups[1].Value);
            var paths = rawPaths.Split(';');

            // Construct list of absolute paths
            var absolutePaths = new List<string>();
            foreach (var path in paths.Select(CleansePath))
            {
                // If path is absolute, add raw path
                if (Path.IsPathRooted(path)) absolutePaths.Add(path);
                else
                {
                    // If path is relative, construct absolute path relative to Mame executable
                    var execPath = _settings.ExecutablePath;
                    var workingDirectory = Directory.GetParent(execPath).ToString();

                    absolutePaths.Add(Path.Combine(workingDirectory, path));
                }
            }

            return absolutePaths;
        }

        /// <summary>
        ///     Cleanses a single path or list of paths, trimming whitespace and removing surrounding speech marks. 
        /// </summary>
        public string CleansePath(string path)
        {
            path = path.Trim();

            // Remove surrounding speech marks from path string. These are added by MAME if spaces are 
            // present in any part of the path .
            return _quotedString.IsMatch(path) ? _quotedString.Match(path).Groups[1].Value : path;
        }

        /// <summary>
        ///    Returns a regular expression matching a MAME configuration line.
        /// </summary>
        /// <param name="key">MAME configuration key to patch</param>
        /// <returns>Regex string</returns>
        private static string ConfigurationLineRegex(string key) => $@"{key}\s+(.+)";
    }
}