namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Virtual Allocation Table (UDF 2.2.11), the contents of the file recorded last on write-once media,
    /// mapping each logical block of the Virtual Partition to a logical block of the underlying partition
    /// </summary>
    /// <remarks>
    /// UDF 2.00 and later (File Type 248): header, then the entries;
    /// UDF 1.50 (File Type 0): the entries, then the identifier and the previous VAT ICB location, no header
    /// </remarks>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class VirtualAllocationTable
    {
        /// <summary>
        /// Length of the header in bytes (UDF 2.00 and later)
        /// </summary>
        public ushort LengthOfHeader { get; set; }

        /// <summary>
        /// Length of the implementation use field in bytes (UDF 2.00 and later)
        /// </summary>
        public ushort LengthOfImplementationUse { get; set; }

        /// <summary>
        /// Logical volume identifier, dstring of 128 bytes (UDF 2.00 and later)
        /// </summary>
        public byte[] LogicalVolumeIdentifier { get; set; } = [];

        /// <summary>
        /// Logical block of the previous Virtual Allocation Table's ICB, 0xFFFFFFFF if none
        /// </summary>
        public uint PreviousVATICBLocation { get; set; }

        /// <summary>
        /// Number of files (UDF 2.00 and later)
        /// </summary>
        public uint NumberOfFiles { get; set; }

        /// <summary>
        /// Number of directories (UDF 2.00 and later)
        /// </summary>
        public uint NumberOfDirectories { get; set; }

        /// <summary>
        /// Minimum UDF revision needed to read the volume (UDF 2.00 and later)
        /// </summary>
        public ushort MinimumUDFReadRevision { get; set; }

        /// <summary>
        /// Minimum UDF revision needed to write the volume (UDF 2.00 and later)
        /// </summary>
        public ushort MinimumUDFWriteRevision { get; set; }

        /// <summary>
        /// Maximum UDF revision the volume was written with (UDF 2.00 and later)
        /// </summary>
        public ushort MaximumUDFWriteRevision { get; set; }

        /// <summary>
        /// Reserved, 2 bytes of 0x00 (UDF 2.00 and later)
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// Implementation use, the header after its fixed fields (UDF 2.00 and later)
        /// </summary>
        public byte[] ImplementationUse { get; set; } = [];

        /// <summary>
        /// Logical block of the underlying partition for each logical block of the Virtual Partition,
        /// 0xFFFFFFFF for an unused logical block
        /// </summary>
        public uint[] Entries { get; set; } = [];

        /// <summary>
        /// Entity identifier after the entries, "*UDF Virtual Alloc Tbl" (UDF 1.50 only)
        /// </summary>
        public EntityIdentifier? Identifier { get; set; }
    }
}
