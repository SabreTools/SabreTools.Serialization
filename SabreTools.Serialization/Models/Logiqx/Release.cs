using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("release")]
    public class Release
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("region")]
        public string? Region { get; set; }

        [XmlAttribute("language")]
        public string? Language { get; set; }

        [XmlAttribute("date")]
        public string? Date { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("default")]
        public string? Default { get; set; }
    }
}