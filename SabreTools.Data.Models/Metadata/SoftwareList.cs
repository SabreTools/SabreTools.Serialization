using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        /// <remarks>(original|compatible)</remarks>
        public SoftwareListStatus? Status { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>string</remarks>
        public const string FilterKey = "filter";

        /// <remarks>string</remarks>
        public const string NotesKey = "notes";

        /// <remarks>Software[]</remarks>
        [NoFilter]
        public const string SoftwareKey = "software";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public SoftwareList() => ItemType = ItemType.SoftwareList;
    }
}
