using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("info"), XmlRoot("info")]
    public class Info : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        public string? Value { get; set; }

        #endregion

        public Info() => ItemType = ItemType.Info;
    }
}
