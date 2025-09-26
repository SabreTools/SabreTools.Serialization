using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("games")]
    public class Games
    {
        [XmlElement("game")]
        public Game[]? Game { get; set; }
    }
}