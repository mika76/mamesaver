using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Mamesaver
{
    internal static class GameListBuilder
    {
        /// <summary>
        ///     Number of roms to process per batch. Rom files are batched when passing as arguments to Mame as a precaution
        ///     in case we have a very large number of roms and end up with command line length limit issues.
        /// </summary>
        private const int RomsPerBatch = 50;

        /// <summary>
        ///     Settings for parsing Mame's listxml output
        /// </summary>
        private static readonly XmlReaderSettings ReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="SelectableGame"/>s which are read from
        /// the full list and then merged with the verified rom's list. The games which are returned
        /// all have a "good" status on their drivers. This check also eliminates BIOS ROMS.
        /// </summary>
        /// <returns>Returns a <see cref="List{T}"/> of <see cref="SelectableGame"/>s</returns>
        public static List<SelectableGame> GetGameList()
        {
            var games = new List<SelectableGame>();

            // Enrich game metadata for each verified game
            var verifiedGames = GetVerifiedSets().Keys;

            // Get details for each verified rom
            var index = 0;
            List<string> romsToProcess;
            while ((romsToProcess = verifiedGames.Skip(index).Take(RomsPerBatch).ToList()).Any())
            {
                using (var stream = GetGameDetails(romsToProcess))
                {
                    games.AddRange(GetRomDetails(stream));
                }

                index += RomsPerBatch;
            }

            return games;
        }

        /// <summary>
        ///     Extracts rom metadata for display from a XML stream from Mame
        /// </summary>
        private static List<SelectableGame> GetRomDetails(StreamReader stream)
        {
            var games = new List<SelectableGame>();

            using (var reader = XmlReader.Create(stream, ReaderSettings))
            {
                reader.ReadStartElement("mame");

                // Read each machine, enriching metadata for verified sets
                while (reader.Read() && reader.Name == "machine")
                {
                    // Read game metadata
                    var element = (XElement)XNode.ReadFrom(reader);

                    var name = element.Attribute("name")?.Value;
                    if (name == null) continue;

                    var driver = element.Element("driver");
                    if (driver == null) continue;

                    // Skip games which aren't fully emulated
                    var status = driver.Attribute("status")?.Value;
                    if (status != "good") continue;

                    var year = element.Element("year")?.Value ?? "";
                    var manufacturer = element.Element("manufacturer")?.Value ?? "";
                    var description = element.Element("description")?.Value ?? "";

                    games.Add(new SelectableGame(name, description, year, manufacturer, false));
                }
            }

            return games;
        }

        /// <summary>
        ///     Returns a <see cref="IDictionary{TKey,TValue}" /> filled with the names of games which are
        ///     verified to work. Only the ones marked as good are returned. The clone names
        ///     are returned in the value of the dictionary while the name is used as the key.
        /// </summary>
        private static IDictionary<string, string> GetVerifiedSets()
        {
            var verifiedRoms = new Dictionary<string, string>();
            var regex = new Regex(@"romset (\w*)(?:\s\[(\w*)\])? is good"); //only accept the "good" ROMS

            var romFiles = GetRomFiles();

            // Verify each rom in directory
            var index = 0;
            List<string> filesToVerify;
            while ((filesToVerify = romFiles.Skip(index).Take(RomsPerBatch).ToList()).Any())
            {
                var arguments = new List<string> { "-verifyroms" }.Concat(filesToVerify).ToArray();
                using (var stream = MameInvoker.GetOutput(arguments))
                {
                    var output = stream.ReadToEnd();

                    var matches = regex.Matches(output);
                    for (var i = 0; i < matches.Count; i++)
                    {
                        var match = matches[i];
                        verifiedRoms[match.Groups[1].Value] = match.Groups[2].Value;
                    }
                }

                index += RomsPerBatch;
            }

            return verifiedRoms;
        }

        /// <summary>
        ///     Returns a list of the base name of roms in the Mame rom directories.
        /// </summary>
        /// <remarks>
        ///     It is assumed that roms are zipped.
        /// </remarks>
        private static List<string> GetRomFiles()
        {
            var roms = new List<string>();

            foreach (var path in GetRomPaths())
            {
                var romFiles = Directory.GetFiles(path, "*.zip");
                roms.AddRange(romFiles.Select(Path.GetFileNameWithoutExtension));
            }

            return roms;
        }

        /// <summary>
        ///     Retrieves absolute paths to the rom directories as configured by Mame.
        /// </summary>
        /// <remarks>
        ///     Multiple rom directories can be specified in Mame by separating directories by semicolons.
        /// </remarks>
        private static IEnumerable<string> GetRomPaths()
        {
            // Configuration in the Mame ini file which indicates path to roms
            var regex = new Regex(@"rompath\s+(.*)");

            using (var stream = MameInvoker.GetOutput("-showconfig"))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    if (!regex.IsMatch(line)) continue;

                    var match = regex.Match(line);
                    var paths = match.Groups[1].Value.Split(';');

                    // Construct list of absolute paths
                    var absolutePaths = new List<string>();
                    foreach (var path in paths.Select(p => p.Trim()))
                    {
                        // If path is absolute, add raw path
                        if (Path.IsPathRooted(path))
                        {
                            absolutePaths.Add(path);
                        }
                        else
                        {
                            // If path is relative, construct absolute path relative to Mame executable
                            var execPath = Settings.ExecutablePath;
                            var workingDirectory = Directory.GetParent(execPath).ToString();

                            absolutePaths.Add(Path.Combine(workingDirectory, path));
                        }
                    }

                    return absolutePaths;
                }
            }

            throw new InvalidOperationException("Unable to retrieve rom paths");
        }

        /// <summary>
        ///     Gets XML details for a list of games from <a href="http://www.mame.org/">Mame</a>.
        /// </summary>
        /// <param name="games">games to retrieve details for</param>
        /// <returns><see cref="StreamReader" /> containing stream of Mame XML</returns>
        private static StreamReader GetGameDetails(List<string> games)
        {
            return MameInvoker.GetOutput(new List<string> { "-listxml" }.Concat(games).ToArray());
        }
    }
}