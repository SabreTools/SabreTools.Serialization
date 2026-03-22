using SabreTools.Data.Models.Atari7800;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class Atari7800CartExtensionsTests
    {
        [Theory]
        [InlineData((AudioDevice)0, "POKEY - none, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        // POKEY
        [InlineData(AudioDevice.Pokey440, "POKEY - @440, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        [InlineData(AudioDevice.Pokey450, "POKEY - @450, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        [InlineData(AudioDevice.Pokey450Plus440, "POKEY - @450+@440, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        [InlineData(AudioDevice.Pokey800, "POKEY - @800, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        [InlineData(AudioDevice.Pokey4000, "POKEY - @4000, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        [InlineData((AudioDevice)0x0007, "Unknown 7, No YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        // YM2151
        [InlineData(AudioDevice.YM2151460, "POKEY - none, YM2151 @460, No COVOX @430, No ADPCM Audio Stream @420")]
        // COVOX
        [InlineData(AudioDevice.COVOX430, "POKEY - none, No YM2151 @460, COVOX @430, No ADPCM Audio Stream @420")]
        // ADPCM
        [InlineData(AudioDevice.ADPCMAudioStream420, "POKEY - none, No YM2151 @460, No COVOX @430, ADPCM Audio Stream @420")]
        public void FromAudioDeviceTest(AudioDevice audio, string expected)
        {
            string actual = audio.FromAudioDevice();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((CartType)0, "N/A")]
        [InlineData(CartType.PokeyAt4000, "pokey at $4000")]
        [InlineData(CartType.SupergameBankSwitched, "supergame bank switched")]
        [InlineData(CartType.SupergameRamAt4000, "supergame ram at $4000")]
        [InlineData(CartType.RomAt4000, "rom at $4000")]
        [InlineData(CartType.Bank6At4000, "bank 6 at $4000")]
        [InlineData(CartType.BankedRam, "banked ram")]
        [InlineData(CartType.PokeyAt450, "pokey at $450")]
        [InlineData(CartType.MirrorRamAt4000, "mirror ram at $4000")]
        [InlineData(CartType.ActivisionBanking, "activision banking")]
        [InlineData(CartType.AbsoluteBanking, "absolute banking")]
        [InlineData(CartType.PokeyAt440, "pokey at $440")]
        [InlineData(CartType.Ym2151At460461, "ym2151 at $460/$461")]
        [InlineData(CartType.Souper, "souper")]
        [InlineData(CartType.Banksets, "banksets")]
        [InlineData(CartType.HaltBankedRam, "halt banked ram")]
        [InlineData(CartType.PokeyAt800, "pokey@800")]
        public void FromCartTypeTest(CartType type, string expected)
        {
            string actual = type.FromCartType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ControllerType.None, "none")]
        [InlineData(ControllerType.Joystick, "7800 joystick")]
        [InlineData(ControllerType.Lightgun, "lightgun")]
        [InlineData(ControllerType.Paddle, "paddle")]
        [InlineData(ControllerType.Trakball, "trakball")]
        [InlineData(ControllerType.VcsJoystick, "2600 joystick")]
        [InlineData(ControllerType.VcsDriving, "2600 driving")]
        [InlineData(ControllerType.VcsKeypad, "2600 keypad")]
        [InlineData(ControllerType.STMouse, "ST mouse")]
        [InlineData(ControllerType.AmigaMouse, "Amiga mouse")]
        [InlineData(ControllerType.AtariVoxSaveKey, "AtariVox/SaveKey")]
        [InlineData(ControllerType.SNES2Atari, "SNES2Atari")]
        [InlineData(ControllerType.Mega7800, "Mega7800")]
        public void FromControllerTypeTest(ControllerType type, string expected)
        {
            string actual = type.FromControllerType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(Interrupt.None, "None")]
        [InlineData(Interrupt.Pokey1, "POKEY 1 (@450 | @800 | @4000)")]
        [InlineData(Interrupt.Pokey2, "POKEY 2 (@440)")]
        [InlineData(Interrupt.YM2151, "YM2151")]
        public void FromInterruptTest(Interrupt interrupt, string expected)
        {
            string actual = interrupt.FromInterrupt();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(Mapper.Linear, "Linear")]
        [InlineData(Mapper.SuperGame, "SuperGame")]
        [InlineData(Mapper.Activision, "Activision")]
        [InlineData(Mapper.Absolute, "Absolute")]
        [InlineData(Mapper.Souper, "Souper")]
        public void FromMapperTest(Mapper mapper, string expected)
        {
            string actual = mapper.FromMapper();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MapperOptions.SuperGameNone, "Option at @4000 - none, Standard ROM")]
        [InlineData(MapperOptions.SuperGame16KRAM, "Option at @4000 - 16K RAM, Standard ROM")]
        [InlineData(MapperOptions.SuperGame8KEXRAMA8, "Option at @4000 - 8K EXRAM/A8, Standard ROM")]
        [InlineData(MapperOptions.SuperGame32KEXRAMM2, "Option at @4000 - 32K EXRAM/M2, Standard ROM")]
        [InlineData(MapperOptions.SuperGameEXROM, "Option at @4000 - EXROM, Standard ROM")]
        [InlineData(MapperOptions.SuperGameEXFIX, "Option at @4000 - EXFIX, Standard ROM")]
        [InlineData(MapperOptions.SuperGame32KEXRAMX2, "Option at @4000 - 32k EXRAM/X2, Standard ROM")]
        [InlineData(MapperOptions.BanksetRom, "Option at @4000 - none, Bankset ROM")]
        public void FromMapperOptionsTest(MapperOptions options, string expected)
        {
            string actual = options.FromMapperOptions();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(SaveDevice.None, "None")]
        [InlineData(SaveDevice.HSC, "HSC")]
        [InlineData(SaveDevice.SaveKeyAtariVox, "SaveKey/AtariVox")]
        public void FromSaveDeviceTest(SaveDevice device, string expected)
        {
            string actual = device.FromSaveDevice();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(SlotPassthroughDevice.None, "None")]
        [InlineData(SlotPassthroughDevice.XM, "XM")]
        public void FromSlotPassthroughDeviceTest(SlotPassthroughDevice device, string expected)
        {
            string actual = device.FromSlotPassthroughDevice();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((TVType)0, "NTSC, Component, Single-region")]
        [InlineData(TVType.PAL, "PAL, Component, Single-region")]
        [InlineData(TVType.Composite, "NTSC, Composite, Single-region")]
        [InlineData(TVType.MultiRegion, "NTSC, Component, Multi-region")]
        public void FromTVTypeTest(TVType type, string expected)
        {
            string actual = type.FromTVType();
            Assert.Equal(expected, actual);
        }
    }
}
