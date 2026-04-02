using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem
    {
        #region Properties

        public string? Mask { get; set; }

        #endregion

        public Analog() => ItemType = ItemType.Analog;
    }
}
