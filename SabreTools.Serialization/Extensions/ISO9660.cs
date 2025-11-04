using System;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;

namespace SabreTools.Data.Extensions
{
    public static class ISO9660
    {
        /// <summary>
        /// Get the logical block size from a sector length
        /// </summary>
        /// <param name="vd">Volume descriptor containing block information</param>
        /// <param name="sectorLength">Defined sector length</param>
        /// <returns>Size of a logical block</returns>
        public static short GetLogicalBlockSize(this VolumeDescriptor vd, short sectorLength)
        {
            BothInt16 blockSize;
            if (vd is PrimaryVolumeDescriptor pvd)
                blockSize = pvd.LogicalBlockSize;
            else if (vd is SupplementaryVolumeDescriptor svd)
                blockSize = svd.LogicalBlockSize;
            else
                return sectorLength;

            // If the block size is inconsistent
            if (!blockSize.IsValid)
            {
                bool leValid = BlockSizeValid(blockSize.LittleEndian, sectorLength);
                bool beValid = BlockSizeValid(blockSize.BigEndian, sectorLength);

                if (leValid && !beValid)
                    blockSize = blockSize.LittleEndian;
                else if (beValid && !leValid)
                    blockSize = blockSize.BigEndian;
                else
                    return sectorLength;
            }

            // Validate logical block size
            if (!BlockSizeValid(blockSize, sectorLength))
                blockSize = sectorLength;

            return blockSize;
        }

        /// <summary>
        /// Indicates if a block size is valid
        /// </summary>
        /// <param name="blockSize">Block length to check</param>
        /// <param name="sectorLength">Defined sector length</param>
        /// <returns>True if the block length is valid, false otherwise</returns>
        private static bool BlockSizeValid(short blockSize, short sectorLength)
            => blockSize >= 512 && blockSize <= sectorLength && (blockSize & (blockSize - 1)) == 0;

        /// <summary>
        /// Indicates if an array contains all ASCII numeric digits
        /// </summary>
        /// TODO: Move to IO as an array extension
        public static bool IsNumericArray(this byte[] arr)
            => Array.TrueForAll(arr, b => b >= 0x30 && b <= 0x39);
    }
}
