namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// WIA / RVZ first header (0x48 bytes at the start of the file).
    /// All multi-byte fields are big-endian on disk; the reader converts to host order.
    /// </summary>
    /// <see href="https://github.com/dolphin-emu/dolphin/blob/master/Source/Core/DiscIO/WIABlob.h"/>
    public sealed class WiaHeader1
    {
        /// <summary>
        /// Format magic: 0x01414957 ("WIA\x01") or 0x015A5652 ("RVZ\x01")
        /// </summary>
        public uint Magic { get; set; }

        /// <summary>
        /// Format version (e.g. 0x01000000)
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Minimum version required to read this file
        /// </summary>
        public uint VersionCompatible { get; set; }

        /// <summary>
        /// Size of WiaHeader2 in bytes
        /// </summary>
        public uint Header2Size { get; set; }

        /// <summary>
        /// SHA-1 hash of WiaHeader2
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] Header2Hash { get; set; } = new byte[20];

        /// <summary>
        /// Total size of the equivalent uncompressed ISO image in bytes
        /// </summary>
        public ulong IsoFileSize { get; set; }

        /// <summary>
        /// Total size of this WIA / RVZ file in bytes
        /// </summary>
        public ulong WiaFileSize { get; set; }

        /// <summary>
        /// SHA-1 hash of this header, excluding this field itself
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] Header1Hash { get; set; } = new byte[20];
    }
}
