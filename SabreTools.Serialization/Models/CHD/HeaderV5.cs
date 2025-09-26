using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class HeaderV5 : Header
    {
        /// <summary>
        /// Which custom compressors are used?
        /// </summary>
        public CodecType[] Compressors { get; set; } = new CodecType[4];

        /// <summary>
        /// Logical size of the data (in bytes)
        /// </summary>
        public ulong LogicalBytes { get; set; }

        /// <summary>
        /// Offset to the map
        /// </summary>
        public ulong MapOffset { get; set; }

        /// <summary>
        /// Offset to the first blob of metadata
        /// </summary>
        public ulong MetaOffset { get; set; }

        /// <summary>
        /// Number of bytes per hunk (512k maximum)
        /// </summary>
        public uint HunkBytes { get; set; }

        /// <summary>
        /// Number of bytes per unit within each hunk
        /// </summary>
        public uint UnitBytes { get; set; }

        /// <summary>
        /// Raw data SHA1
        /// </summary>
        public byte[]? RawSHA1 { get; set; }

        /// <summary>
        /// Combined raw+meta SHA1
        /// </summary>
        public byte[]? SHA1 { get; set; }

        /// <summary>
        /// Combined raw+meta SHA1 of parent
        /// </summary>
        public byte[]? ParentSHA1 { get; set; }
    }
}
