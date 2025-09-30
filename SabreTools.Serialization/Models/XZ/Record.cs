namespace SabreTools.Data.Models.XZ
{
    public class Record
    {
        /// <summary>
        /// Unpadded size of the block
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public byte UnpaddedSize { get; set; }

        /// <summary>
        /// Uncompressed size of the block
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public byte[]? NumberOfRecords { get; set; }
    }
}
