using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryMapping
    {
        public ushort PreloadDirectoryEntryIndex;
    }
}
