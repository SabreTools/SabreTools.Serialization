using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>(unemulated|imperfect)</remarks>
        public const string OverallKey = "overall";

        /// <remarks>(unemulated|imperfect)</remarks>
        public const string StatusKey = "status";

        /// <remarks>(protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan)</remarks>
        public const string FeatureTypeKey = "type";

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public Feature() => Type = ItemType.Feature;
    }
}
