using System;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Determines which type of duplicate a file is
    /// </summary>
    [Flags]
    public enum DupeType
    {
        // Type of match
        Hash = 1 << 0,
        All = 1 << 1,

        // Location of match
        Internal = 1 << 2,
        External = 1 << 3,
    }

    /// <summary>
    /// A subset of fields that can be used as keys
    /// </summary>
    public enum ItemKey
    {
        NULL = 0,

        Machine,

        CRC16,
        CRC32,
        CRC64,
        MD2,
        MD4,
        MD5,
        RIPEMD128,
        RIPEMD160,
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        SpamSum,
    }

    /// <summary>
    /// Determine what type of machine it is
    /// </summary>
    [Flags]
    public enum MachineType
    {
        /// <summary>"none"</summary>
        None = 0,

        /// <summary>"bios"</summary>
        Bios = 1 << 0,

        /// <summary>"device", "dev"</summary>
        Device = 1 << 1,

        /// <summary>"mechanical", "mech"</summary>
        Mechanical = 1 << 2,
    }
}
