using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("dipswitch")]
    public class DipSwitch
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [XmlAttribute("mask")]
        public string? Mask { get; set; }

        [XmlElement("dipvalue")]
        public DipValue[]? DipValue { get; set; }
    }
}