namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>release</remarks>
    public class Release
    {
        /// <remarks>name</remarks>
        [SabreTools.Models.Required]
        public string? Name { get; set; }

        /// <remarks>region</remarks>
        [SabreTools.Models.Required]
        public string? Region { get; set; }

        /// <remarks>language</remarks>
        public string? Language { get; set; }

        /// <remarks>date</remarks>
        public string? Date { get; set; }

        /// <remarks>default</remarks>
        public string? Default { get; set; }
    }
}