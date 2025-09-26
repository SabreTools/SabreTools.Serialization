using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.cpp"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MetadataEntry
    {
        /// <summary>
        /// Offset within the file of the header
        /// </summary>
        public ulong Offset;

        /// <summary>
        /// Offset within the file of the next header
        /// </summary>
        public ulong Next;

        /// <summary>
        /// Offset within the file of the previous header
        /// </summary>
        public ulong Prev;

        /// <summary>
        /// Length of the metadata
        /// </summary>
        public uint Length;

        /// <summary>
        /// Metadata tag
        /// </summary>
        public MetadataTag Metatag;

        /// <summary>
        /// Flag bits
        /// </summary>
        public MetadataFlags Flags;
    }
}
