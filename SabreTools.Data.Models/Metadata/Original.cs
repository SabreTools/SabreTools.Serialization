using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("original"), XmlRoot("original")]
    public class Original : DatItem
    {
        #region Properties

        public string? Content { get; set; }

        #endregion

        #region Keys

        /// <remarks>bool</remarks>
        public const string ValueKey = "value";

        #endregion

        public Original() => ItemType = ItemType.Original;
    }
}
