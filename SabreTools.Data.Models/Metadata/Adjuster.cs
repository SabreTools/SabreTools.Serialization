using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public class Adjuster : DatItem
    {
        #region Properties

        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        #region Keys

        // <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        #endregion

        public Adjuster() => ItemType = ItemType.Adjuster;
    }
}
