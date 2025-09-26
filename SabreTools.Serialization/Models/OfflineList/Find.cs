using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OfflineList
{
    [XmlRoot("find")]
    public class Find
    {
        [XmlAttribute("operation")]
        public string? Operation { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("value")]
        public string? Value { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}