using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("display"), XmlRoot("display")]
    public class Display : DatItem
    {
        #region Properties

        /// <remarks>(raster|vector|lcd|svg|unknown)</remarks>
        public DisplayType? DisplayType { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? FlipX { get; set; }

        public double? Refresh { get; set; }

        public string? Tag { get; set; }

        #endregion

        #region Keys

        /// <remarks>long</remarks>
        public const string HBEndKey = "hbend";

        /// <remarks>long</remarks>
        public const string HBStartKey = "hbstart";

        /// <remarks>long</remarks>
        public const string HeightKey = "height";

        /// <remarks>long</remarks>
        public const string HTotalKey = "htotal";

        /// <remarks>long</remarks>
        public const string PixClockKey = "pixclock";

        /// <remarks>(0|90|180|270)</remarks>
        /// TODO: Convert to enum
        public const string RotateKey = "rotate";

        /// <remarks>long</remarks>
        public const string VBEndKey = "vbend";

        /// <remarks>long</remarks>
        public const string VBStartKey = "vbstart";

        /// <remarks>long</remarks>
        public const string VTotalKey = "vtotal";

        /// <remarks>long</remarks>
        public const string WidthKey = "width";

        #endregion

        public Display() => ItemType = ItemType.Display;
    }
}
