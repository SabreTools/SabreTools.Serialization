using SabreTools.Data.Models.NES;

namespace SabreTools.Data.Extensions
{
    public static class NESCart
    {
        /// <summary>
        /// Convert a <see cref="ConsoleType"/> value to string
        /// </summary>
        public static string FromConsoleType(this ConsoleType type)
        {
            return type switch
            {
                ConsoleType.StandardSystem => "Nintendo Entertainment System/Family Computer",
                ConsoleType.VSUnisystem => "VS Unisystem",
                ConsoleType.PlayChoice10 => "PlayChoice-10 (8 KB of Hint Screen data stored after CHR data)",
                ConsoleType.ExtendedConsoleType => "Extended Console Type",
                _ => $"Unknown {(byte)type}",
            };
        }

        /// <summary>
        /// Convert a <see cref="CPUPPUTiming"/> value to string
        /// </summary>
        public static string FromCPUPPUTiming(this CPUPPUTiming timing)
        {
            return timing switch
            {
                CPUPPUTiming.RP2C02 => "RP2C02 (NTSC NES)",
                CPUPPUTiming.RP2C07 => "RP2C07 (Licensed PAL NES)",
                CPUPPUTiming.MultipleRegion => "Multiple-region",
                CPUPPUTiming.UA6538 => "UA6538 (Dendy)",
                _ => $"Unknown {(byte)timing}",
            };
        }

