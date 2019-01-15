using System;
using System.Xml.Serialization;

namespace Mamesaver.Models.Configuration
{
    [Serializable]
    [XmlRoot("Settings")]
    public class Settings
    {
        /// <summary>
        ///     Initialise settings with defaults.
        /// </summary>
        public Settings()
        {
            ExecutablePath = @"C:\MAME\MAME64.EXE";
            CommandLineOptions = "-skip_gameinfo -nowindow -noswitchres -sleep -triplebuffer -sound none";
            MinutesPerGame = 5;
            CloneScreen = true;
            HotKeys = true;
            LayoutSettings = new LayoutSettings();
            AdvancedSettings = new AdvancedSettings();
        }

        /// <summary>
        ///     Path to the MAME executable file, including the filename and extension. eg: <c>C:\MAME\MAME64.EXE</c>
        /// </summary>
        [XmlElement("executablePath")]
        public string ExecutablePath { get; set; }

        /// <summary>
        ///     Time to run each game.
        /// </summary>
        [XmlElement("minutesPerGame")]
        public int MinutesPerGame { get; set; }

        /// <summary>
        ///     Options to send to the command line when MAME runs the game.
        /// </summary>
        [XmlElement("commandLineOptions")]
        public string CommandLineOptions { get; set; }

        /// <summary>
        ///     Whether the game should be displayed on all screens.
        /// </summary>
        [XmlElement("cloneScreen")]
        public bool CloneScreen { get; set; }

        /// <summary>
        ///     Whether hot keys should be enabled to interact with screensaver.
        /// </summary>
        [XmlElement("hotKeys")]
        public bool HotKeys { get; set; } 

        [XmlElement("layoutSettings")]
        public LayoutSettings LayoutSettings { get; set; }

        [XmlElement("AdvancedSettings")]
        public AdvancedSettings AdvancedSettings { get; set; }
    }
}