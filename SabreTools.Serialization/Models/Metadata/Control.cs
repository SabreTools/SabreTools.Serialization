using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem
    {
        #region Keys

        /// <remarks>long</remarks>
        public const string ButtonsKey = "buttons";

        /// <remarks>long</remarks>
        public const string KeyDeltaKey = "keydelta";

        /// <remarks>long</remarks>
        public const string MaximumKey = "maximum";

        /// <remarks>long</remarks>
        public const string MinimumKey = "minimum";

        /// <remarks>long</remarks>
        public const string PlayerKey = "player";

        /// <remarks>long</remarks>
        public const string ReqButtonsKey = "reqbuttons";

        /// <remarks>(yes|no) "no"</remarks>
        public const string ReverseKey = "reverse";

        /// <remarks>long</remarks>
        public const string SensitivityKey = "sensitivity";

        /// <remarks>(joy|stick|paddle|pedal|lightgun|positional|dial|trackball|mouse|only_buttons|keypad|keyboard|mahjong|hanafuda|gambling)</remarks>
        public const string ControlTypeKey = "type";

        /// <remarks>string, possibly long</remarks>
        public const string WaysKey = "ways";

        /// <remarks>string, possibly long</remarks>
        public const string Ways2Key = "ways2";

        /// <remarks>string, possibly long</remarks>
        public const string Ways3Key = "ways3";

        #endregion

        public Control() => Type = ItemType.Control;
    }
}
