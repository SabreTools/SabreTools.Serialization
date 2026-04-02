using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("chip"), XmlRoot("chip")]
    public class Chip : DatItem
    {
        #region Properties

        /// <remarks>(cpu|audio)</remarks>
        public ChipType? ChipType { get; set; }

        public string? Name { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? SoundOnly { get; set; }

        public string? Tag { get; set; }

        #endregion

        #region Keys

        /// <remarks>long</remarks>
        public const string ClockKey = "clock";

        /// <remarks>string</remarks>
        public const string FlagsKey = "flags";

        #endregion

        public Chip() => ItemType = ItemType.Chip;
    }
}
