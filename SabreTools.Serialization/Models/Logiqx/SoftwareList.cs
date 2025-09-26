using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
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
        public string? Status { get; set; }

        [XmlAttribute("filter")]
        public string? Filter { get; set; }
    }
}