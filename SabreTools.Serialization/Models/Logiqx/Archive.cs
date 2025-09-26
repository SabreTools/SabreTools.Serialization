using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("archive")]
    public class Archive
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}