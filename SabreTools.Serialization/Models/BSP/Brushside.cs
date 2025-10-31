using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Planenum is an index info the plane array, giving the plane
    /// corresponding to this brushside. Texinfo and dispinfo are
    /// references into the texture and displacement info lumps.
    /// Bevel is zero for normal brush sides, but 1 if the side is
    /// a bevel plane (which seem to be used for collision detection).
    ///
    /// Unlike the face array, brushsides are not culled (removed)
    /// where they touch the void. Void-facing sides do however have
    /// their texinfo entry changed to the tools/toolsnodraw texture
    /// during the compile process. Note there is no direct way of
    /// linking brushes and brushsides and the corresponding face array
    /// entries which are used to render that brush. Brushsides are
    /// used by the engine to calculate all player physics collision
    /// with world brushes. (Vphysics objects use lump 29 instead.)
    ///
    /// The maximum number of brushsides is 65536 (MAX_MAP_BRUSHSIDES).
    /// The maximum number of brushsides on a single brush is 128 (MAX_BRUSH_SIDES).
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Brushside
    {
        /// <summary>
        /// Facing out of the leaf
        /// </summary>
        public ushort PlaneNum;

        /// <summary>
        /// Texture info
        /// </summary>
        public short TextureInfo;

        /// <summary>
        /// Displacement info
        /// </summary>
        public short DisplacementInfo;

        /// <summary>
        /// Is the side a bevel plane?
        /// </summary>
        public short Bevel;
    }
}
