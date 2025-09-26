using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("dat")]
    public class Dat
    {
        [XmlAttribute(Namespace = "http://www.w3.org/2001/XMLSchema-instance", AttributeName = "noNamespaceSchemaLocation")]
        public string? NoNamespaceSchemaLocation { get; set; }

        [XmlElement("configuration")]
        public Configuration? Configuration { get; set; }

        [XmlElement("games")]
        public Games? Games { get; set; }

        [XmlElement("gui")]
        public GUI? GUI { get; set; }
    }
}