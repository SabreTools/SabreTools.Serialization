namespace SabreTools.Data.Models.GZIP
{
    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    public sealed class Trailer
    {
        /// <summary>
        /// CRC-32
        /// 
        /// This contains a Cyclic Redundancy Check value of the
        /// uncompressed data computed according to CRC-32 algorithm
        /// used in the ISO 3309 standard and in section 8.1.1.6.2 of
        /// ITU-T recommendation V.42.  (See http://www.iso.ch for
        /// ordering ISO documents. See gopher://info.itu.ch for an
        /// online version of ITU-T V.42.)
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// Input SIZE
        /// 
        /// This contains the size of the original (uncompressed) input
        /// data modulo 2^32.
        /// </summary>
        public uint InputSize { get; set; }
    }
}