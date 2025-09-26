using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("clrmamepro")]
    public class ClrMamePro
    {
        [XmlAttribute("header")]
        public string? Header { get; set; }

        /// <remarks>(none|split|merged|nonmerged|fullmerged|device|full) "split"</remarks>
        [XmlAttribute("forcemerging")]
        public string? ForceMerging { get; set; }

        /// <remarks>(obsolete|required|ignore) "obsolete"</remarks>
        [XmlAttribute("forcenodump")]
        public string? ForceNodump { get; set; }

        /// <remarks>(zip|unzip) "zip"</remarks>
        [XmlAttribute("forcepacking")]
        public string? ForcePacking { get; set; }
    }
}