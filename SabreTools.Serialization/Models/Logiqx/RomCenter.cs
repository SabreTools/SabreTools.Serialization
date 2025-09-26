using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("romcenter")]
    public class RomCenter
    {
        [XmlAttribute("plugin")]
        public string? Plugin { get; set; }

        /// <remarks>(none|split|merged|unmerged|fullmerged|device|full) "split"</remarks>
        [XmlAttribute("rommode")]
        public string? RomMode { get; set; }

        /// <remarks>(none|split|merged|unmerged|fullmerged|device|full) "split"</remarks>
        [XmlAttribute("biosmode")]
        public string? BiosMode { get; set; }

        /// <remarks>(none|split|merged|unmerged|fullmerged|device|full) "merged"</remarks>
        [XmlAttribute("samplemode")]
        public string? SampleMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("lockrommode")]
        public string? LockRomMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("lockbiosmode")]
        public string? LockBiosMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("locksamplemode")]
        public string? LockSampleMode { get; set; }
    }
}