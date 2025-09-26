using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>string</remarks>
        public const string FilterKey = "filter";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string NotesKey = "notes";

        /// <remarks>Software[]</remarks>
        [NoFilter]
        public const string SoftwareKey = "software";

        /// <remarks>(original|compatible)</remarks>
        public const string StatusKey = "status";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public SoftwareList() => Type = ItemType.SoftwareList;
    }
}
