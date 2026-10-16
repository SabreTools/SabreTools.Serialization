namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Extended File Entry (4/14.17, UDF 2.00 and later), a File Entry with object size,
    /// creation time and a stream directory
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class ExtendedFileEntry : FileEntry
    {
        /// <summary>
        /// Size of the file and its streams in bytes
        /// </summary>
        public ulong ObjectSize { get; set; }

        /// <summary>
        /// Creation date and time
        /// </summary>
        public Timestamp CreationDateAndTime { get; set; } = new();

        /// <summary>
        /// Reserved, 0x00000000
        /// </summary>
        public uint Reserved { get; set; }

        /// <summary>
        /// ICB of the stream directory, length 0 if none
        /// </summary>
        public LongAllocationDescriptor StreamDirectoryICB { get; set; } = new();
    }
}
