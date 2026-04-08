using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("disk")]
    public class Disk
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        [XmlAttribute("status")]
        public ItemStatus? Status { get; set; }

        /// <remarks>MAME extension</remarks>
        [XmlAttribute("region")]
        public string? Region { get; set; }
    }
}
