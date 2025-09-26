using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.ClrMamePro
{
    /// <remarks>biosset</remarks>
    public class BiosSet
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>description</remarks>
        [Required]
        public string? Description { get; set; }

        /// <remarks>default</remarks>
        public string? Default { get; set; }
    }
}