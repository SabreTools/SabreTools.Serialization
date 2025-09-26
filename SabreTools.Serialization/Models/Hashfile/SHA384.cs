namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SHA-384 File
    /// </summary>
    public class SHA384
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}