using System.Xml.Serialization;

namespace Mamesaver.Models.Settings
{
    public class LayoutSettings
    {
        [XmlElement("splashScreen")]
        public SplashScreen SplashScreen { get; set; }

        [XmlElement("inGameTitles")]
        public InGameTitles InGameTitles { get; set; }

        public LayoutSettings()
        {
            SplashScreen = new SplashScreen();
            InGameTitles = new InGameTitles();
        }
    }
}