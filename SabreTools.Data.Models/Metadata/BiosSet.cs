using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>bool</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        #endregion

        public BiosSet() => ItemType = ItemType.BiosSet;
    }
}
