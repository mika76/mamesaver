/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Xml.Serialization;

namespace Mamesaver
{
    [Serializable, XmlRoot("game")]
    public class Game
    {
        public Game()
        {
        }

        public Game(string name, string description, string year, string manufacturer, string rotation)
        {
            Name = name;
            Description = description;
            Year = year;
            Manufacturer = manufacturer;
            Rotation = rotation;
        }

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
    }
}