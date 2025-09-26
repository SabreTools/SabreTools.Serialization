using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OpenMSX
{
    [XmlRoot("software")]
    public class Software
    {
        [SabreTools.Models.Required]
        [XmlElement("title")]
        public string? Title { get; set; }

        [XmlElement("genmsxid")]
        public string? GenMSXID { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("system")]
        public string? System { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("company")]
        public string? Company { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("year")]
        public string? Year { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("country")]
        public string? Country { get; set; }

        [XmlElement("dump")]
        public Dump[]? Dump { get; set; }
    }
}