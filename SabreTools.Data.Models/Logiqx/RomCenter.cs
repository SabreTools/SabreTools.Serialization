using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("romcenter")]
    public class RomCenter
    {
        [XmlAttribute("plugin")]
        public string? Plugin { get; set; }

        /// <remarks>(none|split|merged|unmerged|fullmerged|device|full) "split"</remarks>
        [XmlAttribute("rommode")]
        public MergingFlag RomMode { get; set; }

        /// <remarks>(none|split|merged|unmerged|fullmerged|device|full) "split"</remarks>
        [XmlAttribute("biosmode")]
        public MergingFlag BiosMode { get; set; }

        /// <remarks>(none|split|merged|unmerged|fullmerged|device|full) "merged"</remarks>
        [XmlAttribute("samplemode")]
        public MergingFlag SampleMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("lockrommode")]
        public bool? LockRomMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("lockbiosmode")]
        public bool? LockBiosMode { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("locksamplemode")]
        public bool? LockSampleMode { get; set; }
    }
}
