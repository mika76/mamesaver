using System;
using System.Xml.Serialization;

namespace Mamesaver.Models
{
    [Serializable, XmlRoot("SelectableGame")]
    public class SelectableGame : Game
    {
        [XmlAttribute("selected")]
        public bool Selected { get; set; }
    }
}