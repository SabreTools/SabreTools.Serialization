namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>driver</remarks>
    public class Driver
    {
        /// <remarks>status, (good|imperfect|preliminary)</remarks>
        [Required]
        public string? Status { get; set; }

        /// <remarks>color, (good|imperfect|preliminary)</remarks>
        public string? Color { get; set; }

        /// <remarks>sound, (good|imperfect|preliminary)</remarks>
        public string? Sound { get; set; }

        /// <remarks>palettesize, Numeric?</remarks>
        public string? PaletteSize { get; set; }

        /// <remarks>blit, (plain|dirty)</remarks>
        public string? Blit { get; set; }
    }
}