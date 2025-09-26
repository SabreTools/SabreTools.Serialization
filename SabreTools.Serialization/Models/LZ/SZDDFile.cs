namespace SabreTools.Data.Models.LZ
{
    /// <summary>
    /// Standard LZ variant
    /// </summary>
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/kernel32/lzexpand.c"/>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    public sealed class SZDDFile
    {
        /// <summary>
        /// Header
        /// </summary>
        public SZDDHeader? Header { get; set; }

        // Followed immediately by compressed data
    }
}