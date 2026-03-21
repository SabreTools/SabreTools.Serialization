using System.Xml;
using System.Xml.Serialization;

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
