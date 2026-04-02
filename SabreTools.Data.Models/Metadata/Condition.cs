using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("condition"), XmlRoot("condition")]
    public class Condition : DatItem
    {
        #region Properties

        /// <remarks>(eq|ne|gt|le|lt|ge)</remarks>
        public Relation? Relation { get; set; }

        #endregion

        #region Keys

        /// <remarks>string</remarks>
        public const string ValueKey = "clock";

        /// <remarks>string</remarks>
        public const string MaskKey = "mask";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public Condition() => ItemType = ItemType.Condition;
    }
}
