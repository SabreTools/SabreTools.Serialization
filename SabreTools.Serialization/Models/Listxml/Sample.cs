using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("sample")]
    public class Sample
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}