        /// <summary>
        /// Convert a <see cref="DefaultExpansionDevice"/> value to string
        /// </summary>
        public static string FromDefaultExpansionDevice(this DefaultExpansionDevice device)
        {
            return device switch
            {
                DefaultExpansionDevice.Unspecified => "Unspecified",
                DefaultExpansionDevice.StandardControllers => "Standard NES/Famicom controllers",
                DefaultExpansionDevice.NESFourScore => "NES Four Score/Satellite with two additional standard controllers",
                DefaultExpansionDevice.FamicomFourPlayersAdapter => "Famicom Four Players Adapter with two additional standard controllers using the 'simple' protocol",
                DefaultExpansionDevice.VsSystem4016 => "Vs. System (1P via $4016)",
                DefaultExpansionDevice.VsSystem4017 => "Vs. System (1P via $4017)",
                DefaultExpansionDevice.Reserved06 => "Reserved (0x06)",
                DefaultExpansionDevice.VsZapper => "Vs. Zapper",
                DefaultExpansionDevice.Zapper4017 => "Zapper ($4017)",
                DefaultExpansionDevice.TwoZappers => "Two Zappers",
                DefaultExpansionDevice.BandaiHyperShotLightgun => "Bandai Hyper Shot Lightgun",
                DefaultExpansionDevice.PowerPadSideA => "Power Pad Side A",
                DefaultExpansionDevice.PowerPadSideB => "Power Pad Side B",
                DefaultExpansionDevice.FamilyTrainerSideA => "Family Trainer Side A",
                DefaultExpansionDevice.FamilyTrainerSideB => "Family Trainer Side B",
                DefaultExpansionDevice.ArkanoidVausControllerNES => "Arkanoid Vaus Controller (NES)",
                DefaultExpansionDevice.ArkanoidVausControllerFamicom => "Arkanoid Vaus Controller (Famicom)",
                DefaultExpansionDevice.TwoVausControllersPlusFamicomDataRecorder => "Two Vaus Controllers plus Famicom Data Recorder",
                DefaultExpansionDevice.KonamiHyperShotController => "Konami Hyper Shot Controller",
                DefaultExpansionDevice.CoconutsPachinkoController => "Coconuts Pachinko Controller",
                DefaultExpansionDevice.ExcitingBoxingPunchingBag => "Exciting Boxing Punching Bag (Blowup Doll)",
                DefaultExpansionDevice.JissenMahjongController => "Jissen Mahjong Controller",
                DefaultExpansionDevice.YonezawaPartyTap => "米澤 (Yonezawa) Party Tap",
                DefaultExpansionDevice.OekaKidsTablet => "Oeka Kids Tablet",
                DefaultExpansionDevice.SunsoftBarcodeBattler => "Sunsoft Barcode Battler",
                DefaultExpansionDevice.MiraclePianoKeyboard => "Miracle Piano Keyboard",
                DefaultExpansionDevice.PokkunMoguraaTapTapMat => "Pokkun Moguraa Tap-tap Mat (Whack-a-Mole Mat and Mallet)",
                DefaultExpansionDevice.TopRider => "Top Rider (Inflatable Bicycle)",
                DefaultExpansionDevice.DoubleFisted => "Double-Fisted (Requires or allows use of two controllers by one player)",
                DefaultExpansionDevice.Famicom3DSystem => "Famicom 3D System",
                DefaultExpansionDevice.DoremikkoKeyboard => "Doremikko Keyboard",
                DefaultExpansionDevice.ROBGyromite => "R.O.B. Gyromite",
                DefaultExpansionDevice.FamicomDataRecorder => "Famicom Data Recorder ('silent' keyboard)",
                DefaultExpansionDevice.ASCIITurboFile => "ASCII Turbo File",
                DefaultExpansionDevice.IGSStorageBattleBox => "IGS Storage Battle Box",
                DefaultExpansionDevice.FamilyBASICKeyboardPlusFamicomDataRecorder => "Family BASIC Keyboard plus Famicom Data Recorder",
                DefaultExpansionDevice.DongdaPECKeyboard => "东达 (Dōngdá) PEC Keyboard",
                DefaultExpansionDevice.BitCorpBit79Keyboard => "普澤 (Pǔzé, a.k.a. Bit Corp.) Bit-79 Keyboard",
                DefaultExpansionDevice.SuborKeyboard => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard",
                DefaultExpansionDevice.SuborKeyboardPlusMacroWinnersMouse => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Macro Winners Mouse",
                DefaultExpansionDevice.SuborKeyboardPlusSuborMouse4016 => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Subor Mouse via $4016",
                DefaultExpansionDevice.SNESMouse4016 => "SNES Mouse ($4016)",
                DefaultExpansionDevice.Multicart => "Multicart",
                DefaultExpansionDevice.TwoSNESControllers => "Two SNES controllers replacing the two standard NES controllers",
                DefaultExpansionDevice.RacerMateBicycle => "RacerMate Bicycle",
                DefaultExpansionDevice.UForce => "U-Force",
                DefaultExpansionDevice.ROBStackUp => "R.O.B. Stack-Up",
                DefaultExpansionDevice.CityPatrolmanLightgun => "City Patrolman Lightgun",
                DefaultExpansionDevice.SharpC1CassetteInterface => "Sharp C1 Cassette Interface",
                DefaultExpansionDevice.StandardControllerWithSwappedInputs => "Standard Controller with swapped Left-Right/Up-Down/B-A",
                DefaultExpansionDevice.ExcaliburSudokuPad => "Excalibur Sudoku Pad",
                DefaultExpansionDevice.ABLPinball => "ABL Pinball",
                DefaultExpansionDevice.GoldenNuggetCasinoExtraButtons => "Golden Nugget Casino extra buttons",
                DefaultExpansionDevice.KedaKeyboard => "科达 (Kēdá) Keyboard",
                DefaultExpansionDevice.SuborKeyboardPlusSuborMouse4017 => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Subor Mouse via $4017",
                DefaultExpansionDevice.PortTestController => "Port test controller",
                DefaultExpansionDevice.BandaiMultiGamePlayerGamepad => "Bandai Multi Game Player Gamepad buttons",
                DefaultExpansionDevice.VenomTVDanceMat => "Venom TV Dance Mat",
                DefaultExpansionDevice.LGTVRemoteControl => "LG TV Remote Control",
                DefaultExpansionDevice.FamicomNetworkController => "Famicom Network Controller",
                DefaultExpansionDevice.KingFishingController => "King Fishing Controller",
                DefaultExpansionDevice.CroakyKaraokeController => "Croaky Karaoke Controller",
                DefaultExpansionDevice.KingwonKeyboard => "科王 (Kēwáng, a.k.a. Kingwon) Keyboard",
                DefaultExpansionDevice.ZechengKeyboard => "泽诚 (Zéchéng) Keyboard",
                DefaultExpansionDevice.SuborKeyboardPlusL90RotatedPS2Mouse4017 => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus L90-rotated PS/2 mouse in $4017",
                DefaultExpansionDevice.PS2KeyboardInUM6578PS2PortPS2Mouse4017 => "PS/2 Keyboard in UM6578 PS/2 port, PS/2 Mouse via $4017",
                DefaultExpansionDevice.PS2MouseInUM6578PS2Port => "PS/2 Mouse in UM6578 PS/2 port",
                DefaultExpansionDevice.YuxingMouse4016 => "裕兴 (Yùxìng) Mouse via $4016",
                DefaultExpansionDevice.SuborKeyboardPlusYuxingMouse4016 => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus 裕兴 (Yùxìng) Mouse mouse in $4016",
                DefaultExpansionDevice.GigggleTVPump => "Gigggle TV Pump",
                DefaultExpansionDevice.BBKKeyboardPlusR90RotatedPS2Mouse4017 => "步步高 (Bùbùgāo, a.k.a. BBK) Keyboard plus R90-rotated PS/2 mouse in $4017",
                DefaultExpansionDevice.MagicalCooking => "Magical Cooking",
                DefaultExpansionDevice.SNESMouse4017 => "SNES Mouse ($4017)",
                DefaultExpansionDevice.Zapper4016 => "Zapper ($4016)",
                DefaultExpansionDevice.ArkanoidVausControllerPrototype => "Arkanoid Vaus Controller (Prototype)",
                DefaultExpansionDevice.TVMahjongGameController => "TV 麻雀 Game (TV Mahjong Game) Controller",
                DefaultExpansionDevice.MahjongGekitouDensetsuController => "麻雀激闘伝説 (Mahjong Gekitou Densetsu) Controller",
                DefaultExpansionDevice.SuborKeyboardPlusXInvertedPS2Mouse4017 => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus X-inverted PS/2 mouse in $4017",
                DefaultExpansionDevice.IBMPCXTKeyboard => "IBM PC/XT Keyboard",
                DefaultExpansionDevice.SuborKeyboardPlusMegaBookMouse => "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Mega Book Mouse",
                _ => $"Unknown {(byte)device}",
            };
        }

