using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCSD#Development_Card_Info_Header_Extension"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DevelopmentCardInfoHeader
    {
        /// <summary>
        /// CardDeviceReserved1
        /// </summary>
        /// <remarks>0x200 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x200)]
        public byte[]? CardDeviceReserved1;

        /// <summary>
        /// TitleKey
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[]? TitleKey;

        /// <summary>
        /// CardDeviceReserved2
        /// </summary>
        /// <remarks>0x1BF0 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1BF0)]
        public byte[]? CardDeviceReserved2;

        /// <summary>
        /// TestData
        /// </summary>
        public TestData? TestData;
    }
}
