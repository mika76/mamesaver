using System.Xml.Serialization;

namespace Mamesaver.Layout.Models
{
    public class Element
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("image")]
        public Image Image { get; set; }
    }
}
