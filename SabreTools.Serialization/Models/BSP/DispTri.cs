using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// he DispTris lump (Lump 48) contains "triangle tags" or flags
    /// related to the properties of a particular triangle in the
    /// displacement mesh.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DispTri
    {
        /// <summary>
        /// Displacement triangle tags.
        /// </summary>
        public DispTriTag Tags;
    }
}