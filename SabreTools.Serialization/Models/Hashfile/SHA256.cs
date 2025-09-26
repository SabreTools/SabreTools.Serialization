namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SHA-256 File
    /// </summary>
    public class SHA256
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}