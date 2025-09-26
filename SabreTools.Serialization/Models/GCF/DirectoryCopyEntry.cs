using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryCopyEntry
    {
        /// <summary>
        /// Index of the directory item.
        /// </summary>
        public uint DirectoryIndex;
    }
}