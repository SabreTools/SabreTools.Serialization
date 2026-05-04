namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// WIA / RVZ disc type
    /// </summary>
    public enum WiaDiscType : uint
    {
        /// <summary>Nintendo GameCube disc</summary>
        GameCube = 1,

        /// <summary>Nintendo Wii disc</summary>
        Wii = 2,
    }

    /// <summary>
    /// Compression algorithm used inside a WIA or RVZ file.
    /// WIA supports None / Purge / Bzip2 / LZMA / LZMA2.
    /// RVZ additionally supports Zstd; Purge is not used in RVZ.
    /// </summary>
    public enum WiaRvzCompressionType : uint
    {
        /// <summary>No compression — data stored verbatim</summary>
        None = 0,

        /// <summary>Purge — strips known-zero regions (hash blocks, padding). WIA only.</summary>
        Purge = 1,

        /// <summary>bzip2 block compression</summary>
        Bzip2 = 2,

        /// <summary>LZMA compression</summary>
        LZMA = 3,

        /// <summary>LZMA2 compression</summary>
        LZMA2 = 4,

        /// <summary>Zstandard compression. RVZ only.</summary>
        Zstd = 5,
    }
}
