namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// File CRC
    /// </summary>
    public class SFV
    {
        [SabreTools.Models.Required]
        public string? File { get; set; }

        [SabreTools.Models.Required]
        public string? Hash { get; set; }
    }
}