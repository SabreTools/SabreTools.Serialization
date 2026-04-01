using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>(yes|no) "no"</remarks>
        public const string InvertedKey = "inverted";

        /// <remarks>string, possibly long</remarks>
        public const string NumberKey = "number";

        #endregion

        public ConfLocation() => ItemType = ItemType.ConfLocation;
    }
}
