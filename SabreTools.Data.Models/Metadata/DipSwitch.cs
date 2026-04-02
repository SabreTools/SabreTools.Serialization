using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Default { get; set; }

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        /// <remarks>DipLocation[]</remarks>
        [NoFilter]
        public const string DipLocationKey = "diplocation";

        /// <remarks>DipValue[]</remarks>
        [NoFilter]
        public const string DipValueKey = "dipvalue";

        /// <remarks>string[]</remarks>
        public const string EntryKey = "entry";

        /// <remarks>string</remarks>
        public const string MaskKey = "mask";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public DipSwitch() => ItemType = ItemType.DipSwitch;
    }
}
