using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.SoftwareList
{
    [XmlRoot("sharedfeat")]
    public class SharedFeat
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("value")]
        public string? Value { get; set; }
    }
}
