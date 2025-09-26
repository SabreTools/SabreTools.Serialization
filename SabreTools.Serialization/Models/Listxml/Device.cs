using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("device")]
    public class Device
    {
        [SabreTools.Models.Required]
        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [XmlAttribute("fixed_image")]
        public string? FixedImage { get; set; }

        /// <remarks>Numeric boolean</remarks>
        [XmlAttribute("mandatory")]
        public string? Mandatory { get; set; }

        [XmlAttribute("interface")]
        public string? Interface { get; set; }

        [XmlElement("instance")]
        public Instance? Instance { get; set; }

        [XmlElement("extension")]
        public Extension[]? Extension { get; set; }
    }
}