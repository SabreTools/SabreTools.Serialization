using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("archive")]
    public class Archive
    {
        [XmlAttribute("number")]
        public string? Number { get; set; }

        [XmlAttribute("clone")]
        public string? Clone { get; set; }

        [XmlAttribute("regparent")]
        public string? RegParent { get; set; }

        [XmlAttribute("mergeof")]
        public string? MergeOf { get; set; }

        [XmlAttribute("mergename")]
        public string? MergeName { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("name_alt")]
        public string? NameAlt { get; set; }

        [XmlAttribute("region")]
        public string? Region { get; set; }

        [XmlAttribute("languages")]
        public string? Languages { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("showlang")]
        public string? ShowLang { get; set; }

        [XmlAttribute("langchecked")]
        public string? LangChecked { get; set; }

        [XmlAttribute("version1")]
        public string? Version1 { get; set; }

        [XmlAttribute("version2")]
        public string? Version2 { get; set; }

        [XmlAttribute("devstatus")]
        public string? DevStatus { get; set; }

        [XmlAttribute("additional")]
        public string? Additional { get; set; }

        [XmlAttribute("special1")]
        public string? Special1 { get; set; }

        [XmlAttribute("special2")]
        public string? Special2 { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("alt")]
        public string? Alt { get; set; }

        [XmlAttribute("gameid1")]
        public string? GameId1 { get; set; }

        [XmlAttribute("gameid2")]
        public string? GameId2 { get; set; }

        [XmlAttribute("description")]
        public string? Description { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("bios")]
        public string? Bios { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("licensed")]
        public string? Licensed { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("pirate")]
        public string? Pirate { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("physical")]
        public string? Physical { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("complete")]
        public string? Complete { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("adult")]
        public string? Adult { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("dat")]
        public string? Dat { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("listed")]
        public string? Listed { get; set; }

        /// <remarks>byte</remarks>
        [XmlAttribute("private")]
        public string? Private { get; set; }

        [XmlAttribute("sticky_note")]
        public string? StickyNote { get; set; }

        [XmlAttribute("datter_note")]
        public string? DatterNote { get; set; }

        [XmlAttribute("categories")]
        public string? Categories { get; set; }
    }
}
