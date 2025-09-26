using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("romCRC")]
    public class FileRomCRC
    {
        [XmlAttribute("extension")]
        public string? Extension { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}