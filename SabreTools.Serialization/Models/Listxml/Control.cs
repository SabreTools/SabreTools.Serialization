using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("control")]
    public class Control
    {
        /// <remarks>(joy|stick|paddle|pedal|lightgun|positional|dial|trackball|mouse|only_buttons|keypad|keyboard|mahjong|hanafuda|gambling)</remarks>
        [Required]
        [XmlAttribute("type")]
        public string? Type { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("player")]
        public string? Player { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("buttons")]
        public string? Buttons { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("reqbuttons")]
        public string? ReqButtons { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("minimum")]
        public string? Minimum { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("maximum")]
        public string? Maximum { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("sensitivity")]
        public string? Sensitivity { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("keydelta")]
        public string? KeyDelta { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("reverse")]
        public string? Reverse { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("ways")]
        public string? Ways { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("ways2")]
        public string? Ways2 { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("ways3")]
        public string? Ways3 { get; set; }
    }
}