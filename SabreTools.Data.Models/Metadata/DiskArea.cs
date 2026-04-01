using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("diskarea"), XmlRoot("diskarea")]
    public class DiskArea : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>Disk[]</remarks>
        [NoFilter]
        public const string DiskKey = "disk";

        #endregion

        public DiskArea() => ItemType = ItemType.DiskArea;
    }
}
