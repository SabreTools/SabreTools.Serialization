using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;

namespace SabreTools.Data.Extensions
{
    public static class ISO9660Extensions
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
        /// Indicates if a directory record describes a CD-DA file
        /// </summary>
        /// <param name="dr">Directory record to check</param>
        /// <returns>True if the CD-ROM XA system use information marks the file as CD-DA, false otherwise</returns>
        /// <remarks>
        /// The CD-ROM XA system use information is the first 14 bytes of the System Use field,
        /// with the signature "XA" at bytes 6 and 7 and the big-endian attributes at bytes 4 and 5.
        /// Attribute bit 14 marks a file whose sectors are CD-DA audio sectors.
        /// </remarks>
        public static bool IsCDDA(this DirectoryRecord dr)
        {
            byte[] systemUse = dr.SystemUse;
            if (systemUse.Length < 14 || systemUse[6] != 'X' || systemUse[7] != 'A')
                return false;

            return (systemUse[4] & 0x40) != 0;
        }

        /// <summary>
        /// Indicates if a block size is valid
        /// </summary>
        /// <param name="blockSize">Block length to check</param>
        /// <param name="sectorLength">Defined sector length</param>
        /// <returns>True if the block length is valid, false otherwise</returns>
        private static bool BlockSizeValid(short blockSize, short sectorLength)
            => blockSize >= 512 && blockSize <= sectorLength && (blockSize & (blockSize - 1)) == 0;
    }
}
