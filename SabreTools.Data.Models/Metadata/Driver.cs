using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Properties

        /// <remarks>(plain|dirty)</remarks>
        public Blit? Blit { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Cocktail { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Color { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Emulation { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Incomplete { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? NoSoundHardware { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? RequiresArtwork { get; set; }

        /// <remarks>(supported|unsupported)</remarks>
        public Supported? SaveState { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public SupportStatus? Sound { get; set; }

        /// <remarks>(good|imperfect|preliminary|test)</remarks>
        public SupportStatus? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Unofficial { get; set; }

        #endregion

        #region Keys

        /// <remarks>string, possibly long</remarks>
        public const string PaletteSizeKey = "palettesize";

        #endregion

        public Driver() => ItemType = ItemType.Driver;
    }
}
