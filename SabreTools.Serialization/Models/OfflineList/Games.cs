using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("games")]
    public class Games
    {
        [XmlElement("game")]
        public Game[]? Game { get; set; }
    }
}