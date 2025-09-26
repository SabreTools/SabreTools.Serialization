using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Models;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem
    {
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

        /// <remarks>string</remarks>
        public const string MaskKey = "mask";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public Configuration() => Type = ItemType.Configuration;
    }
}
