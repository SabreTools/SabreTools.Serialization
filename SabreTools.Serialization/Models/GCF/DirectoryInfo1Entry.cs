using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryInfo1Entry
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;
    }
}