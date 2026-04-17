namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// A single hash exception entry within a WIA / RVZ Wii-partition group.
    /// Used to restore the correct SHA-1 hash values that were stripped when
    /// the Wii block hash data was removed during compression.
    /// Size: 0x16 bytes (2-byte offset + 20-byte SHA-1).
    /// </summary>
    public sealed class HashExceptionEntry
    {
        /// <summary>
        /// Byte offset within the reconstructed 0x400-byte hash block where
        /// this SHA-1 value must be written
        /// </summary>
        public ushort Offset { get; set; }

        /// <summary>
        /// SHA-1 hash value (20 bytes)
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] Hash { get; set; } = new byte[20];
    }
}
