using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("biosset")]
    public class BiosSet
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("description")]
        public string? Description { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("default")]
        public bool? Default { get; set; }
    }
}
