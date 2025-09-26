using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Hashfile
{
    /// <summary>
    /// File CRC
    /// </summary>
    public class SFV
    {
        [Required]
        public string? File { get; set; }

        [Required]
        public string? Hash { get; set; }
    }
}