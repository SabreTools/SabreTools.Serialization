using System;

namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Flags for Xbox 360 Module Flags field
    /// </summary>
    [Flags]
    public enum ModuleFlags : uint
    {
        TITLE_MODULE = 0x00000001,
        EXPORTS_TO_TITLE = 0x00000002,
        SYSTEM_DEBUGGER = 0x00000004,
        DLL_MODULE = 0x00000008,
        MODULE_PATCH = 0x00000010,
        FULL_PATCH = 0x00000020,
        DELTA_PATCH = 0x00000040,
        USER_MODE = 0x00000080,
        UNKNOWN_MASK = 0xFFFFFF00,
    }

    /// <summary>
    /// Flags for Certificate ImageFlags field
    /// </summary>
    [Flags]
    public enum ImageFlags : uint
    {
        MANUFACTURING_UTILITY = 0x00000002,
        MANUFACTURING_SUPPORT_TOOL = 0x00000004,
        ORIGINAL_MEDIA_ONLY = 0x00000008,
        CARDEA_KEY_WMDRM_ND = 0x00000100,
        XEIKA_KEY_AP25 = 0x00000200,
        TITLE_USERMODE = 0x00000400,
        SYSTEM_USERMODE = 0x00000800,
        ORANGE0 = 0x00001000,
        ORANGE1 = 0x00002000,
        ORANGE2 = 0x00004000,
        SIGNED_KEYVAULT_RESTRICTED = 0x00008000,
        IPTV_SIGNUP_APPLICATION = 0x00010000,
        IPTV_TITLE_APPLICATION = 0x00020000,
        KEYVAULT_PRIVILEGES_REQUIRED = 0x04000000,
        APPLICATION_REQUIRED = 0x08000000,
        FOUR_KB_PAGES = 0x10000000, // else 64 KB Pages
        NO_GAME_REGION = 0x20000000,
        REVOCATION_CHECK_OPTIONAL = 0x40000000,
        REVOCATION_CHECK_REQUIRED = 0x80000000,
        UNKNOWN_MASK = 0x03FC80F1,
    }

    /// <summary>
    /// Flags for OptionalHeader SystemFlags field
    /// </summary>
    [Flags]
    public enum SystemFlags : uint
    {
        NO_FORCED_REBOOT = 0x00000001,
        FOREGROUND_TASKS = 0x00000002,
        NO_ODD_MAPPING = 0x00000004,
        HANDLES_MCE_INPUT = 0x00000008,
        RESTRICTED_HUD_FEATURES = 0x00000010,
        HANDLES_GAMEPAD_DISCONNECT = 0x00000020,
        HAS_SECURE_SOCKETS = 0x00000040,
        XBOX1_INTEROPERABILITY = 0x00000080,
        DASH_CONTEXT = 0x00000100,
        USES_GAME_VOICE_CHANNEL = 0x00000200,
        PAL50_INCOMPATIBLE = 0x00000400,
        INSECURE_UTILITY_DRIVE_SUPPORT = 0x00000800,
        XAM_HOOKS = 0x00001000,
        ACCESSES_PII = 0x00002000,
        CROSS_PLATFORM_SYSTEM_LINK = 0x00004000,
        MULTIDISC_SWAP = 0x00008000,
        SUPPORTS_INSECURE_MULTIDISC_MEDIA = 0x00010000,
        ANTIPIRACY25_MEDIA = 0x00020000,
        NO_CONFIRM_EXIT = 0x00040000,
        ALLOW_BACKGROUND_DOWNLOADING = 0x00080000,
        CREATE_PERSISTABLE_RAM_DRIVE = 0x00100000,
        INHERIT_PERSISTENT_RAM_DRIVE = 0x00200000,
        ALLOW_HUD_VIBRATION = 0x00400000,
        ALLOW_BOTH_UTILITY_PARTITIONS_ACCESS = 0x00800000,
        HANDLES_IPTV_INPUT = 0x01000000,
        PREFERS_BIG_BUTTON_INPUT = 0x02000000,
        ALLOW_EXTENDED_SYSTEM_RESERVATION = 0x04000000,
        MULTIDISC_CROSS_TITLE = 0x08000000,
        TITLE_INSTALL_INCOMPATIBLE = 0x10000000,
        ALLOW_AVATAR_GET_METADATA_BY_XUID = 0x20000000,
        ALLOW_CONTROLLER_SWAPPING = 0x40000000,
        DASH_EXTENSIBILITY_MODULE = 0x80000000,
    }

    /// <summary>
    /// Flags for Certificate AllowedMediaTypeFlags field
    /// </summary>
    [Flags]
    public enum AllowedMediaTypeFlags : uint
    {
        HARD_DISK = 0x00000001,
        XBOX1_ORIGINAL_DISK = 0x00000002,
        DVD_CD = 0x00000004,
        DVD_5 = 0x00000008,
        DVD_9 = 0x00000010,
        SYSTEM_FLASH = 0x00000020,
        MEMORY_UNIT = 0x00000080,
        USB_MASS_STORAGE_DEVICE = 0x00000100,
        NETWORKED_SMB_SHARE = 0x00000200,
        DIRECT_FROM_RAM = 0x00000400,
        RAM_DRIVE = 0x00000800,
        INSECURE_PACKAGE = 0x01000000,
        SAVEGAME_PACKAGE = 0x02000000,
        LOCALLY_SIGNED_PACKAGE = 0x04000000,
        LIVE_SIGNED_PACKAGE = 0x08008000,
        XBOX_PACKAGE = 0x10000000,
        UNKNOWN_MASK = 0xE0FFE040,
    }

    /// <summary>
    /// Flags for Xbox 360 console regions
    /// Source: Original Research
    /// </summary>
    [Flags]
    public enum RegionFlags : uint
    {
        NTSC_U = 0x00_00_00_FF,
        JAPAN = 0x00_00_01_00,
        CHINA = 0x00_00_02_00,
        ASIA = 0x00_00_F8_00,
        JAPAN_AND_ASIA = 0x00_00_F9_00,
        NTSC_J_EXCLUDING_CHINA = 0x00_00_FD_00,
        OCEANIA = 0x00_01_00_00,
        EUROPE = 0x00_FE_00_00,
        PAL = 0x00_FF_00_00,
        REGION_FREE = 0xFF_FF_FF_FF,
    }
}
