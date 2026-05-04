namespace SabreTools.Data.Models.OperaFS
{
    /// <summary>
    /// OperaFS Directory Record
    /// </summary>
    /// <see href="https://groups.google.com/g/rec.games.video.3do/c/1U3qrmLSYMQ"/>
    public class DirectoryRecord
    {
        /// <summary>
        /// Flags about this directory record
        /// </summary>
        public DirectoryRecordFlags DirectoryRecordFlags { get; set; } = new();

        /// <summary>
        /// Hash or random value to identify this record
        /// </summary>
        public byte[] UniqueIdentifier { get; set; } = new byte[4];

        /// <summary>
        /// Type of record, ASCII
        /// </summary>
        public byte[] Type { get; set; } = new byte[4];

        /// <summary>
        /// Sector size for this record
        /// Should be 0x800
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// Number of bytes in this record
        /// </summary>
        public uint ByteCount { get; set; }

        /// <summary>
        /// Number of blocks allocated to this record
        /// </summary>
        public uint BlockCount { get; set; }

        /// <summary>
        /// Burst, usually 0x1
        /// </summary>
        public uint Burst { get; set; }

        /// <summary>
        /// Gap, usually 0x0
        /// </summary>
        public uint Gap { get; set; }

        /// <summary>
        /// Filename of record
        /// </summary>
        public byte[] Filename { get; set; } = new byte[32];

        /// <summary>
        /// Number of duplicates of the file/directory provided
        /// </summary>
        public uint LastAvatarIndex { get; set; }

        /// <summary>
        /// Offset of the file/directories provided
        /// Length of array is LastAvatarIndex + 1
        /// </summary>
        public uint[] AvatarList { get; set; } = [];
    }
}
