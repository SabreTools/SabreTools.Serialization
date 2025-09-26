using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("info"), XmlRoot("info")]
    public class Info : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public Info() => Type = ItemType.Info;
    }
}
