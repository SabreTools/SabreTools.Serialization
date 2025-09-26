namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// MD4 File
    /// </summary>
    public class MD4
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}