using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// A Model, in the terminology of the BSP file format, is a collection
    /// of brushes and faces, often called a "bmodel". It should not be
    /// confused with the prop models used in Hammer, which are usually
    /// called "studiomodels" in the SDK source.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    [StructLayout(LayoutKind.Sequential)]
    public sealed class VbspModel
    {
        /// <summary>
        /// Bounding box
        /// </summary>
        public Vector3D? Mins;

        /// <summary>
        /// Bounding box
        /// </summary>
        public Vector3D? Maxs;

        /// <summary>
        /// For sounds or lights
        /// </summary>
        public Vector3D? OriginVector;

        /// <summary>
        /// Index into nodes
        /// </summary>
        public int HeadNode;

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