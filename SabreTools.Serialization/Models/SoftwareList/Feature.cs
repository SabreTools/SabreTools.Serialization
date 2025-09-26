using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("feature")]
    public class Feature
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("value")]
        public string? Value { get; set; }
    }
}