using System.Xml;
using System.Xml.Serialization;
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.Listxml
{
    [XmlRoot("display")]
    public class Display
    {
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        /// <remarks>(raster|vector|lcd|svg|unknown)</remarks>
        [Required]
        [XmlAttribute("type")]
        public DisplayType? Type { get; set; }

        /// <remarks>(0|90|180|270)</remarks>
        [XmlAttribute("rotate")]
        public string? Rotate { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("flipx")]
        public bool? FlipX { get; set; }

        [XmlAttribute("width")]
        public long? Width { get; set; }

        [XmlAttribute("height")]
        public long? Height { get; set; }

        [Required]
        [XmlAttribute("refresh")]
        public double? Refresh { get; set; }

        [XmlAttribute("pixclock")]
        public long? PixClock { get; set; }

        [XmlAttribute("htotal")]
        public long? HTotal { get; set; }

        [XmlAttribute("hbend")]
        public long? HBEnd { get; set; }

        [XmlAttribute("hbstart")]
        public long? HBStart { get; set; }

        [XmlAttribute("vtotal")]
        public long? VTotal { get; set; }

        [XmlAttribute("vbend")]
        public long? VBEnd { get; set; }

        [XmlAttribute("vbstart")]
        public long? VBStart { get; set; }
    }
}
