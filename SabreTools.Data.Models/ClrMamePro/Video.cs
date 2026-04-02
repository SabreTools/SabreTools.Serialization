
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
        public string? Orientation { get; set; }

        /// <remarks>x, Numeric?</remarks>
        public string? X { get; set; }

        /// <remarks>y, Numeric?</remarks>
        public string? Y { get; set; }

        /// <remarks>aspectx, Numeric?</remarks>
        public string? AspectX { get; set; }

        /// <remarks>aspecty, Numeric?</remarks>
        public string? AspectY { get; set; }

        /// <remarks>freq, Numeric?</remarks>
        public string? Freq { get; set; }
    }
}
