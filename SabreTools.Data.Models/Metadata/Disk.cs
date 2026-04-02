using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("disk"), XmlRoot(elementName: "disk")]
    public class Disk : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Optional { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        public ItemStatus? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Writable { get; set; }

        #endregion

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
        public const string RegionKey = "region";

        /// <remarks>string</remarks>
        public const string SHA1Key = "sha1";

        #endregion

        public Disk() => ItemType = ItemType.Disk;
    }
}
