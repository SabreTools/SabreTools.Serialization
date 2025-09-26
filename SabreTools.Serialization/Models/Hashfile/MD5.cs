namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// MD5 File
    /// </summary>
    public class MD5
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}