using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Listxml
{
    [XmlRoot("video")]
    public class Video
    {
        /// <remarks>(raster|vector)</remarks>
        [Required]
        [XmlAttribute("screen")]
        public string? Screen { get; set; }

        /// <remarks>(vertical|horizontal)</remarks>
        [Required]
        [XmlAttribute("orientation")]
        public string? Orientation { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("width")]
        public string? Width { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("height")]
        public string? Height { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("aspectx")]
        public string? AspectX { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("aspecty")]
        public string? AspectY { get; set; }

        /// <remarks>Numeric</remarks>
        [XmlAttribute("refresh")]
        public string? Refresh { get; set; }
    }
}