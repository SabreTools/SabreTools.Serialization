namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// RIPEMD160 File
    /// </summary>
    public class RIPEMD160
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}