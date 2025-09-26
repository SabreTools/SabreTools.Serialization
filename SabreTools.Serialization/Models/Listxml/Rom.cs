using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("rom")]
    public class Rom
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("bios")]
        public string? Bios { get; set; }

        /// <remarks>Numeric</remarks>
        [Required]
        [XmlAttribute("size")]
        public string? Size { get; set; }

        [XmlAttribute("crc")]
        public string? CRC { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        [XmlAttribute("region")]
        public string? Region { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("offset")]
        public string? Offset { get; set; }

        /// <remarks>(baddump|nodump|good) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("optional")]
        public string? Optional { get; set; }

        /// <remarks>(yes|no) "no", Only present in older versions</remarks>
        [XmlAttribute("dispose")]
        public string? Dispose { get; set; }

        /// <remarks>(yes|no) "no", Only present in older versions</remarks>
        [XmlAttribute("soundonly")]
        public string? SoundOnly { get; set; }
    }
}