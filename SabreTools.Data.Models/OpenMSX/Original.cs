using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Data.Models.OpenMSX
{
    [XmlRoot("original")]
    public class Original
    {
        [XmlAttribute("value")]
        public bool? Value { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}
