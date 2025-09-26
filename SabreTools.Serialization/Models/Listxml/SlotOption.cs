using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
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