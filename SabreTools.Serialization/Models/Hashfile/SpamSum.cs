namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// SpamSum File
    /// </summary>
    public class SpamSum
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}