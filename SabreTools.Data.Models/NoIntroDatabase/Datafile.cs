using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.NoIntroDatabase
{
    [XmlRoot("datafile")]
    public class Datafile
    {
        [XmlElement("game", typeof(Game))]
        public Game[]? Game { get; set; }
    }
}
