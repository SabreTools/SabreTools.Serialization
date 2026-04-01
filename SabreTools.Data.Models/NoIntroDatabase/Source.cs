using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("source")]
    public class Source
    {
        [Required]
        [XmlElement("details", typeof(SourceDetails))]
        public SourceDetails? Details { get; set; }

        [XmlElement("serials", typeof(Serials))]
        public Serials? Serials { get; set; }

        [XmlElement("file", typeof(File))]
        public File[]? File { get; set; }
    }
}