        /// <summary>
        /// Convert a <see cref="ExtendedConsoleType"/> value to string
        /// </summary>
        public static string FromExtendedConsoleType(this ExtendedConsoleType type)
        {
            return type switch
            {
                ExtendedConsoleType.RegularSystem => "Regular NES/Famicom/Dendy",
                ExtendedConsoleType.NintendoVsSystem => "Nintendo Vs. System",
                ExtendedConsoleType.Playchoice10 => "Playchoice 10",
                ExtendedConsoleType.RegularFamicloneDecimalMode => "Regular Famiclone, but with CPU that supports Decimal Mode",
                ExtendedConsoleType.RegularNESWithEPSM => "Regular NES/Famicom with EPSM module or plug-through cartridge",
                ExtendedConsoleType.VRTechnologyVT01 => "V.R. Technology VT01 with red/cyan STN palette",
                ExtendedConsoleType.VRTechnologyVT02 => "V.R. Technology VT02",
                ExtendedConsoleType.VRTechnologyVT03 => "V.R. Technology VT03",
                ExtendedConsoleType.VRTechnologyVT09 => "V.R. Technology VT09",
                ExtendedConsoleType.VRTechnologyVT32 => "V.R. Technology VT32",
                ExtendedConsoleType.VRTechnologyVT369 => "V.R. Technology VT369",
                ExtendedConsoleType.UMCUM6578 => "UMC UM6578",
                ExtendedConsoleType.FamicomNetworkSystem => "Famicom Network System",
                ExtendedConsoleType.ReservedD => "Reserved (0x0D)",
                ExtendedConsoleType.ReservedE => "Reserved (0x0E)",
                ExtendedConsoleType.ReservedF => "Reserved (0x0F)",
                _ => $"Unknown {(byte)type}",
            };
        }

