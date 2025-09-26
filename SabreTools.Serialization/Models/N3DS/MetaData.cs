using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/CIA#Meta"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MetaData
    {
        /// <summary>
        /// Title ID dependency list - Taken from the application's ExHeader
        /// </summary>
        /// TODO: Determine numeric format of each entry
        /// <remarks>0x180 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x180)]
        public byte[]? TitleIDDependencyList;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x180 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x180)]
        public byte[]? Reserved1;

        /// <summary>
        /// Core Version
        /// </summary>
        public uint CoreVersion;

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0xFC bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xFC)]
        public byte[]? Reserved2;

        /// <summary>
        /// Icon Data(.ICN) - Taken from the application's ExeFS
        /// </summary>
        /// <remarks>0x36C0 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x36C0)]
        public byte[]? IconData;
    }
}