using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.GCF
{
    /// <remarks>
    /// Part of version 5 but not version 6.
    /// </remarks>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class BlockEntryMap
    {
        /// <summary>
        /// The previous block entry.  (N/A if == BlockCount.)
        /// </summary>
        public uint PreviousBlockEntryIndex;

        /// <summary>
        /// The next block entry.  (N/A if == BlockCount.)
        /// </summary>
        public uint NextBlockEntryIndex;
    }
}