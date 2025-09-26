using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("image")]
    public class Image
    {
        [XmlAttribute("x")]
        public string? X { get; set; }

        [XmlAttribute("y")]
        public string? Y { get; set; }

        [XmlAttribute("width")]
        public string? Width { get; set; }

        [XmlAttribute("height")]
        public string? Height { get; set; }
    }
}