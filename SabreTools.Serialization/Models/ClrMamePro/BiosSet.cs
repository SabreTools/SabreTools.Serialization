namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>biosset</remarks>
    public class BiosSet
    {
        /// <remarks>name</remarks>
        [SabreTools.Models.Required]
        public string? Name { get; set; }

        /// <remarks>description</remarks>
        [SabreTools.Models.Required]
        public string? Description { get; set; }

        /// <remarks>default</remarks>
        public string? Default { get; set; }
    }
}