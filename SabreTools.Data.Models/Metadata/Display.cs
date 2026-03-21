using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("display"), XmlRoot("display")]
    public class Display : DatItem
    {
        #region Keys

        /// <remarks>(yes|no) "no"</remarks>
        public const string FlipXKey = "flipx";

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

        /// <remarks>double</remarks>
        public const string RefreshKey = "refresh";

        /// <remarks>(0|90|180|270)</remarks>
        public const string RotateKey = "rotate";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        /// <remarks>(raster|vector|lcd|svg|unknown)</remarks>
        public const string DisplayTypeKey = "type";

        /// <remarks>long</remarks>
        public const string VBEndKey = "vbend";

        /// <remarks>long</remarks>
        public const string VBStartKey = "vbstart";

        /// <remarks>long</remarks>
        public const string VTotalKey = "vtotal";

        /// <remarks>long</remarks>
        public const string WidthKey = "width";

        #endregion

        public Display() => Type = ItemType.Display;
    }
}
