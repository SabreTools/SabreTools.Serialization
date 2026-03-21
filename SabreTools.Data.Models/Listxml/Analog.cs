using System.Xml;
using System.Xml.Serialization;

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
