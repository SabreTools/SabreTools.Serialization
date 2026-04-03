using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("video"), XmlRoot("video")]
    public class Video : DatItem
    {
        #region Properties

        /// <remarks>Originally "freq"</remarks>
        public double? Refresh { get; set; }

        /// <remarks>(raster|vector)</remarks>
        public DisplayType? Screen { get; set; }

        #endregion

        #region Keys

        /// <remarks>long</remarks>
        public const string AspectXKey = "aspectx";

        /// <remarks>long</remarks>
        public const string AspectYKey = "aspecty";

        /// <remarks>long; Originally "y"</remarks>
        public const string HeightKey = "height";

        /// <remarks>(vertical|horizontal)</remarks>
        public const string OrientationKey = "orientation";

        /// <remarks>long; Originally "x"</remarks>
        public const string WidthKey = "width";

        #endregion

        public Video() => ItemType = ItemType.Video;
    }
}
