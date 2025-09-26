using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("driver")]
    public class Driver
    {
        /// <remarks>(good|imperfect|preliminary), (good|preliminary|test) in older versions</remarks>
        [Required]
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>(good|imperfect|preliminary), Only present in older versions</remarks>
        [XmlAttribute("color")]
        public string? Color { get; set; }

        /// <remarks>(good|imperfect|preliminary), Only present in older versions</remarks>
        [XmlAttribute("sound")]
        public string? Sound { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("palettesize")]
        public string? PaletteSize { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        [Required]
        [XmlAttribute("emulation")]
        public string? Emulation { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        [Required]
        [XmlAttribute("cocktail")]
        public string? Cocktail { get; set; }

        /// <remarks>(supported|unsupported)</remarks>
        [Required]
        [XmlAttribute("savestate")]
        public string? SaveState { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("requiresartwork")]
        public string? RequiresArtwork { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("unofficial")]
        public string? Unofficial { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("nosoundhardware")]
        public string? NoSoundHardware { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("incomplete")]
        public string? Incomplete { get; set; }
    }
}