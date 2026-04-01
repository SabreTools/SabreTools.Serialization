using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("info"), XmlRoot("info")]
    public class Info : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public Info() => ItemType = ItemType.Info;
    }
}
