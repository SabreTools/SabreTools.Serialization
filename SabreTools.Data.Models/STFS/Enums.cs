using System;

namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// STFS Volume Content Type possible values
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public enum ContentType : int
    {
        SAVED_GAME = 0x00000001,
        MARKETPLACE_CONTENT = 0x00000002,
        PUBLISHER = 0x00000003,
        XBOX_360_TITLE = 0x00001000,
        IPTV_PAUSE_BUFFER = 0x00002000,
        INSTALLED_GAME = 0x00004000,
        XBOX_ORIGINAL_GAME = 0x00005000,
        XBOX_TITLE = 0x00006000,
        GAME_ON_DEMAND = 0x00007000,
        AVATAR_ITEM = 0x00009000,
        PROFILE = 0x00010000,
        GAMER_PICTURE = 0x00020000,
        THEME = 0x00030000,
        CACHE_FILE = 0x00040000,
        STORAGE_DOWNLOAD = 0x00050000,
        XBOX_SAVED_GAME = 0x00060000,
        XBOX_DOWNLOAD = 0x00070000,
        GAME_DEMO = 0x00080000,
        VIDEO = 0x00090000,
        GAME_TITLE = 0x000A0000,
        INSTALLER = 0x000B0000,
        GAME_TRAILER = 0x000C0000,
        ARCADE_TITLE = 0x000D0000,
        XNA = 0x000E0000,
        LICENSE_STORE = 0x000F0000,
        MOVIE = 0x00100000,
        TV = 0x00200000,
        MUSIC_VIDEO = 0x00300000,
        GAME_VIDEO = 0x00400000,
        PODCAST_VIDEO = 0x00500000,
        VIRAL_VIDEO = 0x00600000,
        COMMUNITY_GAME = 0x02000000,
    }

    /// <summary>
    /// STFS Transfer Flags
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    [Flags]
    public enum TransferFlags : uint
    {
        NONE1 = 0x00000001,
        NONE2 = 0x00000002,
        DEEP_LINK_SUPPORTED = 0x00000004,
        DISABLE_NETWORK_STORAGE = 0x00000008,
        KINECT_ENABLED = 0x00000010,
        MOVE_ONLY_TRANSFER = 0x00000020,
        DEVICE_ID_TRANSFER = 0x00000040,
        PROFILE_ID_TRANSFER = 0x00000080,
    }

    /// <summary>
    /// Installer cache package resume state
    /// </summary>
    public enum ResumeState : uint
    {
        FILE_HEADERS_NOT_READY = 0x46494C48,
        NEW_FOLDER = 0x666F6C64,
        NEW_FOLDER_RESUME_ATTEMPT_1 = 0x666F6C31,
        NEW_FOLDER_RESUME_ATTEMPT_2 = 0x666F6C32,
        NEW_FOLDER_RESUME_ATTEMPT_UNKNOWN = 0x666F6C3F,
        NEW_FOLDER_RESUME_ATTEMPT_SPECIFIC = 0x666F6C40,
    }
}
