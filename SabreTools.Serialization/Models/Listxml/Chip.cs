using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("chip")]
    public class Chip
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        /// <remarks>(cpu|audio)</remarks>
        [SabreTools.Models.Required]
        [XmlAttribute("type")]
        public string? Type { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("soundonly")]
        public string? SoundOnly { get; set; }

        [XmlAttribute("clock")]
        public string? Clock { get; set; }
    }
}