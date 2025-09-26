using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("ramoption")]
    public class RamOption
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("default")]
        public string? Default { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}