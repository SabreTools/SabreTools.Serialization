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

        #endregion

        #region Keys

        /// <remarks>Rom[]</remarks>
        [NoFilter]
        public const string RomKey = "rom";

        /// <remarks>long</remarks>
        public const string SizeKey = "size";

        /// <remarks>(8|16|32|64) "8"</remarks>
        /// TODO: Convert to enum
        public const string WidthKey = "width";

        #endregion

        public DataArea() => ItemType = ItemType.DataArea;
    }
}
