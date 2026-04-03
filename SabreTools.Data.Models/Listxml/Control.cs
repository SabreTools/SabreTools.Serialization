using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("control")]
    public class Control
    {
        /// <remarks>(joy|stick|paddle|pedal|lightgun|positional|dial|trackball|mouse|only_buttons|keypad|keyboard|mahjong|hanafuda|gambling)</remarks>
        [Required]
        [XmlAttribute("type")]
        public ControlType? Type { get; set; }

        [XmlAttribute("player")]
        public long? Player { get; set; }

        [XmlAttribute("buttons")]
        public long? Buttons { get; set; }

        [XmlAttribute("reqbuttons")]
        public long? ReqButtons { get; set; }

        [XmlAttribute("minimum")]
        public long? Minimum { get; set; }

        [XmlAttribute("maximum")]
        public long? Maximum { get; set; }

        [XmlAttribute("sensitivity")]
        public long? Sensitivity { get; set; }

        [XmlAttribute("keydelta")]
        public long? KeyDelta { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("reverse")]
        public bool? Reverse { get; set; }

        [XmlAttribute("ways")]
        public string? Ways { get; set; }

        [XmlAttribute("ways2")]
        public string? Ways2 { get; set; }

        [XmlAttribute("ways3")]
        public string? Ways3 { get; set; }
    }
}
