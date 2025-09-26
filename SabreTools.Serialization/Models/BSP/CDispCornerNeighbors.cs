using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://github.com/ValveSoftware/source-sdk-2013/blob/0d8dceea4310fde5706b3ce1c70609d72a38efdf/sp/src/public/bspfile.h#L600"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CDispCornerNeighbors
    {
        /// <summary>
        /// Indices of neighbors.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] Neighbors = new ushort[4];

        public byte NeighborCount;
    }
}