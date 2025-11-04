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
            BothInt16 blockLength;
            if (vd is PrimaryVolumeDescriptor pvd)
                blockLength = pvd.LogicalBlockSize;
            else if (vd is SupplementaryVolumeDescriptor svd)
                blockLength = svd.LogicalBlockSize;
            else
                return sectorLength;

            // If the block size is inconsistent
            if (!blockLength.IsValid)
            {
                bool leValid = BlockLengthValid(blockLength.LittleEndian, sectorLength);
                bool beValid = BlockLengthValid(blockLength.BigEndian, sectorLength);

                if (leValid && !beValid)
                    blockLength = blockLength.LittleEndian;
                else if (beValid && !leValid)
                    blockLength = blockLength.BigEndian;
                else
                    return sectorLength;
            }

            // Validate logical block length
            if (!BlockLengthValid(blockLength, sectorLength))
                blockLength = sectorLength;

            return blockLength;
        }

        /// <summary>
        /// Indicates if a block length is valid
        /// </summary>
        /// <param name="blockLength">Block length to check</param>
        /// <param name="sectorLength">Defined sector length</param>
        /// <returns>True if the block length is valid, false otherwise</returns>
        private static bool BlockLengthValid(short blockLength, short sectorLength)
            => blockLength >= 512 && blockLength <= sectorLength && (blockLength & (blockLength - 1)) == 0;

        /// <summary>
        /// Indicates if an array contains all ASCII numeric digits
        /// </summary>
        /// TODO: Move to IO as an array extension
        public static bool IsNumericArray(this byte[] arr)
            => Array.TrueForAll(arr, b => b >= 0x30 && b <= 0x39);
    }
}
