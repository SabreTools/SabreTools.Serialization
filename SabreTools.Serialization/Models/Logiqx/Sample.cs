using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("sample")]
    public class Sample
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}