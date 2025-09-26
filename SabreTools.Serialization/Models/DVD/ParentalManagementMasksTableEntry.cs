using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ParentalManagementMasksTableEntry
    {
        /// <summary>
        /// Country code
        /// </summary>
        public ushort CountryCode;

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved;

        /// <summary>
        /// Offset to PTL_MAIT
        /// </summary>
        public uint Offset;
    }
}