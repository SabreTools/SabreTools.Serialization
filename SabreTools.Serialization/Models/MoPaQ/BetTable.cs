using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.MoPaQ
{
    /// <summary>
    /// The BET table is present if the BetTablePos64 member of MPQ header is set
    /// to nonzero. BET table is a successor of classic block table, and can fully
    /// replace it. It is also supposed to be more effective.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    public sealed class BetTable
    {
        // TODO: Extract this out and make in common between HET and BET
        #region Common Table Headers

        /// <summary>
        /// 'BET\x1A'
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Signature;

        /// <summary>
        /// Version. Seems to be always 1
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Size of the contained table
        /// </summary>
        public uint DataSize { get; set; }

        #endregion

        /// <summary>
        /// Size of the entire hash table, including the header (in bytes)
        /// </summary>
        public uint TableSize { get; set; }

        /// <summary>
        /// Number of files in the BET table
        /// </summary>
        public uint FileCount { get; set; }

        /// <summary>
        /// Unknown, set to 0x10
        /// </summary>
        public uint Unknown { get; set; }

        /// <summary>
        /// Size of one table entry (in bits)
        /// </summary>
        public uint TableEntrySize { get; set; }

        /// <summary>
        /// Bit index of the file position (within the entry record)
        /// </summary>
        public uint FilePositionBitIndex { get; set; }

        /// <summary>
        /// Bit index of the file size (within the entry record)
        /// </summary>
        public uint FileSizeBitIndex { get; set; }

        /// <summary>
        /// Bit index of the compressed size (within the entry record)
        /// </summary>
        public uint CompressedSizeBitIndex { get; set; }

        /// <summary>
        /// Bit index of the flag index (within the entry record)
        /// </summary>
        public uint FlagIndexBitIndex { get; set; }

        /// <summary>
        /// Bit index of the ??? (within the entry record)
        /// </summary>
        public uint UnknownBitIndex { get; set; }

        /// <summary>
        /// Bit size of file position (in the entry record)
        /// </summary>
        public uint FilePositionBitCount { get; set; }

        /// <summary>
        /// Bit size of file size (in the entry record)
        /// </summary>
        public uint FileSizeBitCount { get; set; }

        /// <summary>
        /// Bit size of compressed file size (in the entry record)
        /// </summary>
        public uint CompressedSizeBitCount { get; set; }

        /// <summary>
        /// Bit size of flags index (in the entry record)
        /// </summary>
        public uint FlagIndexBitCount { get; set; }

        /// <summary>
        /// Bit size of ??? (in the entry record)
        /// </summary>
        public uint UnknownBitCount { get; set; }

        /// <summary>
        /// Total size of the BET hash
        /// </summary>
        public uint TotalBetHashSize { get; set; }

        /// <summary>
        /// Extra bits in the BET hash
        /// </summary>
        public uint BetHashSizeExtra { get; set; }

        /// <summary>
        /// Effective size of BET hash (in bits)
        /// </summary>
        public uint BetHashSize { get; set; }

        /// <summary>
        /// Size of BET hashes array, in bytes
        /// </summary>
        public uint BetHashArraySize { get; set; }

        /// <summary>
        /// Number of flags in the following array
        /// </summary>
        public uint FlagCount { get; set; }

        /// <summary>
        /// Followed by array of file flags. Each entry is 32-bit size and its meaning is the same like
        /// </summary>
        /// <remarks>Size from <see cref="FlagCount"/></remarks>
        public uint[] FlagsArray { get; set; }

        // File table. Size of each entry is taken from dwTableEntrySize.
        // Size of the table is (dwTableEntrySize * dwMaxFileCount), round up to 8.

        // Array of BET hashes. Table size is taken from dwMaxFileCount from HET table
    }
}
