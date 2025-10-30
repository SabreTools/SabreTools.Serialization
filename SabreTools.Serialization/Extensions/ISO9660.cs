using System;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class ISO9660
    {
        /// <summary>
        /// Get the logical block size from a sector length
        /// </summary>
        /// <param name="bvd">Volume descriptor containing block information</param>
        /// <param name="sectorLength">Defined sector length</param>
        /// <returns>Size of a logical block</returns>
        public static short GetLogicalBlockSize(this BaseVolumeDescriptor bvd, short sectorLength)
        {
            short blockLength = sectorLength;
            if (bvd.LogicalBlockSize.IsValid)
            {
                // Validate logical block length
                if (bvd.LogicalBlockSize >= 512 && bvd.LogicalBlockSize <= sectorLength && (bvd.LogicalBlockSize & (bvd.LogicalBlockSize - 1)) == 0)
                    blockLength = bvd.LogicalBlockSize;
            }
            else
            {
                // If logical block size is ambiguous check if only one is valid, otherwise default to sector length
                short le = bvd.LogicalBlockSize.LittleEndian;
                short be = bvd.LogicalBlockSize.LittleEndian;
                bool le_valid = true;
                bool be_valid = true;
                if (le < 512 || le > sectorLength || (le & (le - 1)) != 0)
                    le_valid = false;
                if (be < 512 || be > sectorLength || (be & (be - 1)) != 0)
                    be_valid = false;
                if (le_valid && !be_valid)
                    blockLength = le;
                else if (be_valid && !le_valid)
                    blockLength = be;
                else
                    blockLength = sectorLength;
            }

            return blockLength;
        }

        /// <summary>
        /// Check if a volume descriptor has all 0x00 reserved bytes
        /// </summary>
        /// <param name="bvd">Volume descriptor containing the reserved bytes</param>
        /// <returns>True if the reserved bytes are all 0x00, false otherwise</returns>
        public static bool HasUnsetReservedBytes(this BaseVolumeDescriptor? bvd)
        {
            // Invalid volume descriptor
            if (bvd?.Reserved653Bytes == null)
                return false;

            // Check if all bytes are 0x00
            return Array.TrueForAll(bvd.Reserved653Bytes, b => b == 0x00);
        }

        /// <summary>
        /// Determine if a volume descriptor was written by ImgBurn
        /// </summary>
        /// <param name="bvd">Volume descriptor containing the application use bytes</param>
        /// <returns>True if the descriptor was written by ImgBurn, false otherwise</returns>
        public static bool IsImgBurn(this BaseVolumeDescriptor? bvd)
        {
            // Invalid volume descriptor
            if (bvd?.ApplicationUse == null)
                return false;

            // "ImgBurn"
            return bvd.ApplicationUse.StartsWith([0x49, 0x6D, 0x67, 0x42, 0x75, 0x72, 0x6E]);
        }

        /// <summary>
        /// Determine if a volume descriptor was written by UltraISO
        /// </summary>
        /// <param name="bvd">Volume descriptor containing the application use bytes</param>
        /// <returns>True if the descriptor was written by UltraISO, false otherwise</returns>
        public static bool IsUltraISO(this BaseVolumeDescriptor? bvd)
        {
            // Invalid volume descriptor
            if (bvd?.ApplicationUse == null)
                return false;

            // "ULTRAISO"
            return bvd.ApplicationUse.StartsWith([0x55, 0x4C, 0x54, 0x52, 0x41, 0x49, 0x53, 0x4F]);
        }

        /// <summary>
        /// Indicates if an array contains all ASCII numeric digits
        /// </summary>
        /// TODO: Move to IO as an array extension
        public static bool IsNumericArray(this byte[] arr)
            => Array.TrueForAll(arr, b => b >= 0x30 && b <= 0x39);
    }
}
