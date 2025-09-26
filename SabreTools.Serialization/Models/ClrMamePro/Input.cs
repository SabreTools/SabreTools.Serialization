namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>input</remarks>
    public class Input
    {
        /// <remarks>players, Numeric/remarks>
        [SabreTools.Models.Required]
        public string? Players { get; set; }

        /// <remarks>control</remarks>
        public string? Control { get; set; }

        /// <remarks>buttons, Numeric</remarks>
        [SabreTools.Models.Required]
        public string? Buttons { get; set; }

        /// <remarks>coins, Numeric</remarks>
        public string? Coins { get; set; }

        /// <remarks>tilt, (yes|no) "no"</remarks>
        public string? Tilt { get; set; }

        /// <remarks>service, (yes|no) "no"</remarks>
        public string? Service { get; set; }
    }
}