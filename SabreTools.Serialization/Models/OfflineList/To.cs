using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OfflineList
{
    [XmlRoot("to")]
    public class To
    {
        [XmlAttribute("value")]
        public string? Value { get; set; }

        /// <remarks>Boolean</remarks>
        [XmlAttribute("default")]
        public string? Default { get; set; }

        /// <remarks>Boolean</remarks>
        [XmlAttribute("auto")]
        public string? Auto { get; set; }

        [XmlElement("find")]
        public Find[]? Find { get; set; }
    }
}