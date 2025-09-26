using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem
    {
        #region Keys

        /// <remarks>bool</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public BiosSet() => Type = ItemType.BiosSet;
    }
}
