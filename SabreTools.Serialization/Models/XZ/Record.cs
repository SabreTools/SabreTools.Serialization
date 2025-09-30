namespace SabreTools.Data.Models.XZ
{
    public class Record
    {
        /// <summary>
        /// Unpadded size of the block
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong UnpaddedSize { get; set; }

        /// <summary>
        /// Uncompressed size of the block
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong UncompressedSize { get; set; }
    }
}
