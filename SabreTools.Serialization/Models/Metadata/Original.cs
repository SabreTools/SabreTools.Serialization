using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("original"), XmlRoot("original")]
    public class Original : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string ContentKey = "content";

        /// <remarks>bool</remarks>
        public const string ValueKey = "value";

        #endregion

        public Original() => Type = ItemType.Original;
    }
}
