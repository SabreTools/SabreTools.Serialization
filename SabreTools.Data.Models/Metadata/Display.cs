using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("display"), XmlRoot("display")]
    public class Display : DatItem, ICloneable, IEquatable<Display>, IEquatable<Video>
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

        /// <remarks>(0|90|180|270)</remarks>
        public Rotation? Rotate { get; set; }

        public string? Tag { get; set; }

        public long? VBEnd { get; set; }

        public long? VBStart { get; set; }

        public long? VTotal { get; set; }

        public long? Width { get; set; }

        #endregion

        public Display() => ItemType = ItemType.Display;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Display();

            obj.AspectX = AspectX;
            obj.AspectY = AspectY;
            obj.DisplayType = DisplayType;
            obj.FlipX = FlipX;
            obj.HBEnd = HBEnd;
            obj.HBStart = HBStart;
            obj.Height = Height;
            obj.HTotal = HTotal;
            obj.PixClock = PixClock;
            obj.Refresh = Refresh;
            obj.Rotate = Rotate;
            obj.Tag = Tag;
            obj.VBEnd = VBEnd;
            obj.VBStart = VBStart;
            obj.VTotal = VTotal;
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
            if (DisplayType != other.DisplayType)
                return false;

            if (FlipX != other.FlipX)
                return false;

            if (HBEnd != other.HBEnd)
                return false;

            if (HBStart != other.HBStart)
                return false;

            if (Height != other.Height)
                return false;

            if (HTotal != other.HTotal)
                return false;

            if (PixClock != other.PixClock)
                return false;

            if (Refresh != other.Refresh)
                return false;

            if (Rotate != other.Rotate)
                return false;

            if ((Tag is null) ^ (other.Tag is null))
                return false;
            else if (Tag is not null && !Tag.Equals(other.Tag, StringComparison.OrdinalIgnoreCase))
                return false;

            if (VBEnd != other.VBEnd)
                return false;

            if (VBStart != other.VBStart)
                return false;

            if (VTotal != other.VTotal)
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

            if (DisplayType != other.Screen)
                return false;

            if (Height != other.Height)
                return false;

            if (Refresh != other.Refresh)
                return false;

            if (Rotate != other.Orientation)
                return false;

            if (Width != other.Width)
                return false;

            return true;
        }
    }
}
