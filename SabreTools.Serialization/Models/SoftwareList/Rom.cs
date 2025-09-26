using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("rom")]
    public class Rom
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("size")]
        public string? Size { get; set; }

        [XmlAttribute("length")]
        public string? Length { get; set; }

        [XmlAttribute("crc")]
        public string? CRC { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("offset")]
        public string? Offset { get; set; }

        [XmlAttribute("value")]
        public string? Value { get; set; }

        /// <remarks>(baddump|nodump|good) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>(load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore)</remarks>
        [XmlAttribute("loadflag")]
        public string? LoadFlag { get; set; }
    }
}