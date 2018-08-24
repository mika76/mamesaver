using System;
using System.Xml.Serialization;

namespace Mamesaver
{
    [Serializable, XmlRoot("LayoutOptions")]
    public class LayoutOptions
    {
        [XmlElement("fontFace")]
        public string FontFace { get; set; } = "Arial";

        [XmlElement("fontSize")]
        public int FontSize { get; set; } = 15;

        [XmlElement("classicGameInfo")]
        public bool ClassicGameInfo { get; set; }

        [XmlElement("showGameinfo")]
        public bool ShowGameInfo { get; set; } = true;
    }
}
