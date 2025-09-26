using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("port")]
    public class Port
    {
        [SabreTools.Models.Required]
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [XmlElement("analog")]
        public Analog[]? Analog { get; set; }
    }
}