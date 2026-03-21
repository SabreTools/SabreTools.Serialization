using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("sample")]
    public class Sample
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}
