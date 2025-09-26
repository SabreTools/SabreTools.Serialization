using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("search")]
    public class Search
    {
        [XmlElement("to")]
        public To[]? To { get; set; }
    }
}