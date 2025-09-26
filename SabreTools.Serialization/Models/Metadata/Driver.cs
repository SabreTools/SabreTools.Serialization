using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Models.Metadata
{
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Keys

        /// <remarks>(plain|dirty)</remarks>
        public const string BlitKey = "blit";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string CocktailKey = "cocktail";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string ColorKey = "color";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string EmulationKey = "emulation";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IncompleteKey = "incomplete";

        /// <remarks>(yes|no) "no"</remarks>
        public const string NoSoundHardwareKey = "nosoundhardware";

        /// <remarks>string, possibly long</remarks>
        public const string PaletteSizeKey = "palettesize";

        /// <remarks>(yes|no) "no"</remarks>
        public const string RequiresArtworkKey = "requiresartwork";

        /// <remarks>(supported|unsupported)</remarks>
        public const string SaveStateKey = "savestate";

        /// <remarks>(good|imperfect|preliminary)</remarks>
        public const string SoundKey = "sound";

        /// <remarks>(good|imperfect|preliminary|test)</remarks>
        public const string StatusKey = "status";

        /// <remarks>(yes|no) "no"</remarks>
        public const string UnofficialKey = "unofficial";

        #endregion

        public Driver() => Type = ItemType.Driver;
    }
}
