using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("slotoption")]
    public class SlotOption
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("devname")]
        public string? DevName { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("default")]
        public string? Default { get; set; }
    }
}