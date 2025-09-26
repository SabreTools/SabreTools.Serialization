using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("sound")]
    public class Sound
    {
        /// <remarks>Numeric</remarks>
        [Required]
        [XmlAttribute("channels")]
        public string? Channels { get; set; }
    }
}