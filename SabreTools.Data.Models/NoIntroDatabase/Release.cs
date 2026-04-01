using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("release")]
    public class Release
    {
        [Required]
        [XmlElement("details", typeof(ReleaseDetails))]
        public ReleaseDetails? Details { get; set; }

        [XmlElement("serials", typeof(Serials))]
        public Serials? Serials { get; set; }

        [XmlElement("file", typeof(File))]
        public File[]? File { get; set; }
    }
}
