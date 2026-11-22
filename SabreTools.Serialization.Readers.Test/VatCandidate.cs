namespace SabreTools.Serialization.Readers.Test
{
    /// <summary>
    /// File Entry in the last sector that is not the Virtual Allocation Table, which is then recorded in block 38
    /// </summary>
    public enum VatCandidate
    {
        None,

        /// <summary>
        /// UDF 1.50 table whose 120 bytes in block 36 have no identifier
        /// </summary>
        NotATable,

        /// <summary>
        /// UDF 2.00 table with more entries than the partition has blocks
        /// </summary>
        TooLarge,

        /// <summary>
        /// UDF 2.00 table recorded in its File Entry, with fewer bytes than its information length
        /// </summary>
        Truncated,

        /// <summary>
        /// UDF 2.00 table with extended allocation descriptors, which are not supported
        /// </summary>
        ExtendedDescriptors,

        /// <summary>
        /// Regular file whose 120 bytes are recorded in block 36
        /// </summary>
        RegularFile,

        /// <summary>
        /// UDF 1.50 table whose 120 bytes are recorded in a block past the end of the partition
        /// </summary>
        PastPartition,
    }
}
