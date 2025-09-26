using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#System_Info"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class SystemInfo
    {
        /// <summary>
        /// SaveData Size
        /// </summary>
        public ulong SaveDataSize;

        /// <summary>
        /// Jump ID
        /// </summary>
        public ulong JumpID;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x30 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
        public byte[]? Reserved;
    }
}
