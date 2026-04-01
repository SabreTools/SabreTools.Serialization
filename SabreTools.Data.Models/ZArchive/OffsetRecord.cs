namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Location and size properties of compressed blocks of the file data
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public class OffsetRecord
    {
        /// <summary>
        /// Base offset of compressed blocks
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// Sizes of each compressed block in this record
        /// </summary>
        public ushort[] Size { get; set; } = new ushort[Constants.BlocksPerOffsetRecord];
    }
}
