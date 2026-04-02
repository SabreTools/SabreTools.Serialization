using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Inverted { get; set; }

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string, possibly long</remarks>
        public const string NumberKey = "number";

        #endregion

        public ConfLocation() => ItemType = ItemType.ConfLocation;
    }
}
