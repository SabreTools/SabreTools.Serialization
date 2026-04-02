using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("software"), XmlRoot("software")]
    public class Software : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        /// <remarks>(yes|partial|no) "yes"</remarks>
        public Supported? Supported { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string CloneOfKey = "cloneof";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>Info[]</remarks>
        [NoFilter]
        public const string InfoKey = "info";

        /// <remarks>string</remarks>
        public const string NotesKey = "notes";

        /// <remarks>Part[]</remarks>
        [NoFilter]
        public const string PartKey = "part";

        /// <remarks>string</remarks>
        public const string PublisherKey = "publisher";

        /// <remarks>SharedFeat[]</remarks>
        [NoFilter]
        public const string SharedFeatKey = "sharedfeat";

        /// <remarks>string</remarks>
        public const string YearKey = "year";

        #endregion

        public Software() => ItemType = ItemType.Software;
    }
}
