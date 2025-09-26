namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SHA-512 File
    /// </summary>
    public class SHA512
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}