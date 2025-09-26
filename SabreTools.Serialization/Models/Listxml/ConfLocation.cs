using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("conflocation")]
    public class ConfLocation
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <remarks>Numeric?</remarks>
        [Required]
        [XmlAttribute("number")]
        public string? Number { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("inverted")]
        public string? Inverted { get; set; }
    }
}