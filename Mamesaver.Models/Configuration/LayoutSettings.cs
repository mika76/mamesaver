using System.Xml.Serialization;

namespace Mamesaver.Models.Configuration
{
    public class LayoutSettings
    {
        /// <summary>
        ///     Initialise settings with defaults.
        /// </summary>
        public LayoutSettings()
        {
            SplashScreen = new SplashScreen();
            InGameTitles = new InGameTitles();
        }

        [XmlElement("splashScreen")]
        public SplashScreen SplashScreen { get; set; }

        [XmlElement("inGameTitles")]
        public InGameTitles InGameTitles { get; set; }
    }
}