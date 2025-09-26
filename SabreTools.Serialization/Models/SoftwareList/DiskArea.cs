using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("diskarea")]
    public class DiskArea
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }
    }
}