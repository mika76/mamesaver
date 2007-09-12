/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;

namespace Mamesaver
{
    [Serializable, XmlRoot("SelectableGame")]
    public class SelectableGame : Game
    {
        private bool selected;

        public SelectableGame()
        {
        }

        public SelectableGame(string name, string description, string year, string manufacturer, bool selected)
        {
            this.name = name;
            this.description = description;
            this.year = year;
            this.manufacturer = manufacturer;
            this.selected = selected;
        }

        [XmlAttribute("selected")]
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
    }
}
