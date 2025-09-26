using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem
    {
        #region Keys

        /// <remarks>long</remarks>
        public const string ButtonsKey = "buttons";

        /// <remarks>long</remarks>
        public const string CoinsKey = "coins";

        /// <remarks>string / Control[]</remarks>
        public const string ControlKey = "control";

        /// <remarks>long</remarks>
        public const string PlayersKey = "players";

        /// <remarks>(yes|no) "no"</remarks>
        public const string ServiceKey = "service";

        /// <remarks>(yes|no) "no"</remarks>
        public const string TiltKey = "tilt";

        #endregion

        public Input() => Type = ItemType.Input;
    }
}
