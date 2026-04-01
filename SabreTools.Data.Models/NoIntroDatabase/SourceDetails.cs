using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("details")]
    public class SourceDetails
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }

        [XmlAttribute("append_to_number")]
        public string? AppendToNumber { get; set; }

        [XmlAttribute("section")]
        public string? Section { get; set; }

        [XmlAttribute("rominfo")]
        public string? RomInfo { get; set; }

        [XmlAttribute("d_date")]
        public string? DumpDate { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("d_date_info")]
        public string? DumpDateInfo { get; set; }

        [XmlAttribute("r_date")]
        public string? ReleaseDate { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("r_date_info")]
        public string? ReleaseDateInfo { get; set; }

        [XmlAttribute("dumper")]
        public string? Dumper { get; set; }

        [XmlAttribute("project")]
        public string? Project { get; set; }

        [XmlAttribute("originalformat")]
        public string? OriginalFormat { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("nodump")]
        public string? Nodump { get; set; }

        [XmlAttribute("tool")]
        public string? Tool { get; set; }

        [XmlAttribute("origin")]
        public string? Origin { get; set; }

        [XmlAttribute("comment1")]
        public string? Comment1 { get; set; }

        [XmlAttribute("comment2")]
        public string? Comment2 { get; set; }

        [XmlAttribute("link1")]
        public string? Link1 { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("link1_public")]
        public string? Link1Public { get; set; }

        [XmlAttribute("link2")]
        public string? Link2 { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("link2_public")]
        public string? Link2Public { get; set; }

        [XmlAttribute("link3")]
        public string? Link3 { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("link3_public")]
        public string? Link3Public { get; set; }

        [XmlAttribute("region")]
        public string? Region { get; set; }

        [XmlAttribute("media_title")]
        public string? MediaTitle { get; set; }
    }
}
