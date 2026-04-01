using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string BriefNameKey = "briefname";

        #endregion

        public Instance() => ItemType = ItemType.Instance;
    }
}
