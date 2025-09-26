namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SHA-1 File
    /// </summary>
    public class SHA1
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}