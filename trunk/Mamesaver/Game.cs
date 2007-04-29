/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Mamesaver
{
    [Serializable, XmlRoot("game")]
    public class Game
    {
        protected string name;
        protected string description;
        protected string year;
        protected string manufacturer;

        public Game()
        {
        }

        public Game(string name, string description, string year, string manufacturer)
        {
            this.name = name;
            this.description = description;
            this.year = year;
            this.manufacturer = manufacturer;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlAttribute("description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [XmlAttribute("year")]
        public string Year
        {
            get { return year; }
            set { year = value; }
        }

        [XmlAttribute("manufacturer")]
        public string Manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }
    }
}
