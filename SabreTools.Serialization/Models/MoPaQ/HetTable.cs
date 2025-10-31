using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.MoPaQ
{
    /// <summary>
    /// The HET table is present if the HetTablePos64 member of MPQ header is
    /// set to nonzero. This table can fully replace hash table. Depending on
    /// MPQ size, the pair of HET&BET table can be more efficient than Hash&Block
    /// table. HET table can be encrypted and compressed.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    public sealed class HetTable
    {
        // TODO: Extract this out and make in common between HET and BET
        #region Common Table Headers

        /// <summary>
        /// 'HET\x1A'
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
        /// Maximum number of files in the MPQ
        /// </summary>
        public uint MaxFileCount { get; set; }

        /// <summary>
        /// Size of the hash table (in bytes)
        /// </summary>
        public uint HashTableSize { get; set; }

        /// <summary>
        /// Effective size of the hash entry (in bits)
        /// </summary>
        public uint HashEntrySize { get; set; }

        /// <summary>
        /// Total size of file index (in bits)
        /// </summary>
        public uint TotalIndexSize { get; set; }

        /// <summary>
        /// Extra bits in the file index
        /// </summary>
        public uint IndexSizeExtra { get; set; }

        /// <summary>
        /// Effective size of the file index (in bits)
        /// </summary>
        public uint IndexSize { get; set; }

        /// <summary>
        /// Size of the block index subtable (in bytes)
        /// </summary>
        public uint BlockTableSize { get; set; }

        /// <summary>
        /// HET hash table. Each entry is 8 bits.
        /// </summary>
        /// <remarks>Size is derived from HashTableSize</remarks>
        public byte[] HashTable { get; set; }

        /// <summary>
        /// Array of file indexes. Bit size of each entry is taken from dwTotalIndexSize.
        /// Table size is taken from dwHashTableSize.
        /// </summary>
        public byte[][] FileIndexes { get; set; }
    }
}
