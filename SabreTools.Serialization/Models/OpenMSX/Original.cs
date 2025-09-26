using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization.Models.OpenMSX
{
    [XmlRoot("original")]
    public class Original
    {
        /// <remarks>Boolean?</remarks>
        [XmlAttribute("value")]
        public string? Value { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}