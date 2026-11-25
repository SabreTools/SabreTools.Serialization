using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "PX" POSIX file attributes, the POSIX file mode, links, user and group of the file
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    public sealed class PosixFileAttributes : BaseEntry
    {
        /// <summary>
        /// POSIX file mode (st_mode), the file type and permissions
        /// </summary>
        public BothUInt32 FileMode { get; set; } = 0;

        /// <summary>
        /// POSIX file links (st_nlink)
        /// </summary>
        public BothUInt32 Links { get; set; } = 0;

        /// <summary>
        /// POSIX file user ID (st_uid)
        /// </summary>
        public BothUInt32 UserId { get; set; } = 0;

        /// <summary>
        /// POSIX file group ID (st_gid)
        /// </summary>
        public BothUInt32 GroupId { get; set; } = 0;

        /// <summary>
        /// POSIX file serial number (st_ino), directory records with the same serial number are links to the same file
        /// Optional field, the entry of RRIP 1.09 and 1.10 is 36 bytes long and ends before it
        /// </summary>
        public BothUInt32? SerialNumber { get; set; }
    }
}
