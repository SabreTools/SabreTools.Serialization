using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/BSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    [StructLayout(LayoutKind.Sequential)]
    public class BspLumpEntry
    {
        /// <summary>
        /// File offset to data
        /// </summary>
        public int Offset;

        /// <summary>
        /// Length of data
        /// </summary>
        public int Length;
    }
}