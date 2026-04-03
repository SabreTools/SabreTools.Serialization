using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem
    {
        #region Properties

        public long? Buttons { get; set; }

        /// <remarks>(joy|stick|paddle|pedal|lightgun|positional|dial|trackball|mouse|only_buttons|keypad|keyboard|mahjong|hanafuda|gambling)</remarks>
        public ControlType? ControlType { get; set; }

        public long? KeyDelta { get; set; }

        public long? Maximum { get; set; }

        public long? Minimum { get; set; }

        public long? Player { get; set; }

        public long? ReqButtons { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Reverse { get; set; }

        public long? Sensitivity { get; set; }

        public string? Ways { get; set; }

        public string? Ways2 { get; set; }

        public string? Ways3 { get; set; }

        #endregion

        public Control() => ItemType = ItemType.Control;
    }
}
