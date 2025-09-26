using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The reflectivity vector corresponds to the RGB components of the reflectivity
    /// of the texture, as derived from the material's .vtf file. This is probably
    /// used in radiosity (lighting) calculations of what light bounces from the
    /// texture's surface. The nameStringTableID is an index into the TexdataStringTable
    /// array (below). The other members relate to the texture's source image.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Texdata
    {
        /// <summary>
        /// RGB reflectivity
        /// </summary>
        public Vector3D? Reflectivity;

        /// <summary>
        /// Index into TexdataStringTable
        /// </summary>
        public int NameStringTableID;

        /// <summary>
        /// Source image
        /// </summary>
        public int Width;

        /// <summary>
        /// Source image
        /// </summary>
        public int Height;

        public int ViewWidth;

        public int ViewHeight;
    }
}