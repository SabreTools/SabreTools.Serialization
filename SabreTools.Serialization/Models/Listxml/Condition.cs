using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("condition")]
    public class Condition
    {
        [SabreTools.Models.Required]
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [SabreTools.Models.Required]
        [XmlAttribute("mask")]
        public string? Mask { get; set; }

        /// <remarks>(eq|ne|gt|le|lt|ge)</remarks>
        [SabreTools.Models.Required]
        [XmlAttribute("relation")]
        public string? Relation { get; set; }

        [SabreTools.Models.Required]
        [XmlAttribute("value")]
        public string? Value { get; set; }
    }
}