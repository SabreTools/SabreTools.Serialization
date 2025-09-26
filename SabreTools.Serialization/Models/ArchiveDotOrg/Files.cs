using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.ArchiveDotOrg
{
    [XmlRoot("files")]
    public class Files
    {
        [XmlElement("file")]
        public File[]? File { get; set; }
    }
}