using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("dump"), XmlRoot("dump")]
    public class Dump : DatItem
    {
        #region Keys

        /// <remarks>Rom</remarks>
        [NoFilter]
        public const string MegaRomKey = "megarom";

        /// <remarks>Original</remarks>
        [NoFilter]
        public const string OriginalKey = "original";

        /// <remarks>Rom</remarks>
        [NoFilter]
        public const string RomKey = "rom";

        /// <remarks>Rom</remarks>
        [NoFilter]
        public const string SCCPlusCartKey = "sccpluscart";

        #endregion

        public Dump() => Type = ItemType.Dump;
    }
}
