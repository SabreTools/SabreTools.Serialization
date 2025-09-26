using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.Logiqx
{
    [XmlRoot("trurip")]
    public class Trurip
    {
        [XmlElement("titleid")]
        public string? TitleID { get; set; }

        [XmlElement("publisher")]
        public string? Publisher { get; set; }

        [XmlElement("developer")]
        public string? Developer { get; set; }

        [XmlElement("year")]
        public string? Year { get; set; }

        [XmlElement("genre")]
        public string? Genre { get; set; }

        [XmlElement("subgenre")]
        public string? Subgenre { get; set; }

        [XmlElement("ratings")]
        public string? Ratings { get; set; }

        [XmlElement("score")]
        public string? Score { get; set; }

        [XmlElement("players")]
        public string? Players { get; set; }

        /// <remarks>Boolean?</remarks>
        [XmlElement("enabled")]
        public string? Enabled { get; set; }

        [XmlElement("crc")]
        public string? CRC { get; set; }

        [XmlElement("source")]
        public string? Source { get; set; }

        [XmlElement("cloneof")]
        public string? CloneOf { get; set; }

        [XmlElement("relatedto")]
        public string? RelatedTo { get; set; }
    }
}