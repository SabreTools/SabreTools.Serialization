using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// The physcollide lump (Lump 29) contains physics data for the world.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class PhysModel
    {
        /// <summary>
        /// Perhaps the index of the model to which this physics model applies?
        /// </summary>
        public int ModelIndex;

        /// <summary>
        /// Total size of the collision data sections
        /// </summary>
        public int DataSize;

        /// <summary>
        /// Size of the text section
        /// </summary>
        public int KeydataSize;

        /// <summary>
        /// Number of collision data sections
        /// </summary>
        public int SolidCount;

        /// <summary>
        /// Collision data of length <see cref="SolidCount"/> 
        /// </summary>
        public PhysSolid[]? Solids;

        /// <summary>
        /// Key data of size <see cref="KeydataSize"/> 
        /// </summary>
        public byte[]? TextData;
    }
}