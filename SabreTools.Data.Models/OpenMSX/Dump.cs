using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OpenMSX
{
    [XmlRoot("dump")]
    public class Dump
    {
        [XmlElement("boot")]
        public string? Boot { get; set; }

        [XmlElement("original")]
        public Original? Original { get; set; }

        [XmlElement("rom", typeof(Rom))]
        [XmlElement("megarom", typeof(MegaRom))]
        [XmlElement("sccpluscart", typeof(SCCPlusCart))]
        public RomBase? Rom { get; set; }
    }
}
