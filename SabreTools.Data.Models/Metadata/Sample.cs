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

        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Sample() => ItemType = ItemType.Sample;
    }
}
