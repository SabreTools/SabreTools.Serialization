using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Incomplete { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? NoSoundHardware { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? RequiresArtwork { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Unofficial { get; set; }

        #endregion

        #region Keys

        /// <remarks>(plain|dirty)</remarks>
        public const string BlitKey = "blit";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string CocktailKey = "cocktail";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string ColorKey = "color";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string EmulationKey = "emulation";

        /// <remarks>string, possibly long</remarks>
        public const string PaletteSizeKey = "palettesize";

        /// <remarks>(supported|unsupported)</remarks>
        public const string SaveStateKey = "savestate";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string SoundKey = "sound";

        /// <remarks>(good|imperfect|preliminary|test)</remarks>
        public const string StatusKey = "status";

        #endregion

        public Driver() => ItemType = ItemType.Driver;
    }
}
