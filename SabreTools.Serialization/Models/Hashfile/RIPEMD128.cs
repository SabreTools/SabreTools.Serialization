using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// RIPEMD128 File
    /// </summary>
    public class RIPEMD128
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}