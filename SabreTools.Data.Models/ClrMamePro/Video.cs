
using SabreTools.Data.Models.Metadata;

namespace SabreTools.Data.Models.ClrMamePro
{
    /// <remarks>video</remarks>
    public class Video
    {
        /// <remarks>screen, (raster|vector)</remarks>
        [Required]
        public DisplayType? Screen { get; set; }

        /// <remarks>orientation, (vertical|horizontal)</remarks>
        [Required]
        public Rotation? Orientation { get; set; }

        /// <remarks>x</remarks>
        public long? X { get; set; }

        /// <remarks>y</remarks>
        public long? Y { get; set; }

        /// <remarks>aspectx</remarks>
        public long? AspectX { get; set; }

        /// <remarks>aspecty</remarks>
        public long? AspectY { get; set; }

        /// <remarks>freq</remarks>
        public double? Freq { get; set; }
    }
}
