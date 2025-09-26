using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.ClrMamePro
{
    /// <remarks>sound</remarks>
    public class Sound
    {
        /// <remarks>channels, Numeric?</remarks>
        [Required]
        public string? Channels { get; set; }
    }
}