using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("analog")]
    public class Analog
    {
        [Required]
        [XmlAttribute("mask")]
        public string? Mask { get; set; }
    }
}