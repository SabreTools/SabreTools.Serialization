using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class VbspHeader
    {
        /// <summary>
        /// BSP file signature
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Signature;

        /// <summary>
        /// BSP file version
        /// </summary>
        /// <remarks>17,18,19,20,21,22,23,25,27,29</remarks>
        public int Version;

        /// <summary>
        /// Lump directory array
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.VBSP_HEADER_LUMPS)]
        public VbspLumpEntry[] Lumps = new VbspLumpEntry[Constants.VBSP_HEADER_LUMPS];

        /// <summary>
        /// The map's revision (iteration, version) number.
        /// </summary>
        public int MapRevision;
    }
}
