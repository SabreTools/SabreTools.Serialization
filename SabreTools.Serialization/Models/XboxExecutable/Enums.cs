using System;

namespace SabreTools.Data.Models.XboxExecutable
{
    /// <summary>
    /// Allowed media types for this .XBE
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum AllowedMediaTypes : uint
    {
        HARD_DISK = 0x00000001,
        DVD_X2 = 0x00000002,
        DVD_CD = 0x00000004,
        CD = 0x00000008,
        DVD_5_RO = 0x00000010,
        DVD_9_RO = 0x00000020,
        DVD_5_RW = 0x00000040,
        DVD_9_RW = 0x00000080,
        DONGLE = 0x00000100,
        MEDIA_BOARD = 0x00000200,
        NONSECURE_HARD_DISK = 0x40000000,
        NONSECURE_MODE = 0x80000000,
        MEDIA_MASK = 0x00FFFFFF,
    }

    /// <summary>
    /// Game region for this .XBE
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum GameRegion : uint
    {
        NA = 0x00000001,
        JAPAN = 0x00000002,
        RESTOFWORLD = 0x00000004,
        MANUFACTURING = 0x80000000,
    }

    /// <summary>
    /// Various flags for this .XBE file
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum InitializationFlags : uint
    {
        MountUtilityDrive = 0x00000001,
        FormatUtilityDrive = 0x00000002,
        Limit64Megabytes = 0x00000004,
        DontSetupHarddisk = 0x00000008,
    }

    /// <summary>
    /// Various flags for this library
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum LibraryFlags : ushort
    {
        /// <remarks>13-Bit Mask</remarks>
        QFEVersion = 0x1FFF,

        /// <remarks>02-Bit Mask</remarks>
        Approved = 0x6000,

        /// <remarks>01-Bit Mask</remarks>
        DebugBuild = 0x8000,
    }

    /// <summary>
    /// Various flags for this .XBE section
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum SectionFlags : uint
    {
        Writable = 0x00000001,
        Preload = 0x00000002,
        Executable = 0x00000004,
        InsertedFile = 0x00000008,
        HeadPageReadOnly = 0x00000010,
        TailPageReadOnly = 0x00000020,
    }
}
