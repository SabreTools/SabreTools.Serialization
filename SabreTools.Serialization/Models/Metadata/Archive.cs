using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("archive"), XmlRoot("archive")]
    public class Archive : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Archive() => Type = ItemType.Archive;
    }
}