        /// <summary>
        /// Convert a <see cref="NametableArrangement"/> value to string
        /// </summary>
        public static string FromNametableArrangement(this NametableArrangement type)
        {
            return type switch
            {
                NametableArrangement.Vertical => "Vertical",
                NametableArrangement.Horizontal => "Horizontal",
                _ => $"Unknown {(byte)type}",
            };
        }

        /// <summary>
        /// Convert a <see cref="TVSystem"/> value to string
        /// </summary>
        public static string FromTVSystem(this TVSystem system)
        {
            return system switch
            {
                TVSystem.NTSC => "NTSC",
                TVSystem.PAL => "PAL",
                _ => $"Unknown {(byte)system}",
            };
        }

         /// <summary>
        /// Convert a <see cref="TVSystemExtended"/> value to string
        /// </summary>
        public static string FromTVSystemExtended(this TVSystemExtended system)
        {
            return system switch
            {
                TVSystemExtended.NTSC => "NTSC",
                TVSystemExtended.DualCompatible1 => "Dual-compatible (0x01)",
                TVSystemExtended.PAL => "PAL",
                TVSystemExtended.DualCompatible3 => "Dual-compatible (0x03)",
                _ => $"Unknown {(byte)system}",
            };
        }

        /// <summary>
        /// Convert a <see cref="VsHardwareType"/> value to string
        /// </summary>
        public static string FromVsHardwareType(this VsHardwareType type)
        {
            return type switch
            {
                VsHardwareType.VsUnisystem => "Vs. Unisystem (normal)",
                VsHardwareType.VsUnisystemRBIBaseballProtection => "Vs. Unisystem (RBI Baseball protection)",
                VsHardwareType.VsUnisystemTKOBoxingProtection => "Vs. Unisystem (TKO Boxing protection)",
                VsHardwareType.VsUnisystemSuperXeviousProtection => "Vs. Unisystem (Super Xevious protection)",
                VsHardwareType.VsUnisystemVsIceClimberJapanProtection => "Vs. Unisystem (Vs. Ice Climber Japan protection)",
                VsHardwareType.VsDualSystem => "Vs. Dual System (normal)",
                VsHardwareType.VsDualSystemRaidOnBungelingBayProtection => "Vs. Dual System (Raid on Bungeling Bay protection)",
                _ => $"Unknown {(byte)type}",
            };
        }

        /// <summary>
        /// Convert a <see cref="VsSystemType"/> value to string
        /// </summary>
        public static string FromVsSystemType(this VsSystemType type)
        {
            return type switch
            {
                VsSystemType.AnyRP2C03RC2C03Variant => "Any RP2C03/RC2C03 variant",
                VsSystemType.Reserved1 => "Reserved (0x01)",
                VsSystemType.RP2C040001 => "RP2C04-0001",
                VsSystemType.RP2C040002 => "RP2C04-0002",
                VsSystemType.RP2C040003 => "RP2C04-0003",
                VsSystemType.RP2C040004 => "RP2C04-0004",
                VsSystemType.Reserved6 => "Reserved (0x06)",
                VsSystemType.Reserved7 => "Reserved (0x07)",
                VsSystemType.RC2C0501 => "RC2C05-01 (signature unknown)",
                VsSystemType.RC2C0502 => "RC2C05-02 ($2002 AND $3F =$3D)",
                VsSystemType.RC2C0503 => "RC2C05-03 ($2002 AND $1F =$1C)",
                VsSystemType.RC2C0504 => "RC2C05-04 ($2002 AND $1F =$1B)",
                VsSystemType.ReservedC => "Reserved (0x0C)",
                VsSystemType.ReservedD => "Reserved (0x0D)",
                VsSystemType.ReservedE => "Reserved (0x0E)",
                VsSystemType.ReservedF => "Reserved (0x0F)",
                _ => $"Unknown {(byte)type}",
            };
        }
    }
}
