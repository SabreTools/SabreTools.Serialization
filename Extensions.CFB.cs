using System;
using SabreTools.Models.CFB;

namespace SabreTools.Serialization
{
    public static partial class Extensions
    {
        /// <summary>
        /// Sector size in bytes
        /// </summary>
        /// <param name="binary">Binary model to derive from</param>
        /// <returns>Sector size on success, -1 on error</returns>
        public static long SectorSize(this Binary binary)
        {
            // If we have an invalid header
            if (binary.Header?.SectorShift == null)
                return -1;

            return (long)Math.Pow(2, binary.Header.SectorShift);
        }

        /// <summary>
        /// Mini sector size in bytes
        /// </summary>
        /// <param name="binary">Binary model to derive from</param>
        /// <returns>Sector size on success, -1 on error</returns>
        public static long MiniSectorSize(this Binary binary)
        {
            // If we have an invalid header
            if (binary.Header?.SectorShift == null)
                return -1;

            return (long)Math.Pow(2, binary.Header.MiniSectorShift);
        }
    }
}