using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("dipvalue"), XmlRoot("dipvalue")]
    public class DipValue : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        public string? Value { get; set; }

        #endregion

        #region Keys

        /// <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        #endregion

        public DipValue() => ItemType = ItemType.DipValue;
    }
}
