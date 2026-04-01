using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("slot"), XmlRoot("slot")]
    public class Slot : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>SlotOption[]</remarks>
        [NoFilter]
        public const string SlotOptionKey = "slotoption";

        #endregion

        public Slot() => ItemType = ItemType.Slot;
    }
}
