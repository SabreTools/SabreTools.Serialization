using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("source_details"), XmlRoot("source_details")]
    public class SourceDetails : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string IdKey = "id";

        /// <remarks>string</remarks>
        public const string AppendToNumberKey = "appendtonumber";

        /// <remarks>string</remarks>
        public const string SectionKey = "section";

        /// <remarks>string</remarks>
        public const string RomInfoKey = "rominfo";

        /// <remarks>string</remarks>
        public const string DumpDateKey = "dumpdate";

        /// <remarks>byte</remarks>
        public const string DumpDateInfoKey = "dumpdateinfo";

        /// <remarks>string</remarks>
        public const string ReleaseDateKey = "releasedate";

        /// <remarks>byte</remarks>
        public const string ReleaseDateInfoKey = "releasedateinfo";

        /// <remarks>string</remarks>
        public const string DumperKey = "dumper";

        /// <remarks>string</remarks>
        public const string ProjectKey = "project";

        /// <remarks>string</remarks>
        public const string OriginalFormatKey = "originalformat";

        /// <remarks>byte</remarks>
        public const string NodumpKey = "nodump";

        /// <remarks>string</remarks>
        public const string ToolKey = "tool";

        /// <remarks>string</remarks>
        public const string OriginKey = "origin";

        /// <remarks>string</remarks>
        public const string Comment1Key = "comment1";

        /// <remarks>string</remarks>
        public const string Comment2Key = "comment2";

        /// <remarks>string</remarks>
        public const string Link1Key = "link1";

        /// <remarks>byte</remarks>
        public const string Link1PublicKey = "link1public";

        /// <remarks>string</remarks>
        public const string Link2Key = "link2";

        /// <remarks>byte</remarks>
        public const string Link2PublicKey = "link2public";

        /// <remarks>string</remarks>
        public const string Link3Key = "link3";

        /// <remarks>byte</remarks>
        public const string Link3PublicKey = "link3public";

        /// <remarks>string</remarks>
        public const string RegionKey = "region";

        /// <remarks>string</remarks>
        public const string MediaTitleKey = "mediatitle";

        #endregion

        public SourceDetails() => Type = ItemType.SourceDetails;
    }
}
