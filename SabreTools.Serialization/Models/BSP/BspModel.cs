using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// A model is kind of a mini BSP tree. Its size is determinded
    /// by the bounding box spaned by the first to members of this
    /// struct. The major difference between a model and the BSP
    /// tree holding the scene is that the models use a local
    /// coordinate system for their vertexes and just state its
    /// origin in world coordinates. During rendering the coordinate
    /// system is translated to the origin of the model (glTranslate())
    /// and moved back after the models BSP tree has been traversed.
    /// Furthermore their are 4 indexes into node arrays. The first
    /// one has proofed to index the root node of the mini BSP tree
    /// used for rendering. The other three indexes could probably be
    /// used for collision detection, meaning they point into the
    /// clipnodes, but I am not sure about this. The meaning of the
    /// next value is also somehow unclear to me. Finally their are
    /// direct indexes into the faces array, not taking the redirecting
    /// by the marksurfaces.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class BspModel
    {
        /// <summary>
        /// Defines bounding box
        /// </summary>
        public Vector3D Mins;

        /// <summary>
        /// Defines bounding box
        /// </summary>
        public Vector3D Maxs;

        /// <summary>
        /// Coordinates to move the coordinate system
        /// </summary>
        public Vector3D OriginVector;

        /// <summary>
        /// Index into nodes array
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MAX_MAP_HULLS)]
        public int[] HeadnodesIndex = new int[Constants.MAX_MAP_HULLS];

        /// <summary>
        /// ???
        /// </summary>
        public int VisLeafsCount;

        /// <summary>
        /// Index into faces
        /// </summary>
        public int FirstFaceIndex;

        /// <summary>
        /// Count of faces
        /// </summary>
        public int FacesCount;
    }
}
