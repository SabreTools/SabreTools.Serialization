namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// Metadata block whose size is defined by <see cref="Z3DSFileHeader.MetadataSize"/>
    /// </summary>
    /// <see href="https://github.com/azahar-emu/azahar/blob/master/src/common/zstd_compression.h"/>
    public class Z3DSMetadata
    {
        /// <summary>
        /// Metadata version
        /// </summary>
        /// <remarks>Currently only a value of 1 is expected</remarks>
        public byte Version { get; set; }

        /// <summary>
        /// Set of metadata entries in the block
        /// </summary>
        public Z3DSMetadataItem[] Items { get; set; } = [];
    }
}
