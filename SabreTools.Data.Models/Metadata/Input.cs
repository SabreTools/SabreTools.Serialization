using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem
    {
        #region Properties

        public long? Buttons { get; set; }

        public long? Coins { get; set; }

        /// <remarks>Attribute also named Control</remarks>
        public string? ControlAttr { get; set; }

        public long? Players { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Service { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Tilt { get; set; }

        #endregion

        #region Keys

        /// <remarks>Control[]</remarks>
        public const string ControlKey = "control";

        #endregion

        public Input() => ItemType = ItemType.Input;
    }
}
