using System;

namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// Enum for SymbolicLink.Flags
    /// All other bits are reserved
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    [Flags]
    public enum SymbolicLinkFlags : byte
    {
        /// <summary>
        /// Bit 0: The symbolic link continues in the next "SL" entry
        /// </summary>
        CONTINUE = 0x01,
    }

    /// <summary>
    /// Enum for SymbolicLinkComponent.Flags
    /// All other bits are reserved
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    [Flags]
    public enum ComponentFlags : byte
    {
        /// <summary>
        /// Bit 0: The component continues in the next component record
        /// </summary>
        CONTINUE = 0x01,

        /// <summary>
        /// Bit 1: The component is the current directory (".")
        /// </summary>
        CURRENT = 0x02,

        /// <summary>
        /// Bit 2: The component is the parent directory ("..")
        /// </summary>
        PARENT = 0x04,

        /// <summary>
        /// Bit 3: The component is the root directory ("/")
        /// </summary>
        ROOT = 0x08,

        /// <summary>
        /// Bit 4: Implementation specific, historically the directory the volume is mounted on (VOLROOT in RRIP 1.10)
        /// </summary>
        VOLROOT = 0x10,

        /// <summary>
        /// Bit 5: Implementation specific, historically the network node name of the system (HOST in RRIP 1.10)
        /// </summary>
        HOST = 0x20,
    }

    /// <summary>
    /// Enum for AlternateName.Flags
    /// All other bits are reserved
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    [Flags]
    public enum AlternateNameFlags : byte
    {
        /// <summary>
        /// Bit 0: The alternate name continues in the next "NM" entry
        /// </summary>
        CONTINUE = 0x01,

        /// <summary>
        /// Bit 1: The alternate name is the current directory (".")
        /// </summary>
        CURRENT = 0x02,

        /// <summary>
        /// Bit 2: The alternate name is the parent directory ("..")
        /// </summary>
        PARENT = 0x04,

        /// <summary>
        /// Bit 5: Implementation specific, historically the network node name of the system (HOST in RRIP 1.10)
        /// </summary>
        HOST = 0x20,
    }

    /// <summary>
    /// Enum for TimeStamps.Flags
    /// </summary>
    /// <see href="https://people.freebsd.org/~emaste/rrip112.pdf"/>
    [Flags]
    public enum TimeStampFlags : byte
    {
        /// <summary>
        /// Bit 0: Creation time is recorded
        /// </summary>
        CREATION = 0x01,

        /// <summary>
        /// Bit 1: Modification time (st_mtime) is recorded
        /// </summary>
        MODIFY = 0x02,

        /// <summary>
        /// Bit 2: Last access time (st_atime) is recorded
        /// </summary>
        ACCESS = 0x04,

        /// <summary>
        /// Bit 3: Last attribute change time (st_ctime) is recorded
        /// </summary>
        ATTRIBUTES = 0x08,

        /// <summary>
        /// Bit 4: Last backup time is recorded
        /// </summary>
        BACKUP = 0x10,

        /// <summary>
        /// Bit 5: Expiration time is recorded
        /// </summary>
        EXPIRATION = 0x20,

        /// <summary>
        /// Bit 6: Effective time is recorded
        /// </summary>
        EFFECTIVE = 0x40,

        /// <summary>
        /// Bit 7: The time stamps are in the 17-byte format of a volume descriptor instead of the 7-byte format
        /// of a directory record
        /// </summary>
        LONG_FORM = 0x80,
    }

    /// <summary>
    /// Enum for ExtensionsInUse.Flags
    /// </summary>
    /// <see href="https://github.com/torvalds/linux/blob/master/fs/isofs/rock.h"/>
    [Flags]
    public enum ExtensionsInUseFlags : byte
    {
        /// <summary>
        /// Bit 0: "PX" is recorded
        /// </summary>
        POSIX_FILE_ATTRIBUTES = 0x01,

        /// <summary>
        /// Bit 1: "PN" is recorded
        /// </summary>
        POSIX_DEVICE_NUMBER = 0x02,

        /// <summary>
        /// Bit 2: "SL" is recorded
        /// </summary>
        SYMBOLIC_LINK = 0x04,

        /// <summary>
        /// Bit 3: "NM" is recorded
        /// </summary>
        ALTERNATE_NAME = 0x08,

        /// <summary>
        /// Bit 4: "CL" is recorded
        /// </summary>
        CHILD_LINK = 0x10,

        /// <summary>
        /// Bit 5: "PL" is recorded
        /// </summary>
        PARENT_LINK = 0x20,

        /// <summary>
        /// Bit 6: "RE" is recorded
        /// </summary>
        RELOCATED_DIRECTORY = 0x40,

        /// <summary>
        /// Bit 7: "TF" is recorded
        /// </summary>
        TIME_STAMPS = 0x80,
    }
}
