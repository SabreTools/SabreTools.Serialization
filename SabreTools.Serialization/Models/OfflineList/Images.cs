using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("images")]
    public class Images
    {
        [XmlAttribute("width")]
        public string? Width { get; set; }

        [XmlAttribute("height")]
        public string? Height { get; set; }

        [XmlElement("image")]
        public Image[]? Image { get; set; }
    }
}