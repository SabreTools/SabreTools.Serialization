using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.SoftwareList
{
    [XmlRoot("softwarelist")]
    public class SoftwareList
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("description")]
        public string? Description { get; set; }

        [XmlElement("notes")]
        public string? Notes { get; set; }

        [XmlElement("software")]
        public Software[]? Software { get; set; }
    }
}