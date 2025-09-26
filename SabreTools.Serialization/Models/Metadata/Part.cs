using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Models;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("part"), XmlRoot("part")]
    public class Part : DatItem
    {
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

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Part() => Type = ItemType.Part;
    }
}
