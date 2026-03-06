namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// Console Type
    /// </summary>
    /// <remarks>Actually only 2 bits (bits 0-1 of flag 7)</remarks>
    public enum ConsoleType : byte
    {
        /// <summary>
        /// Nintendo Entertainment System/Family Computer
        /// </summary>
        StandardSystem = 0x00,

        /// <summary>
        /// VS Unisystem
        /// </summary>
        VSUnisystem = 0x01,

        /// <summary>
        /// PlayChoice-10 (8 KB of Hint Screen data stored after CHR data)
        /// </summary>
        PlayChoice10 = 0x02,

        /// <summary>
        /// Extended Console Type
        /// </summary>
        ExtendedConsoleType = 0x03,
    }

    /// <summary>
    /// CPU/PPU Timing
    /// </summary>
    public enum CPUPPUTiming : byte
    {
        /// <summary>
        /// NTSC NES
        /// </summary>
        RP2C02 = 0x00,

        /// <summary>
        /// Licensed PAL NES
        /// </summary>
        RP2C07 = 0x01,

        /// <summary>
        /// Multiple-region
        /// </summary>
        MultipleRegion = 0x02,

        /// <summary>
        /// Dendy
        /// </summary>
        UA6538 = 0x03,
    }

    /// <summary>
    /// Default Expansion Device
    /// </summary>
    public enum DefaultExpansionDevice : byte
    {
        /// <summary>
        /// Unspecified
        /// </summary>
        Unspecified = 0x00,

        /// <summary>
        /// Standard NES/Famicom controllers
        /// </summary>
        StandardControllers = 0x01,

        /// <summary>
        /// NES Four Score/Satellite with two additional standard controllers
        /// </summary>
        NESFourScore = 0x02,

        /// <summary>
        /// Famicom Four Players Adapter with two additional standard controllers
        /// using the "simple" protocol
        /// </summary>
        FamicomFourPlayersAdapter = 0x03,

        /// <summary>
        /// Vs. System (1P via $4016)
        /// </summary>
        VsSystem4016 = 0x04,

        /// <summary>
        /// Vs. System (1P via $4017)
        /// </summary>
        VsSystem4017 = 0x05,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved06 = 0x06,

        /// <summary>
        /// Vs. Zapper
        /// </summary>
        VsZapper = 0x07,

        /// <summary>
        /// Zapper ($4017)
        /// </summary>
        Zapper4017 = 0x08,

        /// <summary>
        /// Two Zappers
        /// </summary>
        TwoZappers = 0x09,

        /// <summary>
        /// Bandai Hyper Shot Lightgun
        /// </summary>
        BandaiHyperShotLightgun = 0x0A,

        /// <summary>
        /// Power Pad Side A
        /// </summary>
        PowerPadSideA = 0x0B,

        /// <summary>
        /// Power Pad Side B
        /// </summary>
        PowerPadSideB = 0x0C,

        /// <summary>
        /// Family Trainer Side A
        /// </summary>
        FamilyTrainerSideA = 0x0D,

        /// <summary>
        /// Family Trainer Side B
        /// </summary>
        FamilyTrainerSideB = 0x0E,

        /// <summary>
        /// Arkanoid Vaus Controller (NES)
        /// </summary>
        ArkanoidVausControllerNES = 0x0F,

        /// <summary>
        /// Arkanoid Vaus Controller (Famicom)
        /// </summary>
        ArkanoidVausControllerFamicom = 0x10,

        /// <summary>
        /// Two Vaus Controllers plus Famicom Data Recorder
        /// </summary>
        TwoVausControllersPlusFamicomDataRecorder = 0x11,

        /// <summary>
        /// Konami Hyper Shot Controller
        /// </summary>
        KonamiHyperShotController = 0x12,

        /// <summary>
        /// Coconuts Pachinko Controller
        /// </summary>
        CoconutsPachinkoController = 0x13,

        /// <summary>
        /// Exciting Boxing Punching Bag (Blowup Doll)
        /// </summary>
        ExcitingBoxingPunchingBag = 0x14,

        /// <summary>
        /// Jissen Mahjong Controller
        /// </summary>
        JissenMahjongController = 0x15,

        /// <summary>
        /// 米澤 (Yonezawa) Party Tap
        /// </summary>
        YonezawaPartyTap = 0x16,

        /// <summary>
        /// Oeka Kids Tablet
        /// </summary>
        OekaKidsTablet = 0x17,

        /// <summary>
        /// Sunsoft Barcode Battler
        /// </summary>
        SunsoftBarcodeBattler = 0x18,

        /// <summary>
        /// Miracle Piano Keyboard
        /// </summary>
        MiraclePianoKeyboard = 0x19,

        /// <summary>
        /// Pokkun Moguraa Tap-tap Mat (Whack-a-Mole Mat and Mallet)
        /// </summary>
        PokkunMoguraaTapTapMat = 0x1A,

        /// <summary>
        /// Top Rider (Inflatable Bicycle)
        /// </summary>
        TopRider = 0x1B,

        /// <summary>
        /// Double-Fisted (Requires or allows use of two controllers by one player)
        /// </summary>
        DoubleFisted = 0x1C,

        /// <summary>
        /// Famicom 3D System
        /// </summary>
        Famicom3DSystem = 0x1D,

        /// <summary>
        /// Doremikko Keyboard
        /// </summary>
        DoremikkoKeyboard = 0x1E,

        /// <summary>
        /// R.O.B. Gyromite
        /// </summary>
        ROBGyromite = 0x1F,

        /// <summary>
        /// Famicom Data Recorder ("silent" keyboard)
        /// </summary>
        FamicomDataRecorder = 0x20,

        /// <summary>
        /// ASCII Turbo File
        /// </summary>
        ASCIITurboFile = 0x21,

        /// <summary>
        /// IGS Storage Battle Box
        /// </summary>
        IGSStorageBattleBox = 0x22,

        /// <summary>
        /// Family BASIC Keyboard plus Famicom Data Recorder
        /// </summary>
        FamilyBASICKeyboardPlusFamicomDataRecorder = 0x23,

        /// <summary>
        /// 东达 (Dōngdá) PEC Keyboard
        /// </summary>
        DongdaPECKeyboard = 0x24,

        /// <summary>
        /// 普澤 (Pǔzé, a.k.a. Bit Corp.) Bit-79 Keyboard
        /// </summary>
        BitCorpBit79Keyboard = 0x25,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard
        /// </summary>
        SuborKeyboard = 0x26,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Macro Winners Mouse
        /// </summary>
        SuborKeyboardPlusMacroWinnersMouse = 0x27,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Subor Mouse via $4016
        /// </summary>
        SuborKeyboardPlusSuborMouse4016 = 0x28,

        /// <summary>
        /// SNES Mouse ($4016)
        /// </summary>
        SNESMouse4016 = 0x29,

        /// <summary>
        /// Multicart
        /// </summary>
        Multicart = 0x2A,

        /// <summary>
        /// Two SNES controllers replacing the two standard NES controllers
        /// </summary>
        TwoSNESControllers = 0x2B,

        /// <summary>
        /// RacerMate Bicycle
        /// </summary>
        RacerMateBicycle = 0x2C,

        /// <summary>
        /// U-Force
        /// </summary>
        UForce = 0x2D,

        /// <summary>
        /// R.O.B. Stack-Up
        /// </summary>
        ROBStackUp = 0x2E,

        /// <summary>
        /// City Patrolman Lightgun
        /// </summary>
        CityPatrolmanLightgun = 0x2F,

        /// <summary>
        /// Sharp C1 Cassette Interface
        /// </summary>
        SharpC1CassetteInterface = 0x30,

        /// <summary>
        /// Standard Controller with swapped Left-Right/Up-Down/B-A
        /// </summary>
        StandardControllerWithSwappedInputs = 0x31,

        /// <summary>
        /// Excalibur Sudoku Pad
        /// </summary>
        ExcaliburSudokuPad = 0x32,

        /// <summary>
        /// ABL Pinball
        /// </summary>
        ABLPinball = 0x33,

        /// <summary>
        /// Golden Nugget Casino extra buttons
        /// </summary>
        GoldenNuggetCasinoExtraButtons = 0x34,

        /// <summary>
        /// 科达 (Kēdá) Keyboard
        /// </summary>
        KedaKeyboard = 0x35,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Subor Mouse via $4017
        /// </summary>
        SuborKeyboardPlusSuborMouse4017 = 0x36,

        /// <summary>
        /// Port test controller
        /// </summary>
        PortTestController = 0x37,

        /// <summary>
        /// Bandai Multi Game Player Gamepad buttons
        /// </summary>
        BandaiMultiGamePlayerGamepad = 0x38,

        /// <summary>
        /// Venom TV Dance Mat
        /// </summary>
        VenomTVDanceMat = 0x39,

        /// <summary>
        /// LG TV Remote Control
        /// </summary>
        LGTVRemoteControl = 0x3A,

        /// <summary>
        /// Famicom Network Controller
        /// </summary>
        FamicomNetworkController = 0x3B,

        /// <summary>
        /// King Fishing Controller
        /// </summary>
        KingFishingController = 0x3C,

        /// <summary>
        /// Croaky Karaoke Controller
        /// </summary>
        CroakyKaraokeController = 0x3D,

        /// <summary>
        /// 科王 (Kēwáng, a.k.a. Kingwon) Keyboard
        /// </summary>
        KingwonKeyboard = 0x3E,

        /// <summary>
        /// 泽诚 (Zéchéng) Keyboard
        /// </summary>
        ZechengKeyboard = 0x3F,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus L90-rotated PS/2 mouse in $4017
        /// </summary>
        SuborKeyboardPlusL90RotatedPS2Mouse4017 = 0x40,

        /// <summary>
        /// PS/2 Keyboard in UM6578 PS/2 port, PS/2 Mouse via $4017
        /// </summary>
        PS2KeyboardInUM6578PS2PortPS2Mouse4017 = 0x41,

        /// <summary>
        /// PS/2 Mouse in UM6578 PS/2 port
        /// </summary>
        PS2MouseInUM6578PS2Port = 0x42,

        /// <summary>
        /// 裕兴 (Yùxìng) Mouse via $4016
        /// </summary>
        YuxingMouse4016 = 0x43,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus 裕兴 (Yùxìng)
        /// Mouse mouse in $4016
        /// </summary>
        SuborKeyboardPlusYuxingMouse4016 = 0x44,

        /// <summary>
        /// Gigggle TV Pump
        /// </summary>
        GigggleTVPump = 0x45,

        /// <summary>
        /// 步步高 (Bùbùgāo, a.k.a. BBK) Keyboard plus R90-rotated PS/2 mouse in $4017
        /// </summary>
        BBKKeyboardPlusR90RotatedPS2Mouse4017 = 0x46,

        /// <summary>
        /// Magical Cooking
        /// </summary>
        MagicalCooking = 0x47,

        /// <summary>
        /// SNES Mouse ($4017)
        /// </summary>
        SNESMouse4017 = 0x48,

        /// <summary>
        /// Zapper ($4016)
        /// </summary>
        Zapper4016 = 0x49,

        /// <summary>
        /// Arkanoid Vaus Controller (Prototype)
        /// </summary>
        ArkanoidVausControllerPrototype = 0x4A,

        /// <summary>
        /// TV 麻雀 Game (TV Mahjong Game) Controller
        /// </summary>
        TVMahjongGameController = 0x4B,

        /// <summary>
        /// 麻雀激闘伝説 (Mahjong Gekitou Densetsu) Controller
        /// </summary>
        MahjongGekitouDensetsuController = 0x4C,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus X-inverted PS/2 mouse in $4017
        /// </summary>
        SuborKeyboardPlusXInvertedPS2Mouse4017 = 0x4D,

        /// <summary>
        /// IBM PC/XT Keyboard
        /// </summary>
        IBMPCXTKeyboard = 0x4E,

        /// <summary>
        /// 小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Mega Book Mouse
        /// </summary>
        SuborKeyboardPlusMegaBookMouse = 0x4F,
    }

    /// <summary>
    /// Extended Console Type
    /// </summary>
    /// <remarks>Actually only 4 bits (bits 0-3 of flag 13)</remarks>
    public enum ExtendedConsoleType : byte
    {
        /// <summary>
        /// Regular NES/Famicom/Dendy
        /// </summary>
        RegularSystem = 0x00,

        /// <summary>
        /// Nintendo Vs. System
        /// </summary>
        NintendoVsSystem = 0x01,

        /// <summary>
        /// Playchoice 10
        /// </summary>
        Playchoice10 = 0x02,

        /// <summary>
        /// Regular Famiclone, but with CPU that supports Decimal Mode
        /// </summary>
        RegularFamicloneDecimalMode = 0x03,

        /// <summary>
        /// Regular NES/Famicom with EPSM module or plug-through cartridge
        /// </summary>
        RegularNESWithEPSM = 0x04,

        /// <summary>
        /// V.R. Technology VT01 with red/cyan STN palette
        /// </summary>
        VRTechnologyVT01 = 0x05,

        /// <summary>
        /// V.R. Technology VT02
        /// </summary>
        VRTechnologyVT02 = 0x06,

        /// <summary>
        /// V.R. Technology VT03
        /// </summary>
        VRTechnologyVT03 = 0x07,

        /// <summary>
        /// V.R. Technology VT09
        /// </summary>
        VRTechnologyVT09 = 0x08,

        /// <summary>
        /// V.R. Technology VT32
        /// </summary>
        VRTechnologyVT32 = 0x09,

        /// <summary>
        /// V.R. Technology VT369
        /// </summary>
        VRTechnologyVT369 = 0x0A,

        /// <summary>
        /// UMC UM6578
        /// </summary>
        UMCUM6578 = 0x0B,

        /// <summary>
        /// Famicom Network System
        /// </summary>
        FamicomNetworkSystem = 0x0C,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedD = 0x0D,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedE = 0x0E,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedF = 0x0F,
    }

    /// <summary>
    /// Nametable arrangement
    /// </summary>
    /// <remarks>Actually only 1 bit (bit 0 of flag 6)</remarks>
    public enum NametableArrangement : byte
    {
        /// <summary>
        /// Vertical arrangement ("horizontal mirrored") or mapper-controlled
        /// </summary>
        /// <remarks>CIRAM A10 = PPU A11</remarks>
        Vertical = 0x00,

        /// <summary>
        /// Horizontal arrangement ("vertically mirrored")
        /// </summary>
        /// <remarks>CIRAM A10 = PPU A10</remarks>
        Horizontal = 0x01,
    }

    /// <summary>
    /// PRG RAM ($6000-$7FFF)
    /// </summary>
    /// <remarks>Actually only 1 bit (bit 4 of flag 10)</remarks>
    public enum PRGRAMPresent : byte
    {
        Present = 0x00,
        NotPresent = 0x01,
    }

    /// <summary>
    /// TV system (rarely used extension)
    /// </summary>
    /// <remarks>Byte 9</remarks>
    public enum TVSystem : byte
    {
        NTSC = 0x00,
        PAL = 0x01,
    }

    /// <summary>
    /// TV system with extended values
    /// </summary>
    /// <remarks>Actually only 2 bits (bits 0-1 of flag 10)</remarks>
    public enum TVSystemExtended : byte
    {
        NTSC = 0x00,
        DualCompatible1 = 0x01,
        PAL = 0x02,
        DualCompatible3 = 0x03,
    }

    /// <summary>
    /// Vs. Hardware Type
    /// </summary>
    /// <remarks>Actually only 4 bits (bits 4-7 of flag 13)</remarks>
    public enum VsHardwareType : byte
    {
        /// <summary>
        /// Vs. Unisystem (normal)
        /// </summary>
        VsUnisystem = 0x00,

        /// <summary>
        /// Vs. Unisystem (RBI Baseball protection)
        /// </summary>
        VsUnisystemRBIBaseballProtection = 0x01,

        /// <summary>
        /// Vs. Unisystem (TKO Boxing protection)
        /// </summary>
        VsUnisystemTKOBoxingProtection = 0x02,

        /// <summary>
        /// Vs. Unisystem (Super Xevious protection)
        /// </summary>
        VsUnisystemSuperXeviousProtection = 0x03,

        /// <summary>
        /// Vs. Unisystem (Vs. Ice Climber Japan protection)
        /// </summary>
        VsUnisystemVsIceClimberJapanProtection = 0x04,

        /// <summary>
        /// Vs. Dual System (normal)
        /// </summary>
        VsDualSystem = 0x05,

        /// <summary>
        /// Vs. Dual System (Raid on Bungeling Bay protection)
        /// </summary>
        VsDualSystemRaidOnBungelingBayProtection = 0x06,
    }

    /// <summary>
    /// Vs. System Type
    /// </summary>
    /// <remarks>Actually only 4 bits (bits 0-3 of flag 13)</remarks>
    public enum VsSystemType : byte
    {
        /// <summary>
        /// Any RP2C03/RC2C03 variant
        /// </summary>
        AnyRP2C03RC2C03Variant = 0x00,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved1 = 0x01,

        /// <summary>
        /// RP2C04-0001
        /// </summary>
        RP2C040001 = 0x02,

        /// <summary>
        /// RP2C04-0002
        /// </summary>
        RP2C040002 = 0x03,

        /// <summary>
        /// RP2C04-0003
        /// </summary>
        RP2C040003 = 0x04,

        /// <summary>
        /// RP2C04-0004
        /// </summary>
        RP2C040004 = 0x05,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved6 = 0x06,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved7 = 0x07,

        /// <summary>
        /// RC2C05-01 (signature unknown)
        /// </summary>
        RC2C0501 = 0x08,

        /// <summary>
        /// RC2C05-02 ($2002 AND $3F =$3D)
        /// </summary>
        RC2C0502 = 0x09,

        /// <summary>
        /// RC2C05-03 ($2002 AND $1F =$1C)
        /// </summary>
        RC2C0503 = 0x0A,

        /// <summary>
        /// RC2C05-04 ($2002 AND $1F =$1B)
        /// </summary>
        RC2C0504 = 0x0B,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedC = 0x0C,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedD = 0x0D,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedE = 0x0E,

        /// <summary>
        /// Reserved
        /// </summary>
        ReservedF = 0x0F,
    }
}
