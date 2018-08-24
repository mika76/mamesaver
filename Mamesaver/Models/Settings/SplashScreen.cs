using System.Xml.Serialization;

namespace Mamesaver.Models.Settings
{
    public class SplashScreen
    {
        /// <summary>
        ///     The number of seconds that the game intro screen (the screen with the big MAME logo) is shown.
        /// </summary>
        [XmlElement("durationSeconds")]
        public int DurationSeconds { get; set; }

        [XmlElement("fontSettings")]
        public FontSettings FontSettings { get; set; }

        /// <summary>
        ///     Whether the splash screen is displayed before each game
        /// </summary>
        [XmlElement("enabled")]
        public bool Enabled { get; set; }

        public SplashScreen()
        {
            DurationSeconds = 3;
            Enabled = false;
            FontSettings = new FontSettings
            {
                Face = "Arial",
                Size = 20
            };
        }
    }
}