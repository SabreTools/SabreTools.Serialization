using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeat : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public SharedFeat() => ItemType = ItemType.SharedFeat;
    }
}
