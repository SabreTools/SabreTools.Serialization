using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("softwarelist")]
    public class SoftwareList
    {
        [Required]
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <remarks>(original|compatible)</remarks>
        [Required]
        [XmlAttribute("status")]
        public SoftwareListStatus? Status { get; set; }

        [XmlAttribute("filter")]
        public string? Filter { get; set; }
    }
}
