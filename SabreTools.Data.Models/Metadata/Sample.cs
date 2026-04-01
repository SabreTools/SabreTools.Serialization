using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("sample"), XmlRoot("sample")]
    public class Sample : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        public Sample() => ItemType = ItemType.Sample;
    }
}
