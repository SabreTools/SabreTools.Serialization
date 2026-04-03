using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("display"), XmlRoot("display")]
    public class Display : DatItem
    {
        #region Properties

        /// <remarks>Only found in Video</remarks>
        public long? AspectX { get; set; }

        /// <remarks>Only found in Video</remarks>
        public long? AspectY { get; set; }

        /// <remarks>(raster|vector|lcd|svg|unknown)</remarks>
        public DisplayType? DisplayType { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? FlipX { get; set; }

        public long? HBEnd { get; set; }

        public long? HBStart { get; set; }

        public long? Height { get; set; }

        public long? HTotal { get; set; }

        public long? PixClock { get; set; }

        public double? Refresh { get; set; }

        public string? Tag { get; set; }

        public long? VBEnd { get; set; }

        public long? VBStart { get; set; }

        public long? VTotal { get; set; }

        public long? Width { get; set; }

        #endregion

        #region Keys

        /// <remarks>(0|90|180|270)</remarks>
        /// TODO: Convert to enum
        public const string RotateKey = "rotate";

        #endregion

        public Display() => ItemType = ItemType.Display;
    }
}
