namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>chip</remarks>
    public class Chip
    {
        /// <remarks>type, (cpu|audio)</remarks>
        [SabreTools.Models.Required]
        public string? Type { get; set; }

        /// <remarks>name</remarks>
        [SabreTools.Models.Required]
        public string? Name { get; set; }

        /// <remarks>flags</remarks>
        public string? Flags { get; set; }

        /// <remarks>clock, Numeric?</remarks>
        public string? Clock { get; set; }
    }
}