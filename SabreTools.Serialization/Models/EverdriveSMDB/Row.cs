namespace SabreTools.Serialization.Models.EverdriveSMDB
{
    /// <summary>
    /// SHA-256 \t Machine Name/Filename \t SHA-1 \t MD5 \t CRC32 \t Size (Optional)
    /// </summary>
    public class Row
    {
        [SabreTools.Models.Required]
        public string? SHA256 { get; set; }

        [SabreTools.Models.Required]
        public string? Name { get; set; }

        [SabreTools.Models.Required]
        public string? SHA1 { get; set; }

        [SabreTools.Models.Required]
        public string? MD5 { get; set; }

        [SabreTools.Models.Required]
        public string? CRC32 { get; set; }

        public string? Size { get; set; }
    }
}