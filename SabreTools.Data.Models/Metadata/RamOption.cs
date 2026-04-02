using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Properties

        public string? Content { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        public RamOption() => ItemType = ItemType.RamOption;
    }
}
