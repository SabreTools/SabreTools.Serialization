namespace SabreTools.Serialization.Models.Hashfile
{
    /// <summary>
    /// RIPEMD128 File
    /// </summary>
    public class RIPEMD128
    {
        [SabreTools.Models.Required]
        public string? Hash { get; set; }

        [SabreTools.Models.Required]
        public string? File { get; set; }
    }
}