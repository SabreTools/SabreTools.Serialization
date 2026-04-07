using System.Collections.Generic;
using SabreTools.IO.Numerics;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Hash Table Entry in a Hash Block's Hash Table
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class HashTableEntry
    {
        /// <summary>
        /// SHA-1 hash of the block
        /// </summary>
        public byte[] Hash { get; set; } = new byte[20];

        /// <summary>
        /// Status of the block
        /// 0x00 = Unused block
        /// 0x40 = Free block (previously used, now freed)
        /// 0x80 = Used block
        /// 0xC0 = Newly allocated block
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Block number corresponding to the hash
        /// FFFFFF = Block 1 (starting 0xB000)
        /// </summary>
        /// <remarks>Big-endian, 3-byte uint24</remarks>
        public UInt24 BlockNumber { get; set; }
    }
}
