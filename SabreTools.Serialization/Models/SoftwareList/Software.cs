using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("software")]
    public class Software
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("cloneof")]
        public string? CloneOf { get; set; }

        /// <remarks>(yes|partial|no) "yes"</remarks>
        [XmlAttribute("supported")]
        public string? Supported { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("description")]
        public string? Description { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("year")]
        public string? Year { get; set; }

        [SabreTools.Models.Required]
        [XmlElement("publisher")]
        public string? Publisher { get; set; }

        [XmlElement("notes")]
        public string? Notes { get; set; }

        [XmlElement("info")]
        public Info[]? Info { get; set; }

        [XmlElement("sharedfeat")]
        public SharedFeat[]? SharedFeat { get; set; }

        [XmlElement("part")]
        public Part[]? Part { get; set; }
    }
}