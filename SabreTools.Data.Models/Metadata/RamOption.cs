using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string ContentKey = "content";

        /// <remarks>(yes|no) "no"</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public RamOption() => Type = ItemType.RamOption;
    }
}
