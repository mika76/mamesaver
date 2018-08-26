using System.Xml.Serialization;

namespace Mamesaver.Configuration.Models
{
    /// <summary>
    ///     Configuration for game titles displayed while MAME is running.
    /// </summary>
    public class InGameTitles
    {
        /// <summary>
        ///     Initialise settings with defaults.
        /// </summary>
        public InGameTitles()
        {
            Enabled = true;
            FontSettings = new FontSettings
            {
                Face = "Arial",
                Size = 13
            };
        }

        [XmlElement("fontSettings")]
        public FontSettings FontSettings { get; set; }

        [XmlElement("enabled")]
        public bool Enabled { get; set; }
    }
}