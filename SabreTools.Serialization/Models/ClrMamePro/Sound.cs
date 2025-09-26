using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>sound</remarks>
    public class Sound
    {
        /// <remarks>channels, Numeric?</remarks>
        [Required]
        public string? Channels { get; set; }
    }
}