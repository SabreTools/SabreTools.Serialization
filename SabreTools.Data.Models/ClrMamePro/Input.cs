
namespace SabreTools.Data.Models.ClrMamePro
{
    /// <remarks>input</remarks>
    public class Input
    {
        /// <remarks>"players"/remarks>
        [Required]
        public long? Players { get; set; }

        /// <remarks>"control"</remarks>
        public string? Control { get; set; }

        /// <remarks>"buttons"</remarks>
        [Required]
        public long? Buttons { get; set; }

        /// <remarks>"coins"</remarks>
        public long? Coins { get; set; }

        /// <remarks>"tilt", (yes|no) "no"</remarks>
        public bool? Tilt { get; set; }

        /// <remarks>"service", (yes|no) "no"</remarks>
        public bool? Service { get; set; }
    }
}
