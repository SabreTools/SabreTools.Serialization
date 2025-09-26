using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SHA-512 File
    /// </summary>
    public class SHA512
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}