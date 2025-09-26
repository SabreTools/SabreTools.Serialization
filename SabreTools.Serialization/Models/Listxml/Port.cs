using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("port")]
    public class Port
    {
        [Required]
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [XmlElement("analog")]
        public Analog[]? Analog { get; set; }
    }
}