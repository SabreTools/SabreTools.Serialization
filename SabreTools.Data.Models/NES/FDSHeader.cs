namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// fwNES FDS header
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/FDS_file_format"/>
    public class FDSHeader
    {
        /// <summary>
        /// Constant $46 $44 $53 $1A ("FDS" followed by MS-DOS end-of-file)
        /// </summary>
        public byte[] IdentificationString { get; set; } = new byte[4];

        /// <summary>
        /// Number of disk sides
        /// </summary>
        public byte DiskSides { get; set; }

        /// <summary>
        /// Zero-filled padding
        /// </summary>
        public byte[] Padding { get; set; } = new byte[11];
    }
}
