namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// Four-part version number commonly used by Microsoft
    /// </summary>
    public class FourPartVersionType
    {
        /// <summary>
        /// Major Version Number
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort Major { get; set; }

        /// <summary>
        /// Minor Version Number
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort Minor { get; set; }

        /// <summary>
        /// Build Version Number
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort Build { get; set; }

        /// <summary>
        /// Revision Version Number
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort Revision { get; set; }
    }
}
