using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("chip"), XmlRoot("chip")]
    public class Chip : DatItem
    {
        #region Properties

        public string? Name { get; set; }

        #endregion

        #region Keys

        /// <remarks>long</remarks>
        public const string ClockKey = "clock";

        /// <remarks>string</remarks>
        public const string FlagsKey = "flags";

        /// <remarks>(yes|no) "no"</remarks>
        public const string SoundOnlyKey = "soundonly";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        /// <remarks>(cpu|audio)</remarks>
        public const string ChipTypeKey = "type";

        #endregion

        public Chip() => ItemType = ItemType.Chip;
    }
}
