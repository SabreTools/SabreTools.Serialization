using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ProgramChainTableEntry
    {
        /// <summary>
        /// PGC category
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public ProgramChainCategory Category;

        /// <summary>
        /// Unknown
        /// </summary>
        public byte Unknown;

        /// <summary>
        /// Parental management mask
        /// </summary>
        public ushort ParentalManagementMask;

        /// <summary>
        /// Offset to VMGM_PGC, relative to VMGM_LU
        /// </summary>
        public uint Offset;
    }
}