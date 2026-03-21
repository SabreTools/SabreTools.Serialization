using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryEntry
    {
        public uint FileNameCRC;

        public uint EntryLength;

        public uint EntryOffset;
    }
}
