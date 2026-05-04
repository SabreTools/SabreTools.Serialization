namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// Describes a single Wii partition: its AES title key and two sector ranges.
    /// Size: 0x30 bytes.
    /// </summary>
    public sealed class PartitionEntry
    {
        /// <summary>
        /// Decrypted AES-128 partition title key
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] PartitionKey { get; set; } = new byte[16];

        /// <summary>First sector range for this partition (typically encrypted data)</summary>
        public PartitionDataEntry DataEntry0 { get; set; } = new();

        /// <summary>Second sector range for this partition (typically decrypted/raw data)</summary>
        public PartitionDataEntry DataEntry1 { get; set; } = new();
    }
}
