using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Keys

        /// <remarks>Condition</remarks>
        [NoFilter]
        public const string ConditionKey = "condition";

        /// <remarks>(yes|no) "no"</remarks>
        public const string DefaultKey = "default";

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
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public DipSwitch() => Type = ItemType.DipSwitch;
    }
}
