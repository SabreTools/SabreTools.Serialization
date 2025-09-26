using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// The dcubemapsample_t structure defines the location of a
    /// env_cubemap entity in the map. The origin member contains
    /// integer x,y,z coordinates of the cubemap, and the size member
    /// is resolution of the cubemap, specified as 2^(size-1) pixels
    /// square. If set as 0, the default size of 6 (32x32 pixels) is
    /// used. There can be a maximum of 1024 (MAX_MAP_CUBEMAPSAMPLES)
    /// cubemaps in a file. 
    /// 
    /// When the "buildcubemaps" console command is performed, six
    /// snapshots of the map (one for each direction) are taken at the
    /// location of each env_cubemap entity. These snapshots are stored
    /// in a multi-frame texture (vtf) file, which is added to the
    /// Pakfile lump (see above). The textures are named cX_Y_Z.vtf,
    /// where (X,Y,Z) are the (integer) coordinates of the corresponding
    /// cubemap. 
    /// 
    /// Faces containing materials that are environment mapped (e.g.
    /// shiny textures) reference their assigned cubemap through their
    /// material name. A face with a material named (e.g.) walls/shiny.vmt
    /// is altered (new Texinfo & Texdata entries are created) to refer
    /// to a renamed material maps/mapname/walls/shiny_X_Y_Z.vmt, where
    /// (X,Y,Z) are the cubemap coordinates as before. This .vmt file
    /// is also stored in the Pakfile, and references the cubemap .vtf
    /// file through its $envmap property. 
    /// 
    /// Version 20 files contain extra cX_Y_Z.hdr.vtf files in the
    /// Pakfile lump, containing HDR texture files in RGBA16161616F
    /// (16-bit per channel) format. 
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Cubemap
    {
        /// <summary>
        /// Position of light snapped to the nearest integer
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[]? Origin = new int[3];

        /// <summary>
        /// Resolution of cubemap, 0 - default
        /// </summary>
        public int Size;
    }
}