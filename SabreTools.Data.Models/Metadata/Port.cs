using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Properties

        public string? Tag { get; set; }

        #endregion

        #region Keys

        /// <remarks>Analog[]</remarks>
        [NoFilter]
        public const string AnalogKey = "analog";

        #endregion

        public Port() => ItemType = ItemType.Port;
    }
}
