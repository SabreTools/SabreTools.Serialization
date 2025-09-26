using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("gui")]
    public class GUI
    {
        [XmlElement("images")]
        public Images? Images { get; set; }
    }
}