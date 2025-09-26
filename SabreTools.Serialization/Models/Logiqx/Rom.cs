using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("rom")]
    public class Rom
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("size")]
        public string? Size { get; set; }

        [XmlAttribute("crc")]
        public string? CRC { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("md2")]
        public string? MD2 { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("md4")]
        public string? MD4 { get; set; }

        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("ripemd128")]
        public string? RIPEMD128 { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("ripemd160")]
        public string? RIPEMD160 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        /// <remarks>Hash/No-Intro extension</remarks>
        [XmlAttribute("sha256")]
        public string? SHA256 { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("sha384")]
        public string? SHA384 { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("sha512")]
        public string? SHA512 { get; set; }

        /// <remarks>Hash extension</remarks>
        [XmlAttribute("spamsum")]
        public string? SpamSum { get; set; }

        /// <remarks>DiscImgeCreator extension</remarks>
        [XmlAttribute("xxh3_64")]
        public string? xxHash364 { get; set; }

        /// <remarks>DiscImgeCreator extension</remarks>
        [XmlAttribute("xxh3_128")]
        public string? xxHash3128 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>No-Intro extension</remarks>
        [XmlAttribute("serial")]
        public string? Serial { get; set; }

        /// <remarks>No-Intro extension</remarks>
        [XmlAttribute("header")]
        public string? Header { get; set; }

        [XmlAttribute("date")]
        public string? Date { get; set; }

        /// <remarks>Boolean; RomVault extension</remarks>
        [XmlAttribute("inverted")]
        public string? Inverted { get; set; }

        /// <remarks>Boolean; RomVault extension</remarks>
        [XmlAttribute("mia")]
        public string? MIA { get; set; }
    }
}