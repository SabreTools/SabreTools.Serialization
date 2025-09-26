using System;

namespace SabreTools.Data.Models.InstallShieldCabinet
{
    /// <remarks>i6comp02</remarks>
    [Flags]
    public enum ComponentStatus : ushort
    {
        /// <summary>
        /// Critical
        /// </summary>
        COMP_STAT_CRITICAL = 0x01,

        /// <summary>
        /// Recommended
        /// </summary>
        COMP_STAT_RECOMMEND = 0x02,

        /// <summary>
        /// Standard
        /// </summary>
        COMP_STAT_STANDARD = 0x04,
    }

    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    [Flags]
    public enum FileFlags : ushort
    {
        FILE_SPLIT = 1,
        FILE_OBFUSCATED = 2,
        FILE_COMPRESSED = 4,
        FILE_INVALID = 8,
    }

    /// <remarks>i6comp02</remarks>
    [Flags]
    public enum FileGroupAttributes : ushort
    {
        /// <summary>
        /// Shared
        /// </summary>
        FGATTR_SHARED = 0x01,

        /// <summary>
        /// Encrypted
        /// </summary>
        FGATTR_ENCRYPTED = 0x02,

        /// <summary>
        /// Compressed
        /// </summary>
        FGATTR_COMPRESSED = 0x04,

        /// <summary>
        /// Self-Registering
        /// </summary>
        FGATTR_SELFREGISTER = 0x10,

        /// <summary>
        /// Potentially locked
        /// </summary>
        FGATTR_LOCKED = 0x20,

        /// <summary>
        /// Uninstall
        /// </summary>
        FGATTR_UNINSTALL = 0x40,
    }

    /// <remarks>i6comp02</remarks>
    [Flags]
    public enum FileGroupFlags : uint
    {
        /// <summary>
        /// Always overwrite
        /// </summary>
        FGDESC_ALWAYS_OVERWRITE = 0x001,

        /// <summary>
        /// Never overwrite
        /// </summary>
        FGDESC_NEVER_OVERWRITE = 0x002,

        /// <summary>
        /// Overwrite if newer date
        /// </summary>
        FGDESC_NEWER_DATE = 0x020,

        /// <summary>
        /// Overwrite if newer version
        /// </summary>
        FGDESC_NEWER_VERSION = 0x200,
    }

    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public enum LinkFlags : byte
    {
        LINK_NONE = 0,
        LINK_PREV = 1,
        LINK_NEXT = 2,
        LINK_BOTH = 3,
    }
}