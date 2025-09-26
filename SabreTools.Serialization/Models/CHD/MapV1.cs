using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MapV1
    {
        /// <summary>
        /// Starting offset within the file
        /// </summary>
        public ulong StartingOffset;

        /// <summary>
        /// Length of data; If == hunksize, data is uncompressed
        /// </summary>
        public ulong Length;
    }
}
