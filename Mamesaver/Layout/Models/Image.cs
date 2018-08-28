using System.Xml.Serialization;

namespace Mamesaver.Layout.Models
{
    public class Image
    {
        [XmlAttribute("file")]
        public string File { get; set; }
    }
}
