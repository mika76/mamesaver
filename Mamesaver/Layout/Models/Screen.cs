using System.Xml.Serialization;

namespace Mamesaver.Layout.Models
{
    public class Screen
    {
        [XmlAttribute("index")]
        public int Index { get; set; }

        [XmlElement("bounds")]
        public Bounds Bounds { get; set; }
    }
}