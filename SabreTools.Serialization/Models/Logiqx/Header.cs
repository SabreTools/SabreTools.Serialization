using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("header")]
    public class Header
    {
        /// <remarks>No-Intro extension</remarks>
        [XmlElement("id")]
        public string? Id { get; set; }

        [Required]
        [XmlElement("name")]
        public string? Name { get; set; }

        [Required]
        [XmlElement("description")]
        public string? Description { get; set; }

        /// <remarks>Trurip extension</remarks>
        [XmlElement("rootdir")]
        public string? RootDir { get; set; }

        [XmlElement("category")]
        public string? Category { get; set; }

        [Required]
        [XmlElement("version")]
        public string? Version { get; set; }

        [XmlElement("date")]
        public string? Date { get; set; }

        [Required]
        [XmlElement("author")]
        public string? Author { get; set; }

        [XmlElement("email")]
        public string? Email { get; set; }

        [XmlElement("homepage")]
        public string? Homepage { get; set; }

        [XmlElement("url")]
        public string? Url { get; set; }

        [XmlElement("comment")]
        public string? Comment { get; set; }

        /// <remarks>Trurip extension</remarks>
        [XmlElement("type")]
        public string? Type { get; set; }

        [XmlElement("clrmamepro")]
        public ClrMamePro? ClrMamePro { get; set; }

        [XmlElement("romcenter")]
        public RomCenter? RomCenter { get; set; }
    }
}