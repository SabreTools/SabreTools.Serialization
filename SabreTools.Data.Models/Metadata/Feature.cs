using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem
    {
        #region Properties

        /// <remarks>(protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan)</remarks>
        public FeatureType? FeatureType { get; set; }

        public string? Name { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        public FeatureStatus? Overall { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        public FeatureStatus? Status { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public Feature() => ItemType = ItemType.Feature;
    }
}
