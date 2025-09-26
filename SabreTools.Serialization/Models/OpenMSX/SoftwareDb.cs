using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OpenMSX
{
    [XmlRoot("softwaredb")]
    public class SoftwareDb
    {
        [XmlAttribute("timestamp")]
        public string? Timestamp { get; set; }

        [XmlElement("software")]
        public Software[]? Software { get; set; }
    }
}