using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.ArchiveDotOrg
{
    [XmlRoot("files")]
    public class Files
    {
        [XmlElement("file")]
        public File[]? File { get; set; }
    }
}