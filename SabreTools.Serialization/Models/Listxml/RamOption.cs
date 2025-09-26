using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("ramoption")]
    public class RamOption
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("default")]
        public string? Default { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}