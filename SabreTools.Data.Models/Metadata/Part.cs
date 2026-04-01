using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("part"), XmlRoot("part")]
    public class Part : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>DataArea[]</remarks>
        [NoFilter]
        public const string DataAreaKey = "dataarea";

        /// <remarks>DiskArea[]</remarks>
        [NoFilter]
        public const string DiskAreaKey = "diskarea";

        /// <remarks>DipSwitch[]</remarks>
        [NoFilter]
        public const string DipSwitchKey = "dipswitch";

        /// <remarks>Feature[]</remarks>
        [NoFilter]
        public const string FeatureKey = "feature";

        /// <remarks>string</remarks>
        public const string InterfaceKey = "interface";

        #endregion

        public Part() => ItemType = ItemType.Part;
    }
}
