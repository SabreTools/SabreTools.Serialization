using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("sample")]
    public class Sample
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}