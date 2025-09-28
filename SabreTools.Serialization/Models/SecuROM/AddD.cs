namespace SabreTools.Data.Models.SecuROM
{
    /// <summary>
    /// Overlay data associated with SecuROM executables
    /// </summary>
    /// <remarks>
    /// All information in this file has been researched in a clean room
    /// environment by using sample from legally obtained software that
    /// is protected by SecuROM.
    /// </remarks>
    public sealed class AddD
    {
        /// <summary>
        /// "AddD"
        /// </summary>
        public string? Signature { get; set; }

        /// <summary>s
        /// Entry count
        /// </summary>
        public uint EntryCount { get; set; }

        /// <summary>
        /// Version, null-terminated
        /// </summary>
        /// <remarks>Always 8 bytes?</remarks>
        public string? Version { get; set; }

        /// <summary>
        /// Build number
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public string? Build { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>44 bytes</remarks>
        public byte[]? Unknown1 { get; set; }

        /// <summary>
        /// Product ID?
        /// </summary>
        /// <remarks>
        /// 10 bytes, only present in 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968, 4.85.07.0009
        /// </remarks>
        public string? ProductId { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// 58 bytes, only present in 4.84.00.0054, 4.84.69.0037, 4.84.76.7966, 4.84.76.7968, 4.85.07.0009
        /// </remarks>
        public byte[]? Unknown2 { get; set; }

        /// <summary>
        /// Entry table
        /// </summary>
        public AddDEntry[]? Entries { get; set; }
    }
}
