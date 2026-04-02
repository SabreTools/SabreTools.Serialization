namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Footer data stored at the end of a ZArchive file
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public class Footer
    {
        /// <summary>
        /// Size and offset values for the CompressedData section
        /// </summary>
		public OffsetInfo SectionCompressedData { get; set; } = new();

        /// <summary>
        /// Size and offset values for the OffsetRecords section
        /// </summary>
		public OffsetInfo SectionOffsetRecords { get; set; } = new();

        /// <summary>
        /// Size and offset values for the NameTable section
        /// </summary>
		public OffsetInfo SectionNameTable { get; set; } = new();

        /// <summary>
        /// Size and offset values for the FileFree section
        /// </summary>
		public OffsetInfo SectionFileTree { get; set; } = new();

        /// <summary>
        /// Size and offset values for the MetaDirectory section
        /// </summary>
		public OffsetInfo SectionMetaDirectory { get; set; } = new();

        /// <summary>
        /// Size and offset values for the MetaData section
        /// </summary>
		public OffsetInfo SectionMetaData { get; set; } = new();

        /// <summary>
        /// SHA-256 hash of the ZArchive file prior the footer
        /// </summary>
        public byte[] IntegrityHash { get; set; } = new byte[32];

        /// <summary>
        /// Size of the entire ZArchive file
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong Size { get; set; }

        /// <summary>
        /// Version indicator, also acts as extended magic
        /// </summary>
        public byte[] Version { get; set; } = new byte[4];

        /// <summary>
        /// Magic bytes to indicate ZArchive file
        /// </summary>
        public byte[] Magic { get; set; } = new byte[4];
    }
}
