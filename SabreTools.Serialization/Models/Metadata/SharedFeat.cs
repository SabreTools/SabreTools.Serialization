using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeat : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

        #endregion

        public SharedFeat() => Type = ItemType.SharedFeat;
    }
}
