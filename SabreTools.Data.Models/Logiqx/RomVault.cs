using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("romvault")]
    public class RomVault
    {
        [XmlAttribute("header")]
        public string? Header { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        [XmlAttribute("forcemerging")]
        public MergingFlag ForceMerging { get; set; }

        /// <remarks>(obsolete|required|ignore) "obsolete"</remarks>
        [XmlAttribute("forcenodump")]
        public NodumpFlag ForceNodump { get; set; }

        /// <remarks>(zip|unzip) "zip"</remarks>
        [XmlAttribute("forcepacking")]
        public PackingFlag ForcePacking { get; set; }
    }
}
