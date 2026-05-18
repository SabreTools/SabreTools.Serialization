namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// Metadata item entry
    /// </summary>
    /// <see href="https://github.com/azahar-emu/azahar/blob/master/src/common/zstd_compression.h"/>
    public class Z3DSMetadataItem
    {
        /// <summary>
        /// Item type
        /// </summary>
        public Z3DSMetadataItemType Type { get; set; }

        /// <summary>
        /// Length of the name field
        /// </summary>
        public byte NameLength { get; set; }

        /// <summary>
        /// Length of the data field
        /// </summary>
        public ushort DataLength { get; set; }

        /// <summary>
        /// Name encoded as bytes whose length is given
        /// by <see cref="NameLength"/>
        /// </summary>
        /// <remarks>Ignored if <see cref="Type"/> is End</remarks>
        public byte[] Name { get; set; } = [];

        /// <summary>
        /// Data encoded as bytes whose length is given
        /// by <see cref="DataLength"/>
        /// </summary>
        /// <remarks>Ignored if <see cref="Type"/> is End</remarks>
        public byte[] Data { get; set; } = [];
    }
}
