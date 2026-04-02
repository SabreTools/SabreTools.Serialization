using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public class ConfSetting : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public ConfSetting() => ItemType = ItemType.ConfSetting;
    }
}
