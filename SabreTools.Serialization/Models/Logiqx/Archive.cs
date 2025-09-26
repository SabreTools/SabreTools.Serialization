using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("archive")]
    public class Archive
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}