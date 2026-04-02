using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("device")]
    public class Device
    {
        /// <remarks>(unknown|cartridge|floppydisk|harddisk|cylinder|cassette|punchcard|punchtape|printout|serial|parallel|snapshot|quickload|memcard|cdrom|magtape|romimage|midiin|midiout|picture|vidfile)</remarks>
        [Required]
        [XmlAttribute("type")]
        public DeviceType? Type { get; set; }

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
