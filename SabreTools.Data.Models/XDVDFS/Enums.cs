using System;

namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// Enum for DirectoryRecord.FileFlags
    /// Values are assumed to match lowest byte value of DOS/FAT/NTFS file attributes
    /// </summary>
    /// <see href="https://multimedia.cx/xdvdfs.html"/>
    /// <see href="https://learn.microsoft.com/dotnet/api/system.io.fileattributes"/>
    [Flags]
    public enum FileFlags : byte
    {
        /// <summary>
        /// Record is read-only
        /// </summary>
        READ_ONLY = 0x01,

        /// <summary>
        /// Record is hidden
        /// </summary>
        HIDDEN = 0x02,

        /// <summary>
        /// Record is part of or for the operating system
        /// </summary>
        SYSTEM = 0x04,

        /// <summary>
        /// Record is a volume ID
        /// </summary>
        VOLUME_ID = 0x08,

        /// <summary>
        /// Record is a directory
        /// </summary>
        DIRECTORY = 0x10,

        /// <summary>
        /// Record should be archived
        /// </summary>
        ARCHIVE = 0x20,

        /// <summary>
        /// Record is a device
        /// </summary>
        DEVICE = 0x40,

        /// <summary>
        /// Record has no other attributes
        /// </summary>
        NORMAL = 0x80,
    }
}
