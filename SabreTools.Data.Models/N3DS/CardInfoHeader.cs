using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCSD#Card_Info_Header"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CardInfoHeader
    {
        /// <summary>
        /// CARD2: Writable Address In Media Units (For 'On-Chip' Savedata). CARD1: Always 0xFFFFFFFF.
        /// </summary>
        public uint WritableAddressMediaUnits;

        /// <summary>
        /// Card Info Bitmask
        /// </summary>
        public uint CardInfoBitmask;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0xF8 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xF8)]
        public byte[] Reserved1 = new byte[0xF8];

        /// <summary>
        /// Filled size of cartridge
        /// </summary>
        public uint FilledSize;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x0C bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
        public byte[] Reserved2 = new byte[0x0C];

        /// <summary>
        /// Title version
        /// </summary>
        public ushort TitleVersion;

        /// <summary>
        /// Card revision
        /// </summary>
        public ushort CardRevision;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x0C bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
        public byte[] Reserved3 = new byte[0x0C];

        /// <summary>
        /// Title ID of CVer in included update partition
        /// </summary>
        /// <remarks>8 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] CVerTitleID = new byte[8];

        /// <summary>
        /// Version number of CVer in included update partition
        /// </summary>
        public ushort CVerVersionNumber;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0xCD6 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xCD6)]
        public byte[] Reserved4 = new byte[0xCD6];

        /// <summary>
        /// This data is returned by 16-byte cartridge command 0x82.
        /// </summary>
        public InitialData InitialData = new();
    }
}
