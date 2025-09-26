using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("release"), XmlRoot("release")]
    public class Release : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string DateKey = "date";

        /// <remarks>(yes|no) "no"</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string LanguageKey = "language";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string RegionKey = "region";

        #endregion

        public Release() => Type = ItemType.Release;
    }
}
