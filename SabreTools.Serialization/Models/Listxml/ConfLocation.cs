using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
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