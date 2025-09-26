using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem
    {
        #region Keys

        /// <remarks>(big|little) "little"</remarks>
        public const string EndiannessKey = "endianness";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>Rom[]</remarks>
        [NoFilter]
        public const string RomKey = "rom";

        /// <remarks>long</remarks>
        public const string SizeKey = "size";

        /// <remarks>(8|16|32|64) "8"</remarks>
        public const string WidthKey = "width";

        #endregion

        public DataArea() => Type = ItemType.DataArea;
    }
}
