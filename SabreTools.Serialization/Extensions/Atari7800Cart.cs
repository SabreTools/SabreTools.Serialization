using System.Collections.Generic;
using SabreTools.Data.Models.Atari7800;

namespace SabreTools.Data.Extensions
{
    public static class Atari7800Cart
    {
        /// <summary>
        /// Convert a <see cref="AudioDevice"/> value to string
        /// </summary>
        public static string FromAudioDevice(this AudioDevice audio)
        {
            string[] devices = new string[4];

            byte pokey = (byte)((byte)audio & 0x0F);
            devices[0] = pokey switch
            {
                0 => "POKEY - none",
                1 => "POKEY - @440",
                2 => "POKEY - @450",
                3 => "POKEY - @450+@440",
                4 => "POKEY - @800",
                5 => "POKEY - @4000",
                _ => $"Unknown {pokey}",
            };

            byte ym2151 = (byte)(((byte)audio >> 1) & 0x01);
            devices[1] = ym2151 switch
            {
                0 => "No YM2151 @460",
                1 => "YM2151 @460",
                _ => $"Unknown {ym2151}",
            };

            byte covox = (byte)(((byte)audio >> 2) & 0x01);
            devices[2] = covox switch
            {
                0 => "No COVOX @430",
                1 => "COVOX @430",
                _ => $"Unknown {covox}",
            };

            byte adpcm = (byte)(((byte)audio >> 3) & 0x01);
            devices[3] = adpcm switch
            {
                0 => "No ADPCM Audio Stream @420",
                1 => "ADPCM Audio Stream @420",
                _ => $"Unknown {adpcm}",
            };

            return string.Join(", ", [.. devices]);
        }

        /// <summary>
        /// Convert a <see cref="CartType"/> value to string
        /// </summary>
        public static string FromCartType(this CartType type)
        {
            List<string> types = [];

#if NET20 || NET35
            if ((type & CartType.PokeyAt4000) != 0)
#else
            if (type.HasFlag(CartType.PokeyAt4000))
#endif
                types.Add("pokey at $4000");

#if NET20 || NET35
            if ((type & CartType.SupergameBankSwitched) != 0)
#else
            if (type.HasFlag(CartType.SupergameBankSwitched))
#endif
                types.Add("supergame bank switched");

#if NET20 || NET35
            if ((type & CartType.SupergameRamAt4000) != 0)
#else
            if (type.HasFlag(CartType.SupergameRamAt4000))
#endif
                types.Add("supergame ram at $4000");

#if NET20 || NET35
            if ((type & CartType.RomAt4000) != 0)
#else
            if (type.HasFlag(CartType.RomAt4000))
#endif
                types.Add("rom at $4000");

#if NET20 || NET35
            if ((type & CartType.Bank6At4000) != 0)
#else
            if (type.HasFlag(CartType.Bank6At4000))
#endif
                types.Add("bank 6 at $4000");

#if NET20 || NET35
            if ((type & CartType.BankedRam) != 0)
#else
            if (type.HasFlag(CartType.BankedRam))
#endif
                types.Add("banked ram");

#if NET20 || NET35
            if ((type & CartType.PokeyAt450) != 0)
#else
            if (type.HasFlag(CartType.PokeyAt450))
#endif
                types.Add("pokey at $450");

#if NET20 || NET35
            if ((type & CartType.MirrorRamAt4000) != 0)
#else
            if (type.HasFlag(CartType.MirrorRamAt4000))
#endif
                types.Add("mirror ram at $4000");

#if NET20 || NET35
            if ((type & CartType.ActivisionBanking) != 0)
#else
            if (type.HasFlag(CartType.ActivisionBanking))
#endif
                types.Add("activision banking");

#if NET20 || NET35
            if ((type & CartType.AbsoluteBanking) != 0)
#else
            if (type.HasFlag(CartType.AbsoluteBanking))
#endif
                types.Add("absolute banking");

#if NET20 || NET35
            if ((type & CartType.PokeyAt440) != 0)
#else
            if (type.HasFlag(CartType.PokeyAt440))
#endif
                types.Add("pokey at $440");

#if NET20 || NET35
            if ((type & CartType.Ym2151At460461) != 0)
#else
            if (type.HasFlag(CartType.Ym2151At460461))
#endif
                types.Add("ym2151 at $460/$461");

#if NET20 || NET35
            if ((type & CartType.Souper) != 0)
#else
            if (type.HasFlag(CartType.Souper))
#endif
                types.Add("souper");

#if NET20 || NET35
            if ((type & CartType.Banksets) != 0)
#else
            if (type.HasFlag(CartType.Banksets))
#endif
                types.Add("banksets");

#if NET20 || NET35
            if ((type & CartType.HaltBankedRam) != 0)
#else
            if (type.HasFlag(CartType.HaltBankedRam))
#endif
                types.Add("halt banked ram");

#if NET20 || NET35
            if ((type & CartType.PokeyAt800) != 0)
#else
            if (type.HasFlag(CartType.PokeyAt800))
#endif
                types.Add("pokey@800");

            // If no flags are set
            if (types.Count == 0)
                types.Add("N/A");

            return string.Join(", ", [.. types]);
        }

