using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("files")]
    public class Files
    {
        [XmlElement("romCRC")]
        public FileRomCRC[]? RomCRC { get; set; }
    }
}