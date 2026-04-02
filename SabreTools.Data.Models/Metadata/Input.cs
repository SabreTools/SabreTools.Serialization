using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Service { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Tilt { get; set; }

        #endregion

        #region Keys

        /// <remarks>long</remarks>
        public const string ButtonsKey = "buttons";

        /// <remarks>long</remarks>
        public const string CoinsKey = "coins";

        /// <remarks>string / Control[]</remarks>
        public const string ControlKey = "control";

        /// <remarks>long</remarks>
        public const string PlayersKey = "players";

        #endregion

        public Input() => ItemType = ItemType.Input;
    }
}
