namespace SabreTools.Data.Models.BZip2
{
    public class BlockHeader
    {
        /// <summary>
        /// A 48-bit integer value 31 41 59 26 53 59, which
        /// is the binary-coded decimal representation of
        /// pi. It is used to differentiate the block
        /// from the footer.
        /// </summary>
        /// <remarks>This may not be byte-aligned</remarks>
        public byte[] Magic { get; set; } = new byte[6];

        /// <summary>
        /// The CRC-32 checksum of the uncompressed data contained
        /// in <see cref="BlockData"/>. It is the same checksum
        /// used in GZip, but is slightly different due to the
        /// bit-packing differences.
        /// </summary>
        public uint Crc32 { get; set; }

        /// <summary>
        /// Should be 0. Previous versions of BZip2 allowed
        /// the input data to be randomized to avoid
        /// pathological strings from causing the runtime
        /// to be exponential.
        /// </summary>
        /// <remarks>Actually a 1-bit value</remarks>
        public byte Randomized { get; set; }

        /// <summary>
        /// Contains the origin pointer used in the BWT stage
        /// </summary>
        /// <remarks>Actually a 24-bit value</remarks>
        public uint OrigPtr { get; set; }
    }
}
