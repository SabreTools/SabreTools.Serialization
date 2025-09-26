using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("extension")]
    public class Extension
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}