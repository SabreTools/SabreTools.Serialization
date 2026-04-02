namespace SabreTools.Data.Models.ClrMamePro
{
    /// <remarks>biosset</remarks>
    public class BiosSet
    {
        /// <remarks>"name"</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>"description"</remarks>
        [Required]
        public string? Description { get; set; }

        /// <remarks>"default", (yes|no) "no"</remarks>
        public bool? Default { get; set; }
    }
}
