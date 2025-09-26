using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem
    {
        #region Keys

        /// <remarks>(yes|no) "no"</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string DevNameKey = "devname";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public SlotOption() => Type = ItemType.SlotOption;
    }
}
