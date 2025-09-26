namespace SabreTools.Data.Models.GameHeader
{
    /// <summary>
    /// File Data section for all files
    /// </summary>
    public sealed class FileData
    {
        public string? System { get; set; }

        public string? Path { get; set; }

        public string? Archive { get; set; }

        public string? File { get; set; }

        public string? BitSize { get; set; }

        public long SizeBytes { get; set; }

        // Hex string, no prefix
        public string? CRC32 { get; set; }

        // Hex string, no prefix
        public string? MD5 { get; set; }

        // Hex string, no prefix
        public string? SHA1 { get; set; }

        // Hex string, no prefix
        public string? SHA256 { get; set; }
    }
}