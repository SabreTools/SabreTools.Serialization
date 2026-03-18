using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("media"), XmlRoot("media")]
    public class Media : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string MD5Key = "md5";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string SHA1Key = "sha1";

        /// <remarks>string</remarks>
        public const string SHA256Key = "sha256";

        /// <remarks>string</remarks>
        public const string SpamSumKey = "spamsum";

        #endregion

        public Media() => Type = ItemType.Media;
    }
}
