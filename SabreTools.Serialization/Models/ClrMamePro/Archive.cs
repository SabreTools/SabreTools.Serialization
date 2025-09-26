using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>archive</remarks>
    public class Archive
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }
    }
}