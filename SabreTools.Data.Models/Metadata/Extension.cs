using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Extension() => ItemType = ItemType.Extension;
    }
}
