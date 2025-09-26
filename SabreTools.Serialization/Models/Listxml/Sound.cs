using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("sound")]
    public class Sound
    {
        /// <remarks>Numeric</remarks>
        [SabreTools.Models.Required]
        [XmlAttribute("channels")]
        public string? Channels { get; set; }
    }
}