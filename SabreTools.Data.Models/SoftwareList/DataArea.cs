using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.SoftwareList
{
    [XmlRoot("dataarea")]
    public class DataArea
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("size")]
        public long? Size { get; set; }

        /// <remarks>(8|16|32|64) "8"</remarks>
        [XmlAttribute("width")]
        public Width? Width { get; set; }

        /// <remarks>(big|little) "little"</remarks>
        [XmlAttribute("endianness")]
        public Endianness? Endianness { get; set; }

        [XmlElement("rom")]
        public Rom[]? Rom { get; set; }
    }
}
