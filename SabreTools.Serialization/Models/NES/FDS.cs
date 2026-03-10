namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// fwNES FDS file
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/FDS_file_format"/>
    public class FDS
    {
        /// <summary>
        /// FDS header
        /// </summary>
        public FDSHeader? Header { get; set; }

        /// <summary>
        /// Disk data (65500 * x bytes)
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
