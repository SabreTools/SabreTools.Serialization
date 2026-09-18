using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OpenMSX
{
    /// <summary>
    /// Base class to unify the various rom types
    /// </summary>
    public abstract class RomBase
    {
        /// <remarks>
        /// "hash" in older versions, "sha1" in newer versions
        /// </remarks>
        [XmlAttribute("sha1")]
        [XmlElement("hash")]
        public string? SHA1 { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [XmlAttribute("type")]
        [XmlElement("type")]
        public string? Type { get; set; }

        /// <remarks>
        /// Only present in newer versions
        /// </remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [XmlAttribute("remark")]
        [XmlElement("remark")]
        public string? Remark { get; set; }

        /// <remarks>
        /// Only present in older versions
        /// </remarks>
        [XmlElement("start")]
        public string? Start { get; set; }
    }
}
