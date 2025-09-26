using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("condition")]
    public class Condition
    {
        [Required]
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        [Required]
        [XmlAttribute("mask")]
        public string? Mask { get; set; }

        /// <remarks>(eq|ne|gt|le|lt|ge)</remarks>
        [Required]
        [XmlAttribute("relation")]
        public string? Relation { get; set; }

        [Required]
        [XmlAttribute("value")]
        public string? Value { get; set; }
    }
}