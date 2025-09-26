using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("diskarea"), XmlRoot("diskarea")]
    public class DiskArea : DatItem
    {
        #region Keys

        /// <remarks>Disk[]</remarks>
        [NoFilter]
        public const string DiskKey = "disk";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public DiskArea() => Type = ItemType.DiskArea;
    }
}
