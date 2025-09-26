using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.SoftwareList
{
    [XmlRoot("dipvalue")]
    public class DipValue
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [Required]
        [XmlAttribute("value")]
        public string? Value { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("default")]
        public string? Default { get; set; }
    }
}