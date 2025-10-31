using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#System_Control_Info"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class SystemControlInfo
    {
        /// <summary>
        /// Application title (default is "CtrApp")
        /// </summary>
        /// <remarks>8 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string ApplicationTitle;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>5 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Reserved1 = new byte[5];

        /// <summary>
        /// Flag (bit 0: CompressExefsCode, bit 1: SDApplication)
        /// </summary>
        public byte Flag;

        /// <summary>
        /// Remaster version
        /// </summary>
        public ushort RemasterVersion;

        /// <summary>
        /// Text code set info
        /// </summary>
        public CodeSetInfo TextCodeSetInfo;

        /// <summary>
        /// Stack size
        /// </summary>
        public uint StackSize;

        /// <summary>
        /// Read-only code set info
        /// </summary>
        public CodeSetInfo ReadOnlyCodeSetInfo;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved2;

        /// <summary>
        /// Data code set info
        /// </summary>
        public CodeSetInfo DataCodeSetInfo;

        /// <summary>
        /// BSS size
        /// </summary>
        public uint BSSSize;

        /// <summary>
        /// Dependency module (program ID) list
        /// </summary>
        /// <remarks>48 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public ulong[]? DependencyModuleList;

        /// <summary>
        /// SystemInfo
        /// </summary>
        public SystemInfo SystemInfo;
    }
}
