namespace SabreTools.Serialization.Models.GameHeader
{
    /// <summary>
    /// Encrypted Data section for an NDS cart image
    /// </summary>
    public sealed class NitroEncryptedData
    {
        public ushort EncryptedSecure { get; set; }

        // Hex string, no prefix
        public string? EncryptedCRC32 { get; set; }

        // Hex string, no prefix
        public string? EncryptedMD5 { get; set; }

        // Hex string, no prefix
        public string? EncryptedSHA1 { get; set; }

        // Hex string, no prefix
        public string? EncryptedSHA256 { get; set; }
    }
}