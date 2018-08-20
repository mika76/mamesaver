using System.Xml.Serialization;

namespace Mamesaver.Layout.Models
{
    public class View
    {
        [XmlElement("screen")]
        public Screen Screen { get; set; }

        [XmlElement("bezel")]
        public Bezel Bezel { get; set; }
    }
}
