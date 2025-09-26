using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("instance")]
    public class Instance
    {
        [SabreTools.Models.Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [SabreTools.Models.Required]
        [XmlAttribute("briefname")]
        public string? BriefName { get; set; }
    }
}