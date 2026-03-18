using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.SoftwareList
{
    [XmlRoot("info")]
    public class Info
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("value")]
        public string? Value { get; set; }
    }
}
