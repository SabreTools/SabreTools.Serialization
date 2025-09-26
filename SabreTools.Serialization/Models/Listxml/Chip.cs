using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("chip")]
    public class Chip
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        /// <remarks>(cpu|audio)</remarks>
        [Required]
        [XmlAttribute("type")]
        public string? Type { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("soundonly")]
        public string? SoundOnly { get; set; }

        [XmlAttribute("clock")]
        public string? Clock { get; set; }
    }
}