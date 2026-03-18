using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.SoftwareList
{
    [XmlRoot("diskarea")]
    public class DiskArea
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }
    }
}
