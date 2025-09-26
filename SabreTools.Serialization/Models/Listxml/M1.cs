using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("m1")]
    public class M1
    {
        [XmlAttribute("version")]
        public string? Version { get; set; }

        [XmlElement("machine", typeof(Machine))]
        [XmlElement("game", typeof(Game))]
        public GameBase[]? Game { get; set; }
    }
}