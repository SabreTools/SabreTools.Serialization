using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.ClrMamePro
{
    /// <remarks>archive</remarks>
    public class Archive
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }
    }
}