using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Models;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("software"), XmlRoot("software")]
    public class Software : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string CloneOfKey = "cloneof";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>Info[]</remarks>
        [NoFilter]
        public const string InfoKey = "info";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

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

        /// <remarks>(yes|partial|no) "yes"</remarks>
        public const string SupportedKey = "supported";

        /// <remarks>string</remarks>
        public const string YearKey = "year";

        #endregion

        public Software() => Type = ItemType.Software;
    }
}
