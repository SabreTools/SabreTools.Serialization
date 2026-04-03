using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("video"), XmlRoot("video")]
    public class Video : DatItem
    {
        #region Properties

        public long? AspectX { get; set; }

        public long? AspectY { get; set; }

        /// <remarks>Originally "y"</remarks>
        public long? Height { get; set; }

        /// <remarks>Originally "freq"</remarks>
        public double? Refresh { get; set; }

        /// <remarks>(raster|vector)</remarks>
        public DisplayType? Screen { get; set; }

        /// <remarks>Originally "x"</remarks>
        public long? Width { get; set; }

        #endregion

        #region Keys

        /// <remarks>(vertical|horizontal)</remarks>
        public const string OrientationKey = "orientation";

        #endregion

        public Video() => ItemType = ItemType.Video;
    }
}
