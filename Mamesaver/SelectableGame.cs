/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Xml.Serialization;

namespace Mamesaver
{
    [Serializable, XmlRoot("SelectableGame")]
    public class SelectableGame : Game
    {
        public SelectableGame()
        {
        }

        public SelectableGame(string name, string description, string year, string manufacturer, string rotation, bool selected)
            : base(name, description, year, manufacturer, rotation)
        {
            Selected = selected;
        }

        [XmlAttribute("selected")]
        public bool Selected { get; set; }
    }
}
