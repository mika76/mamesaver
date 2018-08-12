/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Mamesaver
{
    public class Mamesaver
    {
        private List<MameScreen> _mameScreens = new List<MameScreen>();

        public void ShowConfig()
        {
            ConfigForm frmConfig = new ConfigForm(this);
            Application.EnableVisualStyles();
            Application.Run(frmConfig);
        }

        public void Run()
        {
            try
            {
                // Load list and get only selected games from it
                var gameListFull = Settings.LoadGameList();
                var gameList = new List<Game>();

                if (gameListFull.Count == 0) return;

                foreach (var game in gameListFull)
                    if (game.Selected) gameList.Add(game);

                // Exit run method if there were no selected games
                if (gameList.Count == 0) return;

                // Set up the background form
                Cursor.Hide();

                foreach (var screen in Screen.AllScreens)
                {
                    var mameScreen = new MameScreen(screen, gameList, OnScreenClosed, true);
                    _mameScreens.Add(mameScreen);
                    mameScreen.Initialise();
                }

                // Run the application
                Application.EnableVisualStyles();
                var allForms = _mameScreens.Select(s => s.FrmBackground).OfType<Form>().ToList();
                Application.Run(new MultiFormApplicationContext(allForms));
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message, "Error",  MessageBoxButtons.OK , MessageBoxIcon.Error);
            }
        }

        private void OnScreenClosed(MameScreen mameScreen)
        {
            // one screen has closed so close them all
            foreach (var screen in new List<MameScreen>(_mameScreens))
            {
                _mameScreens.Remove(screen);
                screen.Close();
            }
            Application.Exit();
        }

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="SelectableGame"/>s which are read from
        /// the full list and then merged with the verified rom's list. The games which are returned
        /// all have a "good" status on their drivers. This check also eliminates BIOS ROMS.
        /// </summary>
        /// <returns>Returns a <see cref="List{T}"/> of <see cref="SelectableGame"/>s</returns>
        public List<SelectableGame> GetGameList()
        {
            // Retrieve identifiers of verified games
            var verifiedGames = GetVerifiedSets();

            var games = new List<SelectableGame>();

            // Enrich game metadata
            using (var stream = GetFullGameList())
            {
                var readerSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
                using (var reader = XmlReader.Create(stream, readerSettings))
                {
                    reader.ReadStartElement("mame");

                    // Read each machine, enriching metadata for verified sets
                    while(reader.Read() && reader.Name == "machine")
                    { 
                        // Read game metadata
                        var machine = (XElement) XNode.ReadFrom(reader);

                        var name = machine.Attribute("name")?.Value;
                        if (name == null) continue;
                        
                        // Skip games which aren't verified
                        if (!verifiedGames.Contains(name)) continue;

                        var driver = machine.Element("driver");
                        if (driver == null) continue;

                        // Skip games which aren't fully emulated
                        var status = driver.Attribute("status")?.Value;
                        if (status != "good") continue;

                        var year = machine.Element("year")?.Value ?? "";
                        var manufacturer = machine.Element("manufacturer")?.Value ?? "";
                        var description = machine.Element("description")?.Value ?? "";

                        games.Add(new SelectableGame(name, description, year, manufacturer, false));
                    } 
                }
            }

            return games;
        }
        
        /// <summary>
        /// Gets the full XML game list from <a href="http://www.mame.org/">Mame</a>.
        /// </summary>
        /// <returns><see cref="String"/> holding the Mame XML</returns>
        private StreamReader GetFullGameList()
        {
            string execPath = Settings.ExecutablePath;
            ProcessStartInfo psi = new ProcessStartInfo(execPath);
            psi.Arguments = "-listxml";
            psi.WorkingDirectory = Directory.GetParent(execPath).ToString();
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(psi);

            return p.StandardOutput;

        }

        /// <summary>
        /// Returns a <see cref="Hashtable"/> filled with the names of games which are
        /// verified to work. Only the ones marked as good are returned. The clone names
        /// are returned in the value of the hashtable while the name is used as the key.
        /// </summary>
        /// <returns><see cref="Hashtable"/></returns>
        private Hashtable GetVerifiedSets()
        {
            string execPath = Settings.ExecutablePath;
            ProcessStartInfo psi = new ProcessStartInfo(execPath);
            psi.Arguments = "-verifyroms";
            psi.WorkingDirectory = Directory.GetParent(execPath).ToString();
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            Regex r = new Regex(@"romset (\w*)(?:\s\[(\w*)\])? is good"); //only accept the "good" ROMS
            MatchCollection matches = r.Matches(output);
            Hashtable verifiedGames = new Hashtable();

            foreach (Match m in matches)
            {
                verifiedGames.Add(m.Groups[1].Value, m.Groups[2].Value);
            }

            return verifiedGames;
        }
    }
}
