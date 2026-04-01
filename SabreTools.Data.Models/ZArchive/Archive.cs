namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Represents a single ZAR archive
    /// Most fields are Big Endian
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public class Archive
    {
        /// <summary>
        /// Zstd compressed file data, from 65536-byte blocks of the original files
        /// Blocks are stored uncompressed if ZStd does not decrease the size
        /// Due to the file size, this field is not usually filled in but remains here for completeness
        /// </summary>
        public byte[]? CompressedData { get; set; }

        /// <summary>
        /// Padding bytes to be added after compressed blocks to ensure 8-byte alignment
        /// Padding bytes are all NULL (0x00)
        /// </summary>
        public byte[]? Padding { get; set; }

        /// <summary>
        /// Records containing the offsets and block sizes of each group of blocks
        /// This allows the reader to jump to any 65536-byte boundary in the uncompressed stream.
        /// </summary>
        public OffsetRecord[] OffsetRecords { get; set; } = [];

        /// <summary>
        /// UTF-8 strings, prepended by string lengths
        /// </summary>
        public NameTable NameTable { get; set; } = new();

        /// <summary>
        /// Serialized file tree structure using a queue of nodes
        /// </summary>
        public FileDirectoryEntry[] FileTree { get; set; } = [];

        /// <summary>
        /// Section for custom key-value pairs and properties
        /// </summary>
        public Metadata? Metadata { get; set; }

        /// <summary>
        /// Archive footer containing the offsets and sizes of all other sections
        /// Ends with a SHA256 hash/size of the entire archive, and magic bytes
        /// </summary>
        public Footer Footer { get; set; } = new();
    }
}
