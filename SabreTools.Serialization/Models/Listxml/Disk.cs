using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("disk")]
    public class Disk
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        [XmlAttribute("region")]
        public string? Region { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("index")]
        public string? Index { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("writable")]
        public string? Writable { get; set; }

        /// <remarks>(baddump|nodump|good) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("optional")]
        public string? Optional { get; set; }
    }
}