        /// <summary>
        /// Convert a <see cref="ControllerType"/> value to string
        /// </summary>
        public static string FromControllerType(this ControllerType type)
        {
            return type switch
            {
                ControllerType.None => "none",
                ControllerType.Joystick => "7800 joystick",
                ControllerType.Lightgun => "lightgun",
                ControllerType.Paddle => "paddle",
                ControllerType.Trakball => "trakball",
                ControllerType.VcsJoystick => "2600 joystick",
                ControllerType.VcsDriving => "2600 driving",
                ControllerType.VcsKeypad => "2600 keypad",
                ControllerType.STMouse => "ST mouse",
                ControllerType.AmigaMouse => "Amiga mouse",
                ControllerType.AtariVoxSaveKey => "AtariVox/SaveKey",
                ControllerType.SNES2Atari => "SNES2Atari",
                ControllerType.Mega7800 => "Mega7800",
                _ => $"Unknown {(byte)type}",
            };
        }

        /// <summary>
        /// Convert a <see cref="Interrupt"/> value to string
        /// </summary>
        public static string FromInterrupt(this Interrupt interrupt)
        {
            return interrupt switch
            {
                Interrupt.None => "None",
                Interrupt.Pokey1 => "POKEY 1 (@450 | @800 | @4000)",
                Interrupt.Pokey2 => "POKEY 2 (@440)",
                Interrupt.YM2151 => "YM2151",
                _ => $"Unknown {(byte)interrupt}",
            };
        }

        /// <summary>
        /// Convert a <see cref="Mapper"/> value to string
        /// </summary>
        public static string FromMapper(this Mapper mapper)
        {
            return mapper switch
            {
                Mapper.Linear => "Linear",
                Mapper.SuperGame => "SuperGame",
                Mapper.Activision => "Activision",
                Mapper.Absolute => "Absolute",
                Mapper.Souper => "Souper",
                _ => $"Unknown {(byte)mapper}",
            };
        }

        /// <summary>
        /// Convert a <see cref="MapperOptions"/> value to string
        /// </summary>
        public static string FromMapperOptions(this MapperOptions options)
        {
            string[] romOptions = new string[2];

            byte option4000 = (byte)((byte)options & 0x0F);
            romOptions[0] = option4000 switch
            {
                0 => "Option at @4000 - none",
                1 => "Option at @4000 - 16K RAM",
                2 => "Option at @4000 - 8K EXRAM/A8",
                3 => "Option at @4000 - 32K EXRAM/M2",
                4 => "Option at @4000 - EXROM",
                5 => "Option at @4000 - EXFIX",
                6 => "Option at @4000 - 32k EXRAM/X2",
                _ => $"Unknown {option4000}",
            };

            byte romType = (byte)(((byte)options >> 7) & 0x01);
            romOptions[1] = romType switch
            {
                0 => "Standard ROM",
                1 => "Bankset ROM",
                _ => $"Unknown {romType}",
            };

            return string.Join(", ", [.. romOptions]);
        }

        /// <summary>
        /// Convert a <see cref="SaveDevice"/> value to string
        /// </summary>
        public static string FromSaveDevice(this SaveDevice device)
        {
            return device switch
            {
                SaveDevice.None => "None",
                SaveDevice.HSC => "HSC",
                SaveDevice.SaveKeyAtariVox => "SaveKey/AtariVox",
                _ => $"Unknown {(byte)device}",
            };
        }

        /// <summary>
        /// Convert a <see cref="SlotPassthroughDevice"/> value to string
        /// </summary>
        public static string FromSlotPassthroughDevice(this SlotPassthroughDevice device)
        {
            return device switch
            {
                SlotPassthroughDevice.None => "None",
                SlotPassthroughDevice.XM => "XM",
                _ => $"Unknown {(byte)device}",
            };
        }

        /// <summary>
        /// Convert a <see cref="TVType"/> value to string
        /// </summary>
        public static string FromTVType(this TVType type)
        {
            string[] types = new string[3];

            byte standard = (byte)((byte)type & 0x01);
            types[0] = standard switch
            {
                0 => "NTSC",
                1 => "PAL",
                _ => $"Unknown {standard}",
            };

            byte connection = (byte)(((byte)type >> 1) & 0x01);
            types[1] = connection switch
            {
                0 => "Component",
                1 => "Composite",
                _ => $"Unknown {standard}",
            };

            byte region = (byte)(((byte)type >> 2) & 0x01);
            types[2] = region switch
            {
                0 => "Single-region",
                1 => "Multi-region",
                _ => $"Unknown {standard}",
            };

            return string.Join(", ", [.. types]);
        }
    }
}
