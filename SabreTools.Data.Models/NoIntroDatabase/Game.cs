using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("game")]
    public class Game
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlElement("archive", typeof(Archive))]
        public Archive? Archive { get; set; }

        [XmlElement("media", typeof(Media))]
        public Media[]? Media { get; set; }

        [XmlElement("source", typeof(Source))]
        public Source[]? Source { get; set; }

        [XmlElement("release", typeof(Release))]
        public Release[]? Release { get; set; }
    }
}
