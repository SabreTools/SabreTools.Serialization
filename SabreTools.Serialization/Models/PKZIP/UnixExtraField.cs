namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the UNIX "extra" block.
    ///
    /// The variable length data field will contain file type
    /// specific data.  Currently the only values allowed are
    /// the original "linked to" file names for hard or symbolic
    /// links, and the major and minor device node numbers for
    /// character and block device nodes.  Since device nodes
    /// cannot be either symbolic or hard links, only one set of
    /// variable length data is stored.  Link files will have the
    /// name of the original file stored.  This name is NOT NULL
    /// terminated.  Its size can be determined by checking TSize -
    /// 12.  Device entries will have eight bytes stored as two 4
    /// byte entries (in little endian format).  The first entry
    /// will be the major device number, and the second the minor
    /// device number.
    /// </summary>
    /// <remarks>Header ID = 0x000D</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class UnixExtraField : ExtensibleDataField
    {
        /// <summary>
        /// File last access time
        /// </summary>
        public uint FileLastAccessTime { get; set; }

        /// <summary>
        /// File last modification time
        /// </summary>
        public uint FileLastModificationTime { get; set; }

        /// <summary>
        /// File user ID
        /// </summary>
        public ushort FileUserID { get; set; }

        /// <summary>
        /// File group ID
        /// </summary>
        public ushort FileGroupID { get; set; }

        /// <summary>
        /// Variable length data field
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
