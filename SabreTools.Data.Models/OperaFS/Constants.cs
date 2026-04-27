namespace SabreTools.Data.Models.OperaFS
{
    /// <summary>
    /// OperaFS constant values and arrays
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Standard block size for OperaFS disc images
        /// </summary>
        public static readonly int SectorSize = 2048;

        /// <summary>
        /// Start of a standard OperaFS image
        /// </summary>
        public static readonly byte[] MagicBytes = [0x01, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x01];

        /// <summary>
        /// Padding bytes within a OperaFS FileSystem, "iamaduck"
        /// </summary>
        public static readonly byte[] PaddingBytes = [0x69, 0x61, 0x6D, 0x61, 0x64, 0x75, 0x63, 0x6B];
    }
}
