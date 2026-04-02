using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public class DipLocation : DatItem
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

        public DipLocation() => ItemType = ItemType.DipLocation;
    }
}
