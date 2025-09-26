using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SHA-384 File
    /// </summary>
    public class SHA384
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}