namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// GameCube / Wii disc boot block header (first 0x440 bytes of the disc)
    /// </summary>
    /// <see href="https://wiibrew.org/wiki/Wii_disc"/>
    public sealed class DiscHeader
    {
        /// <summary>
        /// 6-character ASCII game ID (e.g. "GALE01")
        /// </summary>
        /// <remarks>6 bytes at offset 0x000</remarks>
        public string GameId { get; set; } = string.Empty;

        /// <summary>
        /// Zero-based disc number for multi-disc games
        /// </summary>
        public byte DiscNumber { get; set; }

        /// <summary>
        /// Disc version
        /// </summary>
        public byte DiscVersion { get; set; }

        /// <summary>
        /// Non-zero if audio streaming is enabled
        /// </summary>
        public byte AudioStreaming { get; set; }

        /// <summary>
        /// Audio streaming buffer size (in 16 KiB units)
        /// </summary>
        public byte StreamingBufferSize { get; set; }

        /// <summary>
        /// Unused 0x0E bytes (offsets 0x00A-0x017)
        /// </summary>
        public byte[] Padding00A { get; set; } = [];

        /// <summary>
        /// Wii magic word at offset 0x018 (0x5D1C9EA3 for Wii discs, 0 for GameCube)
        /// </summary>
        public uint WiiMagic { get; set; }

        /// <summary>
        /// GameCube magic word at offset 0x01C (0xC2339F3D for GameCube discs)
        /// </summary>
        public uint GCMagic { get; set; }

        /// <summary>
        /// Null-terminated ASCII game title (up to 0x60 bytes at offset 0x020)
        /// </summary>
        public string GameTitle { get; set; } = string.Empty;

        /// <summary>
        /// Non-zero to disable hash verification (GameCube only)
        /// </summary>
        public byte DisableHashVerification { get; set; }

        /// <summary>
        /// Non-zero to disable disc encryption (GameCube only)
        /// </summary>
        public byte DisableDiscEncryption { get; set; }

        /// <summary>
        /// Unused padding until DOL/FST offset fields at 0x420
        /// </summary>
        public byte[] Padding082 { get; set; } = [];

        /// <summary>
        /// Offset of the main DOL executable (no shift for GameCube; &lt;&lt;2 for Wii)
        /// </summary>
        public uint DolOffset { get; set; }

        /// <summary>
        /// Offset of the File System Table (no shift for GameCube; &lt;&lt;2 for Wii)
        /// </summary>
        public uint FstOffset { get; set; }

        /// <summary>
        /// Maximum size of the File System Table in bytes
        /// </summary>
        public uint FstSize { get; set; }

        /// <summary>
        /// Remaining bytes to complete the 0x440 header
        /// </summary>
        public byte[] Padding42C { get; set; } = [];
    }
}
