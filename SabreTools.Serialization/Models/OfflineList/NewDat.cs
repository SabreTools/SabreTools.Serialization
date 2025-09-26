using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("newDat")]
    public class NewDat
    {
        [XmlElement("datVersionURL")]
        public string? DatVersionUrl { get; set; }

        [XmlElement("datURL")]
        public DatUrl? DatUrl { get; set; }

        [XmlElement("imURL")]
        public string? ImUrl { get; set; }
    }
}