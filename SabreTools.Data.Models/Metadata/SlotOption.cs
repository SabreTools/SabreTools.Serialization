using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? DevName { get; set; }

        public string? Name { get; set; }

        #endregion

        public SlotOption() => ItemType = ItemType.SlotOption;
    }
}
