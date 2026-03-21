using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("video"), XmlRoot("video")]
    public class Video : DatItem
    {
        #region Keys

        /// <remarks>long</remarks>
        public const string AspectXKey = "aspectx";

        /// <remarks>long</remarks>
        public const string AspectYKey = "aspecty";

        /// <remarks>long; Originally "y"</remarks>
        public const string HeightKey = "height";

        /// <remarks>(vertical|horizontal)</remarks>
        public const string OrientationKey = "orientation";

        /// <remarks>double; Originally "freq"</remarks>
        public const string RefreshKey = "refresh";

        /// <remarks>(raster|vector)</remarks>
        public const string ScreenKey = "screen";

        /// <remarks>long; Originally "x"</remarks>
        public const string WidthKey = "width";

        #endregion

        public Video() => Type = ItemType.Video;
    }
}
