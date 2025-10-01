namespace SabreTools.Data.Models.BZip2
{
    public class Footer
    {
        /// <summary>
        /// A 48-bit integer value 17 72 45 38 50 90, which
        /// is the binary-coded decimal representation of
        /// sqrt(pi). It is used to differentiate the block
        /// from the footer.
        /// </summary>
        /// <remarks>This may not be byte-aligned</remarks>
        public byte[]? Magic { get; set; }

        /// <summary>
        /// Contains a custom checksum computed using each of
        /// the Block CRCs.
        /// </summary>
        /// <remarks>This may not be byte-aligned</remarks>
        public uint Checksum { get; set; }

        /// <summary>
        /// Used to align the bit-stream to the next byte-aligned
        /// edge and will contain between 0 and 7 bits.
        /// </summary>
        public byte Padding { get; set; }
    }
}
