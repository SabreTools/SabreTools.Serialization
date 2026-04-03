using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Properties

        public string? Description { get; set; }

        public string? Filter { get; set; }

        public string? Name { get; set; }

        public string? Notes { get; set; }

        /// <remarks>(original|compatible)</remarks>
        public SoftwareListStatus? Status { get; set; }

        public string? Tag { get; set; }

        #endregion

        #region Keys

        /// <remarks>Software[]</remarks>
        [NoFilter]
        public const string SoftwareKey = "software";

        #endregion

        public SoftwareList() => ItemType = ItemType.SoftwareList;
    }
}
