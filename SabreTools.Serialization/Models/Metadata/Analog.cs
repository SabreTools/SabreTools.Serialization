using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string MaskKey = "mask";

        #endregion

        public Analog() => Type = ItemType.Analog;
    }
}
