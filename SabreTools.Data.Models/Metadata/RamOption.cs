using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string ContentKey = "content";

        /// <remarks>(yes|no) "no"</remarks>
        public const string DefaultKey = "default";

        #endregion

        public RamOption() => ItemType = ItemType.RamOption;
    }
}
