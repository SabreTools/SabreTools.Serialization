using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.OpenMSX
{
    [XmlRoot("software")]
    public class Software
    {
        [Required]
        [XmlElement("title")]
        public string? Title { get; set; }

        [XmlElement("genmsxid")]
        public string? GenMSXID { get; set; }

        [Required]
        [XmlElement("system")]
        public string? System { get; set; }

        [Required]
        [XmlElement("company")]
        public string? Company { get; set; }

        [Required]
        [XmlElement("year")]
        public string? Year { get; set; }

        [Required]
        [XmlElement("country")]
        public string? Country { get; set; }

        [XmlElement("dump")]
        public Dump[]? Dump { get; set; }
    }
}