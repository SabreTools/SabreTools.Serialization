using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("analog")]
    public class Analog
    {
        [Required]
        [XmlAttribute("mask")]
        public string? Mask { get; set; }
    }
}