using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [StructLayout(LayoutKind.Sequential)]
    public class UncompressedMapV5
    {
        /// <summary>
        /// Starting offset / hunk size
        /// </summary>
        public uint StartingOffset;
    }
}
