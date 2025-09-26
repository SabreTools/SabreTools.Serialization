using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("device_ref")]
    public class DeviceRef
    {
        [Required]
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}