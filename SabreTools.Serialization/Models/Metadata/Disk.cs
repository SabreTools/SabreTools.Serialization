using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("disk"), XmlRoot(elementName: "disk")]
    public class Disk : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string FlagsKey = "flags";

        /// <remarks>string, possibly long</remarks>
        public const string IndexKey = "index";

        /// <remarks>string</remarks>
        public const string MD5Key = "md5";

        /// <remarks>string</remarks>
        public const string MergeKey = "merge";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>(yes|no) "no"</remarks>
        public const string OptionalKey = "optional";

        /// <remarks>string</remarks>
        public const string RegionKey = "region";

        /// <remarks>string</remarks>
        public const string SHA1Key = "sha1";

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        public const string StatusKey = "status";

        /// <remarks>(yes|no) "no"</remarks>
        public const string WritableKey = "writable";

        #endregion

        public Disk() => Type = ItemType.Disk;
    }
}
