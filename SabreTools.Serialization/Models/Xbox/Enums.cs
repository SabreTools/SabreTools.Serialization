using System;

namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// Allowed media types for this .XBE
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum XbeAllowedMediaTypes : uint
    {
        XBEIMAGE_MEDIA_TYPE_HARD_DISK = 0x00000001,
        XBEIMAGE_MEDIA_TYPE_DVD_X2 = 0x00000002,
        XBEIMAGE_MEDIA_TYPE_DVD_CD = 0x00000004,
        XBEIMAGE_MEDIA_TYPE_CD = 0x00000008,
        XBEIMAGE_MEDIA_TYPE_DVD_5_RO = 0x00000010,
        XBEIMAGE_MEDIA_TYPE_DVD_9_RO = 0x00000020,
        XBEIMAGE_MEDIA_TYPE_DVD_5_RW = 0x00000040,
        XBEIMAGE_MEDIA_TYPE_DVD_9_RW = 0x00000080,
        XBEIMAGE_MEDIA_TYPE_DONGLE = 0x00000100,
        XBEIMAGE_MEDIA_TYPE_MEDIA_BOARD = 0x00000200,
        XBEIMAGE_MEDIA_TYPE_NONSECURE_HARD_DISK = 0x40000000,
        XBEIMAGE_MEDIA_TYPE_NONSECURE_MODE = 0x80000000,
        XBEIMAGE_MEDIA_TYPE_MEDIA_MASK = 0x00FFFFFF,
    }

    /// <summary>
    /// Game region for this .XBE
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum XbeGameRegion : uint
    {
        XBEIMAGE_GAME_REGION_NA = 0x00000001,
        XBEIMAGE_GAME_REGION_JAPAN = 0x00000002,
        XBEIMAGE_GAME_REGION_RESTOFWORLD = 0x00000004,
        XBEIMAGE_GAME_REGION_MANUFACTURING = 0x80000000,
    }

    /// <summary>
    /// Various flags for this .XBE file
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    [Flags]
    public enum XbeInitializationFlags : uint
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
    public enum XbeLibraryFlags : uint
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
    public enum XbeSectionFlags : uint
    {
        Writable = 0x00000001,
        Preload = 0x00000002,
        Executable = 0x00000004,
        InsertedFile = 0x00000008,
        HeadPageReadOnly = 0x00000010,
        TailPageReadOnly = 0x00000020,
    }
}
