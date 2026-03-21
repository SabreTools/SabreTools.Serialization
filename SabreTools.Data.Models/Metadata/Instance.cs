using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string BriefNameKey = "briefname";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Instance() => Type = ItemType.Instance;
    }
}
