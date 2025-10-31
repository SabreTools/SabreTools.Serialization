using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.InstallShieldArchiveV3
{
    /// <see href="https://github.com/wfr/unshieldv3/blob/master/ISArchiveV3.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public class Header
    {
        public uint Signature1;

        public uint Signature2;

        public ushort Reserved0;

        public ushort IsMultivolume;

        public ushort FileCount;

        public uint DateTime;

        public uint CompressedSize;

        public uint UncompressedSize;

        public uint Reserved1;

        /// <remarks>
        /// Set in first vol only, zero in subsequent vols
        /// </remarks>
        public byte VolumeTotal;

        /// <remarks>
        /// [1...n]
        /// </remarks>
        public byte VolumeNumber;

        public byte Reserved2;

        public uint SplitBeginAddress;

        public uint SplitEndAddress;

        public uint TocAddress;

        public uint Reserved3;

        public ushort DirCount;

        public uint Reserved4;

        public uint Reserved5;
    }
}
