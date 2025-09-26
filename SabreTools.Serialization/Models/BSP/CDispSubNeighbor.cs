using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://github.com/ValveSoftware/source-sdk-2013/blob/0d8dceea4310fde5706b3ce1c70609d72a38efdf/sp/src/public/bspfile.h#L557"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CDispSubNeighbor
    {
        /// <summary>
        /// This indexes into ddispinfos.
        /// </summary>
        /// <remarks>0xFFFF if there is no neighbor here.</remarks>
        public ushort NeighborIndex;

        /// <summary>
        /// (CCW) rotation of the neighbor wrt this displacement.
        /// </summary>
        public byte NeighborOrientation;

        // These use the NeighborSpan type.

        /// <summary>
        /// Where the neighbor fits onto this side of our displacement.
        /// </summary>
        public byte Span;

        /// <summary>
        /// Where we fit onto our neighbor.
        /// </summary>
        public byte NeighborSpan;
    }
}