using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class UnknownEntry
    {
        /// <summary>
        /// Reserved
        /// </summary>
        public uint Dummy0;
    }
}
