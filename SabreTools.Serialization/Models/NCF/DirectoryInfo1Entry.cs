using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryInfo1Entry
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;
    }
}
