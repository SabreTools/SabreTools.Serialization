
namespace SabreTools.Data.Models.Hashfile
{
    /// <summary>
    /// BLAKE3 File
    /// </summary>
    public class BLAKE3
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}
