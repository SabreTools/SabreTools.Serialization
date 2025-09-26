using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.Logiqx
{
    [XmlRoot("datafile")]
    public class Datafile
    {
        [XmlAttribute("build")]
        public string? Build { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("debug")]
        public string? Debug { get; set; }

        /// <remarks>No-Intro extension</remarks>
        [XmlAttribute(Namespace = "http://www.w3.org/2001/XMLSchema-instance", AttributeName = "schemaLocation")]
        public string? SchemaLocation { get; set; }

        [XmlElement("header")]
        public Header? Header { get; set; }

        [XmlElement("game", typeof(Game))]
        [XmlElement("machine", typeof(Machine))]
        public GameBase[]? Game { get; set; }

        /// <remarks>RomVault extension</remarks>
        [XmlElement("dir")]
        public Dir[]? Dir { get; set; }
    }
}