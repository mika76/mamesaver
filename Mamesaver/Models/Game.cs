using System;
using System.Xml.Serialization;

namespace Mamesaver.Models
{
    [Serializable, XmlRoot("game")]
    public class Game
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("year")]
        public string Year { get; set; }

        [XmlAttribute("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlAttribute("rotation")]
        public string Rotation { get; set; }

        [XmlAttribute("category")]
        public string Category { get; set; }

        [XmlAttribute("subcategory")]
        public string Subcategory { get; set; }
    }
}