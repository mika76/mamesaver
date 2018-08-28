using System.Xml.Serialization;

namespace Mamesaver.Layout.Models
{
    [XmlRoot("mamelayout")]
    public class MameLayout
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement("element")]
        public Element Element { get; set; }

        [XmlElement("view")]
        public View View { get; set; }
    }
}
