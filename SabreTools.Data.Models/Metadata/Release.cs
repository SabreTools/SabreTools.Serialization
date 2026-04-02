using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("release"), XmlRoot("release")]
    public class Release : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string DateKey = "date";

        /// <remarks>string</remarks>
        public const string LanguageKey = "language";

        /// <remarks>string</remarks>
        public const string RegionKey = "region";

        #endregion

        public Release() => ItemType = ItemType.Release;
    }
}
