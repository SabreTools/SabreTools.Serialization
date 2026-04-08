using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("video"), XmlRoot("video")]
    public class Video : DatItem, ICloneable, IEquatable<Display>, IEquatable<Video>
    {
        #region Properties

        public long? AspectX { get; set; }

        public long? AspectY { get; set; }

        /// <remarks>Originally "y"</remarks>
        public long? Height { get; set; }

        /// <remarks>(vertical|horizontal)</remarks>
        public Rotation? Orientation { get; set; }

        /// <remarks>Originally "freq"</remarks>
        public double? Refresh { get; set; }

        /// <remarks>(raster|vector)</remarks>
        public DisplayType? Screen { get; set; }

        /// <remarks>Originally "x"</remarks>
        public long? Width { get; set; }

        #endregion

        public Video() => ItemType = ItemType.Video;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Video();

            obj.AspectX = AspectX;
            obj.AspectY = AspectY;
            obj.Height = Height;
            obj.Orientation = Orientation;
            obj.Refresh = Refresh;
            obj.Screen = Screen;
            obj.Width = Width;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Display? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (AspectX != other.AspectX)
                return false;

            if (AspectY != other.AspectY)
                return false;

            if (Height != other.Height)
                return false;

            if (Orientation != other.Rotate)
                return false;

            if (Refresh != other.Refresh)
                return false;

            if (Screen != other.DisplayType)
                return false;

            if (Width != other.Width)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public bool Equals(Video? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (AspectX != other.AspectX)
                return false;

            if (AspectY != other.AspectY)
                return false;

            if (Height != other.Height)
                return false;

            if (Orientation != other.Orientation)
                return false;

            if (Refresh != other.Refresh)
                return false;

            if (Screen != other.Screen)
                return false;

            if (Width != other.Width)
                return false;

            return true;
        }
    }
}
