using System.Xml.Serialization;

namespace Mamesaver.Models.Configuration
{
    public class SplashScreen
    {
        /// <summary>
        ///     Initialise settings with defaults.
        /// </summary>
        public SplashScreen()
        {
            DurationSeconds = 3;
            Enabled = false;
            FontSettings = new FontSettings
            {
                Face = "Arial",
                Size = 18
            };
        }

        /// <summary>
        ///     The number of seconds that the game intro screen (the screen with MAME logo) is shown
        /// </summary>
        /// <remarks>
        ///     Even if the splash screen is disabled, the MAME logo is displayed while MAME starts
        /// </remarks>
        [XmlElement("durationSeconds")]
        public int DurationSeconds { get; set; }

        [XmlElement("fontSettings")]
        public FontSettings FontSettings { get; set; }

        /// <summary>
        ///     Whether the splash screen with game information is displayed before each game.
        /// </summary>
        [XmlElement("enabled")]
        public bool Enabled { get; set; }
    }
}