using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#ARM11_Local_System_Capabilities"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ARM11LocalSystemCapabilities
    {
        /// <summary>
        /// Program ID
        /// </summary>
        public ulong ProgramID;

        /// <summary>
        /// Core version (The Title ID low of the required FIRM)
        /// </summary>
        public uint CoreVersion;

        /// <summary>
        /// Flag1 (implemented starting from 8.0.0-18).
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public ARM11LSCFlag1 Flag1;

        /// <summary>
        /// Flag2 (implemented starting from 8.0.0-18).
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public ARM11LSCFlag2 Flag2;

        /// <summary>
        /// Flag0
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public ARM11LSCFlag0 Flag0;

        /// <summary>
        /// Priority
        /// </summary>
        public byte Priority;

        /// <summary>
        /// Resource limit descriptors. The first byte here controls the maximum allowed CpuTime.
        /// </summary>
        /// <remarks>16 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[]? ResourceLimitDescriptors;

        /// <summary>
        /// Storage info
        /// </summary>
        public StorageInfo? StorageInfo;

        /// <summary>
        /// Service access control
        /// </summary>
        /// <remarks>32 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ulong[]? ServiceAccessControl;

        /// <summary>
        /// Extended service access control, support for this was implemented with 9.3.0-X.
        /// </summary>
        /// <remarks>2 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ulong[]? ExtendedServiceAccessControl;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x0F bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0F)]
        public byte[]? Reserved;

        /// <summary>
        /// Resource limit category. (0 = APPLICATION, 1 = SYS_APPLET, 2 = LIB_APPLET, 3 = OTHER (sysmodules running under the BASE memregion))
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public ResourceLimitCategory ResourceLimitCategory;
    }
}
