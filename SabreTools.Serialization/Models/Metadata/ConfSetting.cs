using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public class ConfSetting : DatItem
    {
        #region Keys

        /// <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        /// <remarks>(yes|no) "no"</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public ConfSetting() => Type = ItemType.ConfSetting;
    }
}
