using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("files")]
    public class Files
    {
        [XmlElement("romCRC")]
        public FileRomCRC[]? RomCRC { get; set; }
    }
}