using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem
    {
        #region Properties

        public string? Mask { get; set; }

        public string? Name { get; set; }

        public string? Tag { get; set; }

        #endregion

        #region Keys

        /// <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        /// <remarks>ConfLocation[]</remarks>
        [NoFilter]
        public const string ConfLocationKey = "conflocation";

        /// <remarks>ConfSetting[]</remarks>
        [NoFilter]
        public const string ConfSettingKey = "confsetting";

        #endregion

        public Configuration() => ItemType = ItemType.Configuration;
    }
}
