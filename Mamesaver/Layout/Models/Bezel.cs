using System.Xml.Serialization;

namespace Mamesaver.Layout.Models
{
    public class Bezel
    {
        [XmlAttribute("element")]
        public string Element { get; set; }

        [XmlElement("bounds")]
        public Bounds Bounds { get; set; }
    }
}