using System;

namespace SabreTools.Data.Models.Atari7800
{
    /// <summary>
    /// Audio device (v4+ only)
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    [Flags]
    public enum AudioDevice : ushort
    {
        #region POKEY location

        /// <summary>
        /// POKEY - none
        /// </summary>
        PokeyNone = 0b00000000,

        /// <summary>
        /// POKEY - @440
        /// </summary>
        Pokey440 = 0b00000001,

        /// <summary>
        /// POKEY - @450
        /// </summary>
        Pokey450 = 0b00000010,

        /// <summary>
        /// POKEY - @450+@440
        /// </summary>
        Pokey450Plus440 = 0b00000011,

        /// <summary>
        /// POKEY - @800
        /// </summary>
        Pokey800 = 0b00000100,

        /// <summary>
        /// POKEY - @4000
        /// </summary>
        Pokey4000 = 0b00000101,

        #endregion

        /// <summary>
        /// YM2151 @460
        /// </summary>
        YM2151460 = 0b00001000,

        /// <summary>
        /// COVOX @430
        /// </summary>
        COVOX430 = 0b00010000,

        /// <summary>
        /// ADPCM Audio Stream @420
        /// </summary>
        ADPCMAudioStream420 = 0b00100000,
    }

    /// <summary>
    /// Cart type specifications
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    [Flags]
    public enum CartType : ushort
    {
        /// <summary>
        /// pokey at $4000
        /// </summary>
        PokeyAt4000 = 0b00000000_00000001,

        /// <summary>
        /// supergame bank switched
        /// </summary>
        SupergameBankSwitched = 0b00000000_00000010,

        /// <summary>
        /// supergame ram at $4000
        /// </summary>
        SupergameRamAt4000 = 0b00000000_00000100,

        /// <summary>
        /// rom at $4000
        /// </summary>
        RomAt4000 = 0b00000000_00001000,

        /// <summary>
        /// bank 6 at $4000
        /// </summary>
        Bank6At4000 = 0b00000000_00010000,

        /// <summary>
        /// banked ram
        /// </summary>
        BankedRam = 0b00000000_00100000,

        /// <summary>
        /// pokey at $450
        /// </summary>
        PokeyAt450 = 0b00000000_01000000,

        /// <summary>
        /// mirror ram at $4000
        /// </summary>
        MirrorRamAt4000 = 0b00000000_10000000,

        /// <summary>
        /// activision banking
        /// </summary>
        ActivisionBanking = 0b00000001_00000000,

        /// <summary>
        /// absolute banking
        /// </summary>
        AbsoluteBanking = 0b00000010_00000000,

        /// <summary>
        /// pokey at $440
        /// </summary>
        PokeyAt440 = 0b00000100_00000000,

        /// <summary>
        /// ym2151 at $460/$461
        /// </summary>
        Ym2151At460461 = 0b00001000_00000000,

        /// <summary>
        /// souper
        /// </summary>
        Souper = 0b00010000_00000000,

        /// <summary>
        /// banksets
        /// </summary>
        Banksets = 0b00100000_00000000,

        /// <summary>
        /// halt banked ram
        /// </summary>
        HaltBankedRam = 0b01000000_00000000,

        /// <summary>
        /// pokey@800
        /// </summary>
        PokeyAt800 = 0b10000000_00000000,
    }

    /// <summary>
    /// Controller type
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    public enum ControllerType : byte
    {
        /// <summary>
        /// none
        /// </summary>
        None = 0,

        /// <summary>
        /// 7800 joystick
        /// </summary>
        Joystick = 1,

        /// <summary>
        /// lightgun
        /// </summary>
        Lightgun = 2,

        /// <summary>
        /// paddle
        /// </summary>
        Paddle = 3,

        /// <summary>
        /// trakball
        /// </summary>
        Trakball = 4,

        /// <summary>
        /// 2600 joystick
        /// </summary>
        VcsJoystick = 5,

        /// <summary>
        /// 2600 driving
        /// </summary>
        VcsDriving = 6,

        /// <summary>
        /// 2600 keypad
        /// </summary>
        VcsKeypad = 7,

        /// <summary>
        /// ST mouse
        /// </summary>
        STMouse = 8,

        /// <summary>
        /// Amiga mouse
        /// </summary>
        AmigaMouse = 9,

        /// <summary>
        /// AtariVox/SaveKey
        /// </summary>
        AtariVoxSaveKey = 10,

        /// <summary>
        /// SNES2Atari
        /// </summary>
        SNES2Atari = 11,

        /// <summary>
        /// Mega7800
        /// </summary>
        Mega7800 = 12,
    }

