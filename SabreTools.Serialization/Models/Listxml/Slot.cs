using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("slot")]
    public class Slot
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlElement("slotoption")]
        public SlotOption[]? SlotOption { get; set; }
    }
}