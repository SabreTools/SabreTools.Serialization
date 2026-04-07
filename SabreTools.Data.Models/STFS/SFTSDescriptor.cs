using System.Collections.Generic;
using SabreTools.IO.Numerics;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Volume Descriptor, for STFS packages
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class SFTSDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// Volume descriptor size (Should be 0x24)
        /// </summary>
        public byte VolumeDescriptorSize { get; set; }

        /// <summary>
        /// Reserved (Should be 0x00)
        /// </summary>
        public byte Reserved { get; set; }

        /// <summary>
        /// Block Separation
        /// </summary>
        public byte BlockSeparation { get; set; }

        /// <summary>
        /// File Table Block Count
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public short FileTableBlockCount { get; set; }

        /// <summary>
        /// File Table Block Number
        /// </summary>
        /// <remarks>Big-endian, 3-byte int24</remarks>
        public Int24 FileTableBlockNumber { get; set; }

        /// <summary>
        /// Top Hash Table Hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] TopHashTableHash { get; set; } = new byte[20];

        /// <summary>
        /// Total Allocated Block Count
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int TotalAllocatedBlockCount { get; set; }

        /// <summary>
        /// Total Unallocated Block Count
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int TotalUnallocatedBlockCount { get; set; }
    }
}
