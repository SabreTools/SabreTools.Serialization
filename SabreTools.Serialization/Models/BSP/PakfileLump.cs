namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// The Pakfile lump (Lump 40) is a special lump that can contains
    /// multiple files which are embedded into the bsp file. Usually,
    /// they contain special texture (.vtf) and material (.vmt) files
    /// which are used to store the reflection maps from env_cubemap
    /// entities in the map; these files are built and placed in the
    /// Pakfile lump when the buildcubemaps console command is executed.
    /// The Pakfile can optionally contain such things as custom textures
    /// and prop models used in the map, and are placed into the bsp file
    /// by using the BSPZIP program (or alternate programs such as Pakrat).
    /// These files are integrated into the game engine's file system
    /// and will be loaded preferentially before externally located
    /// files are used.
    /// 
    /// The format of the Pakfile lump is identical to that used by the
    /// Zip compression utility when no compression is specified (i.e.,
    /// the individual files are stored in uncompressed format). In some
    /// branches, such as , LZMA compression can be used as well. If the
    /// Pakfile lump is extracted and written to a file, it can therefore
    /// be opened with WinZip and similar programs.
    /// 
    /// The header public/zip_uncompressed.h defines the structures
    /// present in the Pakfile lump. The last element in the lump is a
    /// ZIP_EndOfCentralDirRecord structure. This points to an array of
    /// ZIP_FileHeader structures immediately preceeding it, one for each
    /// file present in the Pak. Each of these headers then point to
    /// ZIP_LocalFileHeader structures that are followed by that file's
    /// data. 
    /// 
    /// The Pakfile lump is usually the last element of the bsp file.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    public sealed class PakfileLump
    {
        /// <summary>
        /// Pakfile data
        /// </summary>
        /// TODO: Split and/or decompress data?
        public byte[]? Data;
    }
}