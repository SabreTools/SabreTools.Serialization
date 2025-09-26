namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// MD5 File
    /// </summary>
    public class MD5
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}