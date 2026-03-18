using System.Runtime.InteropServices;

#pragma warning disable CS0618 // 'UnmanagedType.AnsiBStr' is obsolete
namespace SabreTools.Data.Models.InstallShieldArchiveV3
{
    /// <see href="https://github.com/wfr/unshieldv3/blob/master/ISArchiveV3.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public class File
    {
        public byte VolumeEnd;

        public ushort Index;

        public uint UncompressedSize;

        public uint CompressedSize;

        public uint Offset;

        public uint DateTime;

        public uint Reserved0;

        public ushort ChunkSize;

        [MarshalAs(UnmanagedType.U1)]
        public Attributes Attrib;

        public byte IsSplit;

        public byte Reserved1;

        public byte VolumeStart;

        [MarshalAs(UnmanagedType.AnsiBStr)]
        public string Name = string.Empty;
    }
}
