using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("extension")]
    public class Extension
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}