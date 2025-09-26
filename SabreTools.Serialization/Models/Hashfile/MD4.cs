using SabreTools.Data.Attributes;

namespace SabreTools.Data.Models.Hashfile
{
    /// <summary>
    /// MD4 File
    /// </summary>
    public class MD4
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}