using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("canOpen")]
    public class CanOpen
    {
        [XmlElement("extension")]
        public string[]? Extension { get; set; }
    }
}