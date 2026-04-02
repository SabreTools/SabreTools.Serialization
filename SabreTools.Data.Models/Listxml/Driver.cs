using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("driver")]
    public class Driver
    {
        /// <remarks>(good|imperfect|preliminary), (good|preliminary|test) in older versions</remarks>
        [Required]
        [XmlAttribute("status")]
        public SupportStatus? Status { get; set; }

        /// <remarks>(good|imperfect|preliminary), Only present in older versions</remarks>
        [XmlAttribute("color")]
        public SupportStatus? Color { get; set; }

        /// <remarks>(good|imperfect|preliminary), Only present in older versions</remarks>
        [XmlAttribute("sound")]
        public SupportStatus? Sound { get; set; }

        /// <remarks>Only present in older versions</remarks>
        [XmlAttribute("palettesize")]
        public string? PaletteSize { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        [Required]
        [XmlAttribute("emulation")]
        public SupportStatus? Emulation { get; set; }

        /// <remarks>(good|imperfect|preliminary)</remarks>
        [Required]
        [XmlAttribute("cocktail")]
        public SupportStatus? Cocktail { get; set; }

        /// <remarks>(supported|unsupported)</remarks>
        [Required]
        [XmlAttribute("savestate")]
        public Supported? SaveState { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("requiresartwork")]
        public bool? RequiresArtwork { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("unofficial")]
        public bool? Unofficial { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("nosoundhardware")]
        public bool? NoSoundHardware { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("incomplete")]
        public bool? Incomplete { get; set; }
    }
}
