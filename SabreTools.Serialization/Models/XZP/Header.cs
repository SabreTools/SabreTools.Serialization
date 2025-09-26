using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Header
    {
        /// <summary>
        /// "piZx"
        /// </summary>
        /// <remarks>4 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string? Signature;

        public uint Version;

        public uint PreloadDirectoryEntryCount;

        public uint DirectoryEntryCount;

        public uint PreloadBytes;

        public uint HeaderLength;

        public uint DirectoryItemCount;

        public uint DirectoryItemOffset;

        public uint DirectoryItemLength;
    }
}
