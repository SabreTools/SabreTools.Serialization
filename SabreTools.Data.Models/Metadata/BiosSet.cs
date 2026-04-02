using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Description { get; set; }

        public string? Name { get; set; }

        #endregion

        public BiosSet() => ItemType = ItemType.BiosSet;
    }
}
