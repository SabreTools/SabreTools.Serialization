using System;

namespace SabreTools.Data.Models.OperaFS
{
    /// <summary>
    /// Volume descriptor flag values
    /// Formerly mapped 0x01 to DATA_DISC but this was not used by 3DO, so was remapped for M2 discs
    /// </summary>
    [Flags]
    public enum VolumeFlags : byte
    {
        /// <summary>
        /// Indicates it is a data disc, system does not need rebooting
        /// Should not be set for any retail 3DO discs
        /// </summary>
        M1_DATA_DISC = 0x01,

        /// <summary>
        /// Indicates M2 compatible disc?
        /// Should be set for all Konami M2 discs
        /// </summary>
        M2 = 0x01,

        /// <summary>
        /// Indicates disc is only compatible with M2 ?
        /// </summary>
        M2_ONLY = 0x02,

        /// <summary>
        /// Indicates it is a data disc, system does not need rebooting
        /// </summary>
        M2_DATA_DISC = 0x04,

        /// <summary>
        /// "Blessed" volume
        /// Should be set for all retail Konami M2 discs
        /// </summary>
        M2_SIGNED = 0x08,

        /// <summary>
        /// Mask for bits that are reserved
        /// </summary>
        RESERVED_MASK = 0xF0,
    }

    /// <summary>
    /// Directory record flag values
    /// </summary>
    [Flags]
    public enum DirectoryRecordFlags : uint
    {
        /// <summary>
        /// Record is a directory
        /// </summary>
        DIRECTORY = 0x01,

        /// <summary>
        /// Record is read-only
        /// </summary>
        READ_ONLY = 0x02,

        /// <summary>
        /// Record is for filesystem use only
        /// </summary>
        SYSTEM = 0x04,

        /// <summary>
        /// Final record in this block
        /// </summary>
        BLOCK_FINAL = 0x40000000,

        /// <summary>
        /// Final record in this directory
        /// </summary>
        DIRECTORY_FINAL = 0x80000000,

        /// <summary>
        /// Mask for bits that are reserved
        /// </summary>
        RESERVED_MASK = 0x3FFFFFF8,
    }
}
