using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Each of this structures defines a plane in 3-dimensional
    /// space by using the Hesse normal form: normal * point - distance = 0
    /// 
    /// Where vNormal is the normalized normal vector of the plane
    /// and fDist is the distance of the plane to the origin of
    /// the coord system. Additionally, the structure also saves an
    /// integer describing the orientation of the plane in space.
    /// If nType equals PLANE_X, then the normal of the plane will
    /// be parallel to the x axis, meaning the plane is perpendicular
    /// to the x axis. If nType equals PLANE_ANYX, then the plane's
    /// normal is nearer to the x axis then to any other axis.
    /// This information is used by the renderer to speed up some
    /// computations. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Plane
    {
        /// <summary>
        /// The planes normal vector
        /// </summary>
        public Vector3D? NormalVector;

        /// <summary>
        /// Plane equation is: vNormal * X = fDist
        /// </summary>
        public float Distance;

        /// <summary>
        /// Plane type
        /// </summary>
        public PlaneType PlaneType;
    }
}