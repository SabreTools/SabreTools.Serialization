namespace SabreTools.Data.Models.SpoonInstaller
{
    /// <summary>
    /// Single entry in the file table
    /// </summary>
    /// <remarks>Entry data is BZ2-compressed</remarks>
    public class FileEntry
    {
        /// <summary>
        /// File offset relative to the start of the file
        /// </summary>
        public uint FileOffset { get; set; }

        /// <summary>
        /// Compressed size of the entry
        /// </summary>
        /// <remarks>Includes headers</remarks>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// Uncompressed size of the entry
        /// </summary>
        public uint UncompressedSize { get; set; }

        /// <summary>
        /// CRC-32(?)
        /// </summary>
        public uint Crc32 { get; set; }

        /// <summary>
        /// Length of <see cref="Filename"/>
        /// </summary>
        public byte FilenameLength { get; set; }

        /// <summary>
        /// ASCII filename
        /// </summary>
        public string Filename { get; set; } = string.Empty;
    }
}
