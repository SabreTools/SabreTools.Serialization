using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("part")]
    public class Part
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [SabreTools.Models.Required]
        [XmlAttribute("interface")]
        public string? Interface { get; set; }

        [XmlElement("feature")]
        public Feature[]? Feature { get; set; }

        [XmlElement("dataarea")]
        public DataArea[]? DataArea { get; set; }

        [XmlElement("diskarea")]
        public DiskArea[]? DiskArea { get; set; }

        [XmlElement("dipswitch")]
        public DipSwitch[]? DipSwitch { get; set; }
    }
}