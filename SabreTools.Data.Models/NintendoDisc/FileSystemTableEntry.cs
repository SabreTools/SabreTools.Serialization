namespace SabreTools.Data.Models.NintendoDisc
{
    /// <summary>
    /// File entry with start and end byte offsets on disc
    /// </summary>
    public class FileSystemTableEntry
    {
        /// <summary>
        /// Offset to the entry name
        /// </summary>
        /// <remarks>Big-endian, has high byte set to 0xFF if a directory entry</remarks>
        public uint NameOffset { get; set; }

        /// <summary>
        /// Offset to the start of the file
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint FileOffset { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint FileSize { get; set; }
    }
}
