namespace SabreTools.Data.Models.OperaFS
{
    /// <summary>
    /// OperaFS Directory Descriptor
    /// </summary>
    /// <see href="https://groups.google.com/g/rec.games.video.3do/c/1U3qrmLSYMQ"/>
    public class DirectoryDescriptor
    {
        /// <summary>
        /// Offset of the next block
        /// 0xFFFFFFFF implies this is the last block
        /// </summary>
        public int NextBlock { get; set; }

        /// <summary>
        /// Offset of the previous block
        /// 0xFFFFFFFF implies this is the first block
        /// </summary>
        public int PreviousBlock { get; set; }

        /// <summary>
        /// Should be zeroed
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        /// First free byte
        /// </summary>
        public uint FirstFreeByte { get; set; }

        /// <summary>
        /// First entry offset
        /// </summary>
        public uint FirstEntryOffset { get; set; }

        /// <summary>
        /// Directory records in this directory
        /// </summary>
        public DirectoryRecord[] DirectoryRecords { get; set; } = [];
    }
}
