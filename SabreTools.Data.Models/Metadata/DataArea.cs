using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem
    {
        #region Properties

        /// <remarks>(big|little) "little"</remarks>
        public Endianness? Endianness { get; set; }

        public string? Name { get; set; }

        public long? Size { get; set; }

        /// <remarks>(8|16|32|64) "8"</remarks>
        public Width? Width { get; set; }

        #endregion

        #region Keys

        /// <remarks>Rom[]</remarks>
        [NoFilter]
        public const string RomKey = "rom";

        #endregion

        public DataArea() => ItemType = ItemType.DataArea;
    }
}
