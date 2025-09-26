using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Hashfile
{
    /// <summary>
    /// SHA-256 File
    /// </summary>
    public class SHA256
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}