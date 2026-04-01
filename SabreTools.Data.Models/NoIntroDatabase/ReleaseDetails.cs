using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("details")]
    public class ReleaseDetails
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }

        [XmlAttribute("append_to_number")]
        public string? AppendToNumber { get; set; }

        [XmlAttribute("date")]
        public string? Date { get; set; }

        [XmlAttribute("originalformat")]
        public string? OriginalFormat { get; set; }

        [XmlAttribute("group")]
        public string? Group { get; set; }

        [XmlAttribute("dirname")]
        public string? DirName { get; set; }

        [XmlAttribute("nfoname")]
        public string? NfoName { get; set; }

        [XmlAttribute("nfosize")]
        public string? NfoSize { get; set; }

        [XmlAttribute("nfocrc")]
        public string? NfoCRC { get; set; }

        [XmlAttribute("archivename")]
        public string? ArchiveName { get; set; }

        [XmlAttribute("rominfo")]
        public string? RomInfo { get; set; }

        [XmlAttribute("category")]
        public string? Category { get; set; }

        [XmlAttribute("comment")]
        public string? Comment { get; set; }

        [XmlAttribute("tool")]
        public string? Tool { get; set; }

        [XmlAttribute("region")]
        public string? Region { get; set; }

        [XmlAttribute("origin")]
        public string? Origin { get; set; }
    }
}
