using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Keys

        /// <remarks>Analog[]</remarks>
        [NoFilter]
        public const string AnalogKey = "analog";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public Port() => Type = ItemType.Port;
    }
}
