using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// RIPEMD160 File
    /// </summary>
    public class RIPEMD160
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}