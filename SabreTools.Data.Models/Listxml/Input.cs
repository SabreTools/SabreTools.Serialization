using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("input")]
    public class Input
    {
        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("service")]
        public bool? Service { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("tilt")]
        public bool? Tilt { get; set; }

        [Required]
        [XmlAttribute("players")]
        public long? Players { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("control")]
        public string? ControlAttr { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("buttons")]
        public long? Buttons { get; set; }

        [XmlAttribute("coins")]
        public long? Coins { get; set; }

        [XmlElement("control")]
        public Control[]? Control { get; set; }
    }
}
