using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("feature")]
    public class Feature
    {
        /// <remarks>(protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan)</remarks>
        [Required]
        [XmlAttribute("type")]
        public FeatureType? Type { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        [XmlAttribute("status")]
        public FeatureStatus? Status { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        [XmlAttribute("overall")]
        public FeatureStatus? Overall { get; set; }
    }
}
