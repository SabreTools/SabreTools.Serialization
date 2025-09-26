using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.SoftwareList
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