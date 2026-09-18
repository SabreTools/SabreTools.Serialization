using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OpenMSX
{
    [XmlRoot("software")]
    public class Software
    {
        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [Required]
        [XmlAttribute("title")]
        [XmlElement("title")]
        public string? Title { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [Required]
        [XmlAttribute("system")]
        [XmlElement("system")]
        public string? System { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [Required]
        [XmlAttribute("company")]
        [XmlElement("company")]
        public string? Company { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [Required]
        [XmlAttribute("year")]
        [XmlElement("year")]
        public string? Year { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [Required]
        [XmlAttribute("country")]
        [XmlElement("country")]
        public string? Country { get; set; }

        /// <remarks>
        /// Element in older versions and DTD, attribute in newer versions
        /// </remarks>
        [XmlAttribute("genmsxid")]
        [XmlElement("genmsxid")]
        public string? GenMSXID { get; set; }

        /// <remarks>
        /// Present in older versions
        /// </remarks>
        [XmlElement("dump")]
        public Dump[]? Dump { get; set; }

        /// <remarks>
        /// Present in newer versions
        /// </remarks>
        [XmlElement("rom")]
        public RomBase[]? Rom { get; set; }
    }
}
