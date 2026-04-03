using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("video")]
    public class Video
    {
        /// <remarks>(raster|vector)</remarks>
        [Required]
        [XmlAttribute("screen")]
        public DisplayType? Screen { get; set; }

        /// <remarks>(vertical|horizontal)</remarks>
        [Required]
        [XmlAttribute("orientation")]
        public Rotation? Orientation { get; set; }

        [XmlAttribute("width")]
        public long? Width { get; set; }

        [XmlAttribute("height")]
        public long? Height { get; set; }

        [XmlAttribute("aspectx")]
        public long? AspectX { get; set; }

        [XmlAttribute("aspecty")]
        public long? AspectY { get; set; }

        [XmlAttribute("refresh")]
        public double? Refresh { get; set; }
    }
}