    /// <summary>
    /// Interrupt (v4+ only)
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    [Flags]
    public enum Interrupt : ushort
    {
        None = 0b00000000,

        /// <summary>
        /// POKEY 1 (@450 | @800 | @4000)
        /// </summary>
        Pokey1 = 0b00000001,

        /// <summary>
        /// POKEY 2 (@440)
        /// </summary>
        Pokey2 = 0b00000010,

        /// <summary>
        /// YM2151
        /// </summary>
        YM2151 = 0b00000100,
    }

    /// <summary>
    /// Mapper (v4+ only)
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    public enum Mapper : byte
    {
        Linear = 0,
        SuperGame = 1,
        Activision = 2,
        Absolute = 3,
        Souper = 4,
    }

    /// <summary>
    /// Mapper options/details (v4+ only)
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    [Flags]
    public enum MapperOptions : byte
    {
        #region Linear

        /// <summary>
        /// Option at @4000 - none
        /// </summary>
        LinearNone = 0b00000000,

        /// <summary>
        /// Option at @4000 - 16K RAM
        /// </summary>
        Linear16KRAM = 0b00000001,

        /// <summary>
        /// Option at @4000 - 8K EXRAM/A8
        /// </summary>
        Linear8KEXRAMA8 = 0b00000010,

        /// <summary>
        /// Option at @4000 - 32K EXRAM/M2
        /// </summary>
        Linear32KEXRAMM2 = 0b00000011,

        #endregion

        #region SuperGame

        /// <summary>
        /// Option at @4000 - none
        /// </summary>
        SuperGameNone = 0b00000000,

        /// <summary>
        /// Option at @4000 - 16K RAM
        /// </summary>
        SuperGame16KRAM = 0b00000001,

        /// <summary>
        /// Option at @4000 - 8K EXRAM/A8
        /// </summary>
        SuperGame8KEXRAMA8 = 0b00000010,

        /// <summary>
        /// Option at @4000 - 32K EXRAM/M2
        /// </summary>
        SuperGame32KEXRAMM2 = 0b00000011,

        /// <summary>
        /// Option at @4000 - EXROM
        /// </summary>
        SuperGameEXROM = 0b00000100,

        /// <summary>
        /// Option at @4000 - EXFIX
        /// </summary>
        SuperGameEXFIX = 0b00000101,

        /// <summary>
        /// Option at @4000 - 32k EXRAM/X2
        /// </summary>
        SuperGame32KEXRAMX2 = 0b00000110,

        #endregion

        /// <summary>
        /// Bankset ROM
        /// </summary>
        BanksetRom = 0b10000000,
    }

    /// <summary>
    /// Save device
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    public enum SaveDevice : byte
    {
        None = 0b00000000,

        /// <summary>
        /// HSC
        /// </summary>
        HSC = 0b00000001,

        /// <summary>
        /// SaveKey/AtariVox
        /// </summary>
        SaveKeyAtariVox = 0b00000010,
    }

    /// <summary>
    /// Slot passthrough device
    /// </summary>
    public enum SlotPassthroughDevice : byte
    {
        None = 0b00000000,
        XM = 0b00000001,
    }

    /// <summary>
    /// TV type
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    [Flags]
    public enum TVType : byte
    {
        NTSC = 0b00000000,
        PAL = 0b00000001,

        Component = 0b00000000,
        Composite = 0b00000010,

        SingleRegion = 0b00000000,
        MultiRegion = 0b00000100,
    }
}
