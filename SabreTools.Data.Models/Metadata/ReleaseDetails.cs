using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("release_details"), XmlRoot("release_details")]
    public class ReleaseDetails : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string IdKey = "id";

        /// <remarks>string</remarks>
        public const string AppendToNumberKey = "appendtonumber";

        /// <remarks>string</remarks>
        public const string DateKey = "date";

        /// <remarks>string</remarks>
        public const string OriginalFormatKey = "originalformat";

        /// <remarks>string</remarks>
        public const string GroupKey = "group";

        /// <remarks>string</remarks>
        public const string DirNameKey = "dirname";

        /// <remarks>string</remarks>
        public const string NfoNameKey = "nfoname";

        /// <remarks>string</remarks>
        public const string NfoSizeKey = "nfosize";

        /// <remarks>string</remarks>
        public const string NfoCRCKey = "nfocrc";

        /// <remarks>string</remarks>
        public const string ArchiveNameKey = "archivename";

        /// <remarks>string</remarks>
        public const string RomInfoKey = "rominfo";

        /// <remarks>string</remarks>
        public const string CategoryKey = "category";

        /// <remarks>string</remarks>
        public const string CommentKey = "comment";

        /// <remarks>string</remarks>
        public const string ToolKey = "tool";

        /// <remarks>string</remarks>
        public const string RegionKey = "region";

        /// <remarks>string</remarks>
        public const string OriginKey = "origin";

        #endregion

        public ReleaseDetails() => Type = ItemType.ReleaseDetails;
    }
}
