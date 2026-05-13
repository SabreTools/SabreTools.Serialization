namespace SabreTools.Data.Models.NintendoDisc
{
    public class FileSystemTable
    {
        /// <summary>
        /// 8 bytes of unknown data
        /// </summary>
        /// <remarks>
        /// Maps to <see cref="FileSystemTableEntry.NameOffset"/> and
        /// <see cref="FileSystemTableEntry.FileOffset"/> but is unused?
        /// </remarks>
        public byte[] Unknown { get; set; } = new byte[8];

        /// <summary>
        /// Number of entries in the table
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint EntryCount { get; set; }

        /// <summary>
        /// File system table entries
        /// </summary>
        /// <remarks>Length given by <see cref="EntryCount"/></remarks>
        public FileSystemTableEntry[] Entries { get; set; } = [];
    }
}
