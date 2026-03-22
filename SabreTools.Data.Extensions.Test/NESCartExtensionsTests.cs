using SabreTools.Data.Models.NES;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class NESCartExtensionsTests
    {
        [Theory]
        [InlineData(ConsoleType.StandardSystem, "Nintendo Entertainment System/Family Computer")]
        [InlineData(ConsoleType.VSUnisystem, "VS Unisystem")]
        [InlineData(ConsoleType.PlayChoice10, "PlayChoice-10 (8 KB of Hint Screen data stored after CHR data)")]
        [InlineData(ConsoleType.ExtendedConsoleType, "Extended Console Type")]
        public void FromConsoleTypeTest(ConsoleType type, string expected)
        {
            string actual = type.FromConsoleType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(CPUPPUTiming.RP2C02, "RP2C02 (NTSC NES)")]
        [InlineData(CPUPPUTiming.RP2C07, "RP2C07 (Licensed PAL NES)")]
        [InlineData(CPUPPUTiming.MultipleRegion, "Multiple-region")]
        [InlineData(CPUPPUTiming.UA6538, "UA6538 (Dendy)")]
        public void FromCPUPPUTimingTest(CPUPPUTiming timing, string expected)
        {
            string actual = timing.FromCPUPPUTiming();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(DefaultExpansionDevice.Unspecified, "Unspecified")]
        [InlineData(DefaultExpansionDevice.StandardControllers, "Standard NES/Famicom controllers")]
        [InlineData(DefaultExpansionDevice.NESFourScore, "NES Four Score/Satellite with two additional standard controllers")]
        [InlineData(DefaultExpansionDevice.FamicomFourPlayersAdapter, "Famicom Four Players Adapter with two additional standard controllers using the 'simple' protocol")]
        [InlineData(DefaultExpansionDevice.VsSystem4016, "Vs. System (1P via $4016)")]
        [InlineData(DefaultExpansionDevice.VsSystem4017, "Vs. System (1P via $4017)")]
        [InlineData(DefaultExpansionDevice.Reserved06, "Reserved (0x06)")]
        [InlineData(DefaultExpansionDevice.VsZapper, "Vs. Zapper")]
        [InlineData(DefaultExpansionDevice.Zapper4017, "Zapper ($4017)")]
        [InlineData(DefaultExpansionDevice.TwoZappers, "Two Zappers")]
        [InlineData(DefaultExpansionDevice.BandaiHyperShotLightgun, "Bandai Hyper Shot Lightgun")]
        [InlineData(DefaultExpansionDevice.PowerPadSideA, "Power Pad Side A")]
        [InlineData(DefaultExpansionDevice.PowerPadSideB, "Power Pad Side B")]
        [InlineData(DefaultExpansionDevice.FamilyTrainerSideA, "Family Trainer Side A")]
        [InlineData(DefaultExpansionDevice.FamilyTrainerSideB, "Family Trainer Side B")]
        [InlineData(DefaultExpansionDevice.ArkanoidVausControllerNES, "Arkanoid Vaus Controller (NES)")]
        [InlineData(DefaultExpansionDevice.ArkanoidVausControllerFamicom, "Arkanoid Vaus Controller (Famicom)")]
        [InlineData(DefaultExpansionDevice.TwoVausControllersPlusFamicomDataRecorder, "Two Vaus Controllers plus Famicom Data Recorder")]
        [InlineData(DefaultExpansionDevice.KonamiHyperShotController, "Konami Hyper Shot Controller")]
        [InlineData(DefaultExpansionDevice.CoconutsPachinkoController, "Coconuts Pachinko Controller")]
        [InlineData(DefaultExpansionDevice.ExcitingBoxingPunchingBag, "Exciting Boxing Punching Bag (Blowup Doll)")]
        [InlineData(DefaultExpansionDevice.JissenMahjongController, "Jissen Mahjong Controller")]
        [InlineData(DefaultExpansionDevice.YonezawaPartyTap, "米澤 (Yonezawa) Party Tap")]
        [InlineData(DefaultExpansionDevice.OekaKidsTablet, "Oeka Kids Tablet")]
        [InlineData(DefaultExpansionDevice.SunsoftBarcodeBattler, "Sunsoft Barcode Battler")]
        [InlineData(DefaultExpansionDevice.MiraclePianoKeyboard, "Miracle Piano Keyboard")]
        [InlineData(DefaultExpansionDevice.PokkunMoguraaTapTapMat, "Pokkun Moguraa Tap-tap Mat (Whack-a-Mole Mat and Mallet)")]
        [InlineData(DefaultExpansionDevice.TopRider, "Top Rider (Inflatable Bicycle)")]
        [InlineData(DefaultExpansionDevice.DoubleFisted, "Double-Fisted (Requires or allows use of two controllers by one player)")]
        [InlineData(DefaultExpansionDevice.Famicom3DSystem, "Famicom 3D System")]
        [InlineData(DefaultExpansionDevice.DoremikkoKeyboard, "Doremikko Keyboard")]
        [InlineData(DefaultExpansionDevice.ROBGyromite, "R.O.B. Gyromite")]
        [InlineData(DefaultExpansionDevice.FamicomDataRecorder, "Famicom Data Recorder ('silent' keyboard)")]
        [InlineData(DefaultExpansionDevice.ASCIITurboFile, "ASCII Turbo File")]
        [InlineData(DefaultExpansionDevice.IGSStorageBattleBox, "IGS Storage Battle Box")]
        [InlineData(DefaultExpansionDevice.FamilyBASICKeyboardPlusFamicomDataRecorder, "Family BASIC Keyboard plus Famicom Data Recorder")]
        [InlineData(DefaultExpansionDevice.DongdaPECKeyboard, "东达 (Dōngdá) PEC Keyboard")]
        [InlineData(DefaultExpansionDevice.BitCorpBit79Keyboard, "普澤 (Pǔzé, a.k.a. Bit Corp.) Bit-79 Keyboard")]
        [InlineData(DefaultExpansionDevice.SuborKeyboard, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusMacroWinnersMouse, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Macro Winners Mouse")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusSuborMouse4016, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Subor Mouse via $4016")]
        [InlineData(DefaultExpansionDevice.SNESMouse4016, "SNES Mouse ($4016)")]
        [InlineData(DefaultExpansionDevice.Multicart, "Multicart")]
        [InlineData(DefaultExpansionDevice.TwoSNESControllers, "Two SNES controllers replacing the two standard NES controllers")]
        [InlineData(DefaultExpansionDevice.RacerMateBicycle, "RacerMate Bicycle")]
        [InlineData(DefaultExpansionDevice.UForce, "U-Force")]
        [InlineData(DefaultExpansionDevice.ROBStackUp, "R.O.B. Stack-Up")]
        [InlineData(DefaultExpansionDevice.CityPatrolmanLightgun, "City Patrolman Lightgun")]
        [InlineData(DefaultExpansionDevice.SharpC1CassetteInterface, "Sharp C1 Cassette Interface")]
        [InlineData(DefaultExpansionDevice.StandardControllerWithSwappedInputs, "Standard Controller with swapped Left-Right/Up-Down/B-A")]
        [InlineData(DefaultExpansionDevice.ExcaliburSudokuPad, "Excalibur Sudoku Pad")]
        [InlineData(DefaultExpansionDevice.ABLPinball, "ABL Pinball")]
        [InlineData(DefaultExpansionDevice.GoldenNuggetCasinoExtraButtons, "Golden Nugget Casino extra buttons")]
        [InlineData(DefaultExpansionDevice.KedaKeyboard, "科达 (Kēdá) Keyboard")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusSuborMouse4017, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Subor Mouse via $4017")]
        [InlineData(DefaultExpansionDevice.PortTestController, "Port test controller")]
        [InlineData(DefaultExpansionDevice.BandaiMultiGamePlayerGamepad, "Bandai Multi Game Player Gamepad buttons")]
        [InlineData(DefaultExpansionDevice.VenomTVDanceMat, "Venom TV Dance Mat")]
        [InlineData(DefaultExpansionDevice.LGTVRemoteControl, "LG TV Remote Control")]
        [InlineData(DefaultExpansionDevice.FamicomNetworkController, "Famicom Network Controller")]
        [InlineData(DefaultExpansionDevice.KingFishingController, "King Fishing Controller")]
        [InlineData(DefaultExpansionDevice.CroakyKaraokeController, "Croaky Karaoke Controller")]
        [InlineData(DefaultExpansionDevice.KingwonKeyboard, "科王 (Kēwáng, a.k.a. Kingwon) Keyboard")]
        [InlineData(DefaultExpansionDevice.ZechengKeyboard, "泽诚 (Zéchéng) Keyboard")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusL90RotatedPS2Mouse4017, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus L90-rotated PS/2 mouse in $4017")]
        [InlineData(DefaultExpansionDevice.PS2KeyboardInUM6578PS2PortPS2Mouse4017, "PS/2 Keyboard in UM6578 PS/2 port, PS/2 Mouse via $4017")]
        [InlineData(DefaultExpansionDevice.PS2MouseInUM6578PS2Port, "PS/2 Mouse in UM6578 PS/2 port")]
        [InlineData(DefaultExpansionDevice.YuxingMouse4016, "裕兴 (Yùxìng) Mouse via $4016")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusYuxingMouse4016, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus 裕兴 (Yùxìng) Mouse mouse in $4016")]
        [InlineData(DefaultExpansionDevice.GigggleTVPump, "Gigggle TV Pump")]
        [InlineData(DefaultExpansionDevice.BBKKeyboardPlusR90RotatedPS2Mouse4017, "步步高 (Bùbùgāo, a.k.a. BBK) Keyboard plus R90-rotated PS/2 mouse in $4017")]
        [InlineData(DefaultExpansionDevice.MagicalCooking, "Magical Cooking")]
        [InlineData(DefaultExpansionDevice.SNESMouse4017, "SNES Mouse ($4017)")]
        [InlineData(DefaultExpansionDevice.Zapper4016, "Zapper ($4016)")]
        [InlineData(DefaultExpansionDevice.ArkanoidVausControllerPrototype, "Arkanoid Vaus Controller (Prototype)")]
        [InlineData(DefaultExpansionDevice.TVMahjongGameController, "TV 麻雀 Game (TV Mahjong Game) Controller")]
        [InlineData(DefaultExpansionDevice.MahjongGekitouDensetsuController, "麻雀激闘伝説 (Mahjong Gekitou Densetsu) Controller")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusXInvertedPS2Mouse4017, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus X-inverted PS/2 mouse in $4017")]
        [InlineData(DefaultExpansionDevice.IBMPCXTKeyboard, "IBM PC/XT Keyboard")]
        [InlineData(DefaultExpansionDevice.SuborKeyboardPlusMegaBookMouse, "小霸王 (Xiǎobàwáng, a.k.a. Subor) Keyboard plus Mega Book Mouse")]
        public void FromDefaultExpansionDeviceTest(DefaultExpansionDevice device, string expected)
        {
            string actual = device.FromDefaultExpansionDevice();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ExtendedConsoleType.RegularSystem, "Regular NES/Famicom/Dendy")]
        [InlineData(ExtendedConsoleType.NintendoVsSystem, "Nintendo Vs. System")]
        [InlineData(ExtendedConsoleType.Playchoice10, "Playchoice 10")]
        [InlineData(ExtendedConsoleType.RegularFamicloneDecimalMode, "Regular Famiclone, but with CPU that supports Decimal Mode")]
        [InlineData(ExtendedConsoleType.RegularNESWithEPSM, "Regular NES/Famicom with EPSM module or plug-through cartridge")]
        [InlineData(ExtendedConsoleType.VRTechnologyVT01, "V.R. Technology VT01 with red/cyan STN palette")]
        [InlineData(ExtendedConsoleType.VRTechnologyVT02, "V.R. Technology VT02")]
        [InlineData(ExtendedConsoleType.VRTechnologyVT03, "V.R. Technology VT03")]
        [InlineData(ExtendedConsoleType.VRTechnologyVT09, "V.R. Technology VT09")]
        [InlineData(ExtendedConsoleType.VRTechnologyVT32, "V.R. Technology VT32")]
        [InlineData(ExtendedConsoleType.VRTechnologyVT369, "V.R. Technology VT369")]
        [InlineData(ExtendedConsoleType.UMCUM6578, "UMC UM6578")]
        [InlineData(ExtendedConsoleType.FamicomNetworkSystem, "Famicom Network System")]
        [InlineData(ExtendedConsoleType.ReservedD, "Reserved (0x0D)")]
        [InlineData(ExtendedConsoleType.ReservedE, "Reserved (0x0E)")]
        [InlineData(ExtendedConsoleType.ReservedF, "Reserved (0x0F)")]
        public void FromExtendedConsoleTypeTest(ExtendedConsoleType type, string expected)
        {
            string actual = type.FromExtendedConsoleType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(NametableArrangement.Vertical, "Vertical")]
        [InlineData(NametableArrangement.Horizontal, "Horizontal")]
        public void FromNametableArrangementTest(NametableArrangement arrangement, string expected)
        {
            string actual = arrangement.FromNametableArrangement();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(TVSystem.NTSC, "NTSC")]
        [InlineData(TVSystem.PAL, "PAL")]
        public void FromTVSystemTest(TVSystem system, string expected)
        {
            string actual = system.FromTVSystem();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(TVSystemExtended.NTSC, "NTSC")]
        [InlineData(TVSystemExtended.DualCompatible1, "Dual-compatible (0x01)")]
        [InlineData(TVSystemExtended.PAL, "PAL")]
        [InlineData(TVSystemExtended.DualCompatible3, "Dual-compatible (0x03)")]
        public void FromTVSystemExtendedTest(TVSystemExtended system, string expected)
        {
            string actual = system.FromTVSystemExtended();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(VsHardwareType.VsUnisystem, "Vs. Unisystem (normal)")]
        [InlineData(VsHardwareType.VsUnisystemRBIBaseballProtection, "Vs. Unisystem (RBI Baseball protection)")]
        [InlineData(VsHardwareType.VsUnisystemTKOBoxingProtection, "Vs. Unisystem (TKO Boxing protection)")]
        [InlineData(VsHardwareType.VsUnisystemSuperXeviousProtection, "Vs. Unisystem (Super Xevious protection)")]
        [InlineData(VsHardwareType.VsUnisystemVsIceClimberJapanProtection, "Vs. Unisystem (Vs. Ice Climber Japan protection)")]
        [InlineData(VsHardwareType.VsDualSystem, "Vs. Dual System (normal)")]
        [InlineData(VsHardwareType.VsDualSystemRaidOnBungelingBayProtection, "Vs. Dual System (Raid on Bungeling Bay protection)")]
        public void FromVsHardwareTypeTest(VsHardwareType type, string expected)
        {
            string actual = type.FromVsHardwareType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(VsSystemType.AnyRP2C03RC2C03Variant, "Any RP2C03/RC2C03 variant")]
        [InlineData(VsSystemType.Reserved1, "Reserved (0x01)")]
        [InlineData(VsSystemType.RP2C040001, "RP2C04-0001")]
        [InlineData(VsSystemType.RP2C040002, "RP2C04-0002")]
        [InlineData(VsSystemType.RP2C040003, "RP2C04-0003")]
        [InlineData(VsSystemType.RP2C040004, "RP2C04-0004")]
        [InlineData(VsSystemType.Reserved6, "Reserved (0x06)")]
        [InlineData(VsSystemType.Reserved7, "Reserved (0x07)")]
        [InlineData(VsSystemType.RC2C0501, "RC2C05-01 (signature unknown)")]
        [InlineData(VsSystemType.RC2C0502, "RC2C05-02 ($2002 AND $3F =$3D)")]
        [InlineData(VsSystemType.RC2C0503, "RC2C05-03 ($2002 AND $1F =$1C)")]
        [InlineData(VsSystemType.RC2C0504, "RC2C05-04 ($2002 AND $1F =$1B)")]
        [InlineData(VsSystemType.ReservedC, "Reserved (0x0C)")]
        [InlineData(VsSystemType.ReservedD, "Reserved (0x0D)")]
        [InlineData(VsSystemType.ReservedE, "Reserved (0x0E)")]
        [InlineData(VsSystemType.ReservedF, "Reserved (0x0F)")]
        public void FromVsSystemTypeTest(VsSystemType type, string expected)
        {
            string actual = type.FromVsSystemType();
            Assert.Equal(expected, actual);
        }
    }
}
