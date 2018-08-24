using System.Xml.Serialization;

namespace Mamesaver.Models.Settings
{
    public class InGameTitles
    {
        [XmlElement("fontSettings")]
        public FontSettings FontSettings { get; set; }

        [XmlElement("enabled")]
        public bool Enabled { get; set; }

        public InGameTitles()
        {
            Enabled = true;
            FontSettings = new FontSettings
            {
                Face = "Arial",
                Size = 13
            };
        }
    }
}