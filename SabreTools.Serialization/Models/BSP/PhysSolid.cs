using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The last two parts appear to be identical to the PHY file format,
    /// which means their exact contents are unknown. Note that the
    /// compactsurfaceheader_t structure contains the data size of each
    /// collision data section (including the rest of the header)
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class PhysSolid
    {
        /// <summary>
        /// Size of the collision data
        /// </summary>
        public int Size;

        /// <summary>
        /// Collision data of length <see cref="Size"/>
        /// </summary>
        public byte[] CollisionData;
    }
}
