using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Keys

        /// <remarks>(yes|no) "no"</remarks>
        public const string InvertedKey = "inverted";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string, possibly long</remarks>
        public const string NumberKey = "number";

        #endregion

        public ConfLocation() => Type = ItemType.ConfLocation;
    }
}
