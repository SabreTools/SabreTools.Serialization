using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.NES;

namespace SabreTools.Serialization.Wrappers
{
    public partial class NESCart : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("Nintendo Entertainment System Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);

            builder.AppendLine(Model.Trainer, "Trainer Data");
            builder.AppendLine(Model.PRGROMData, "PRG-ROM Data");
            builder.AppendLine(Model.CHRROMData, "CHR-ROM Data");
            builder.AppendLine(Model.PlayChoiceINSTROM, "PlayChoice INST-ROM Data");
            builder.AppendLine(Model.PlayChoicePROM, "PlayChoice PROM Data");
            builder.AppendLine(Model.Title, "Title:");
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No header present");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.IdentificationString, "  Identification string");
            builder.AppendLine(header.PRGROMSize, "  PRG-ROM size in 16KiB units");
            builder.AppendLine(header.CHRROMSize, "  CHR-ROM size in 8KiB units");

            #region Flag 6

            builder.AppendLine("  Flag 6:");

            // Bit 0
#if NET20 || NET35
            if ((header.Flag6 & Flag6.NametableArrangementHorizontal) != 0)
#else
            if (header.Flag6.HasFlag(Flag6.NametableArrangementHorizontal))
#endif
                builder.AppendLine("    Nametable Arrangement: Horizontal");
            else
                builder.AppendLine("    Nametable Arrangement: Vertical");

            // Bit 1
#if NET20 || NET35
            if ((header.Flag6 & Flag6.BatteryBackedPRGRAMPresent) != 0)
#else
            if (header.Flag6.HasFlag(Flag6.BatteryBackedPRGRAMPresent))
#endif
                builder.AppendLine("    Battery-Backed PRG RAM: Present");
            else
                builder.AppendLine("    Battery-Backed PRG RAM: Not present");

            // Bit 2
#if NET20 || NET35
            if ((header.Flag6 & Flag6.TrainerPresent) != 0)
#else
            if (header.Flag6.HasFlag(Flag6.TrainerPresent))
#endif
                builder.AppendLine("    Trainer: Present");
            else
                builder.AppendLine("    Trainer: Not present");

            // Bit 3
#if NET20 || NET35
            if ((header.Flag6 & Flag6.AlternativeNametableLayout) != 0)
#else
            if (header.Flag6.HasFlag(Flag6.AlternativeNametableLayout))
#endif
                builder.AppendLine("    Alternative Nametable Layout: True");
            else
                builder.AppendLine("    Alternative Nametable Layout: False");

            #endregion

            #region Flag 7

            builder.AppendLine("  Flag 7:");

            // Bits 0-1
#if NET20 || NET35
            if ((header.Flag7 & Flag7.ExtendedConsoleType) != 0)
#else
            if (header.Flag7.HasFlag(Flag7.ExtendedConsoleType))
#endif
                builder.AppendLine("    System Type: Extended Console Type");
#if NET20 || NET35
            else if ((header.Flag7 & Flag7.PlayChoice10) != 0)
#else
            else if (header.Flag7.HasFlag(Flag7.PlayChoice10))
#endif
                builder.AppendLine("    System Type: PlayChoice-10");
#if NET20 || NET35
            else if ((header.Flag7 & Flag7.VSUnisystem) != 0)
#else
            else if (header.Flag7.HasFlag(Flag7.VSUnisystem))
#endif
                builder.AppendLine("    System Type: Vs. Unisystem");
            else
                builder.AppendLine("    System Type: Nintendo Entertainment System/Family Computer");

            // Bits 2-3
#if NET20 || NET35
            if ((header.Flag7 & Flag7.NES20) != 0)
#else
            if (header.Flag7.HasFlag(Flag7.NES20))
#endif
                builder.AppendLine("    NES 2.0: True");
            else
                builder.AppendLine("    NES 2.0: False");

            #endregion

            byte mapperNumber = (byte)((((byte)header.Flag7 >> 4) << 4) | (byte)((byte)header.Flag6 >> 4));
            builder.AppendLine(mapperNumber, "  Mapper number");

            if (header is Header1 header1)
            {
                // Byte 8
                builder.AppendLine(header1.PRGRAMSize, prefixString: "  PRG-RAM size in 8KiB units");

                // Byte 9
                builder.AppendLine($"  TV system: {header1.TVSystem} (0x{header1.TVSystem:X})");

                // Byte 10
                #region Flag 10

                builder.AppendLine("  Flag 10:");

                // Bits 0-1
#if NET20 || NET35
                if ((header1.Flag10 & Flag10.DualCompatible2) != 0)
#else
                if (header1.Flag10.HasFlag(Flag10.DualCompatible2))
#endif
                    builder.AppendLine("    TV System: Dual-compatible");
#if NET20 || NET35
                else if ((header1.Flag10 & Flag10.PAL) != 0)
#else
                else if (header1.Flag10.HasFlag(Flag10.PAL))
#endif
                    builder.AppendLine("    TV System: PAL");
#if NET20 || NET35
                else if ((header1.Flag10 & Flag10.DualCompatible1) != 0)
#else
                else if (header1.Flag10.HasFlag(Flag10.DualCompatible1))
#endif
                    builder.AppendLine("    TV System: Dual-compatible");
                else
                    builder.AppendLine("    TV System: NTSC");

                // Bit 4
#if NET20 || NET35
                if ((header1.Flag10 & Flag10.PRGRAMNotPresent) != 0)
#else
                if (header1.Flag10.HasFlag(Flag10.PRGRAMNotPresent))
#endif
                    builder.AppendLine("    PRG-RAM: Not present");
                else
                    builder.AppendLine("    PRG-RAM: Present");

                // Bit 5
#if NET20 || NET35
                if ((header1.Flag10 & Flag10.BoardHasBusConflicts) != 0)
#else
                if (header1.Flag10.HasFlag(Flag10.BoardHasBusConflicts))
#endif
                    builder.AppendLine("    Has Bus Conflicts: True");
                else
                    builder.AppendLine("    Has Bus Conflicts: False");

                #endregion

                // Bytes 11-15
                builder.AppendLine(header1.Padding, "  Padding");
            }
            else if (header is Header2 header2)
            {
                // Byte 8
                byte mapperMsb = (byte)(header2.MapperMSBSubmapper & 0x0F);
                ushort extendedMapperNumber = (ushort)((mapperMsb << 8)
                    | (byte)((((byte)header.Flag7 >> 4) << 4)
                    | (byte)((byte)header.Flag6 >> 4)));
                byte submapperNumber = (byte)(header2.MapperMSBSubmapper >> 4);

                builder.AppendLine(mapperMsb, "  Mapper MSB");
                builder.AppendLine(extendedMapperNumber, "  Extended mapper number");
                builder.AppendLine(submapperNumber, "  Submapper number");

                // Byte 9
                byte prgRomMsb = (byte)(header2.PRGCHRMSB & 0x0F);
                ushort extendedPrgRomSize = (ushort)((prgRomMsb << 8) | header.PRGROMSize);
                byte chrRomMsb = (byte)(header2.PRGCHRMSB >> 4);
                ushort extendedChrRomSize = (ushort)((chrRomMsb << 8) | header.CHRROMSize);

                builder.AppendLine(prgRomMsb, "  PRG-ROM size MSB");
                builder.AppendLine(extendedPrgRomSize, "  Extended PRG-ROM size");
                builder.AppendLine(chrRomMsb, "  CHR-ROM size MSB");
                builder.AppendLine(extendedChrRomSize, "  Extended CHR-ROM size");

                // Byte 10
                byte prgRamShiftCount = (byte)(header2.PRGRAMEEPROMSize & 0x0F);
                int prgRamSize = prgRamShiftCount > 0 ? 64 << prgRamShiftCount : 0;
                byte eepromShiftCount = (byte)(header2.PRGRAMEEPROMSize >> 4);
                int eepromSize = eepromShiftCount > 0 ? 64 << eepromShiftCount : 0;

                builder.AppendLine(prgRamShiftCount, "  PRG-RAM shift count");
                builder.AppendLine(prgRamSize, "  PRG-RAM size");
                builder.AppendLine(eepromShiftCount, "  PRG-NVRAM/EEPROM shift count");
                builder.AppendLine(eepromSize, "  PRG-NVRAM/EEPROM size");

                // Byte 11
                byte chrRamShiftCount = (byte)(header2.CHRRAMSize & 0x0F);
                int chrRamSize = chrRamShiftCount > 0 ? 64 << chrRamShiftCount : 0;
                byte chrNvramShiftCount = (byte)(header2.CHRRAMSize >> 4);
                int chrNvramSize = eepromShiftCount > 0 ? 64 << chrNvramShiftCount : 0;

                builder.AppendLine(chrRamShiftCount, "  CHR-RAM shift count");
                builder.AppendLine(chrRamSize, "  CHR-RAM size");
                builder.AppendLine(chrNvramShiftCount, "  CHR-NVRAM shift count");
                builder.AppendLine(chrNvramSize, "  CHR-NVRAM size");

                // Byte 12
                string cpuTiming = header2.CPUPPUTiming.FromCPUPPUTiming();
                builder.AppendLine(cpuTiming, "  CPU timing");

                // Byte 13
#if NET20 || NET35
                if ((header.Flag7 & Flag7.ExtendedConsoleType) != 0)
#else
                if (header.Flag7.HasFlag(Flag7.ExtendedConsoleType))
#endif
                {
                    ExtendedConsoleType extendedConsoleType = (ExtendedConsoleType)(header2.ExtendedSystemType & 0x0F);
                    string extendedConsoleTypeString = extendedConsoleType.FromExtendedConsoleType();
                    builder.AppendLine(extendedConsoleTypeString, "  Extended console type");
                }
#if NET20 || NET35
                else if ((header.Flag7 & Flag7.VSUnisystem) != 0)
#else
                else if (header.Flag7.HasFlag(Flag7.VSUnisystem))
#endif
                {
                    VsSystemType vsSystemType = (VsSystemType)(header2.ExtendedSystemType & 0x0F);
                    string vsSystemTypeString = vsSystemType.FromVsSystemType();
                    builder.AppendLine(vsSystemTypeString, "  Vs. system type");

                    VsHardwareType vsHardwareType = (VsHardwareType)(header2.ExtendedSystemType >> 4);
                    string vsHardwareTypeString = vsHardwareType.FromVsHardwareType();
                    builder.AppendLine(vsHardwareTypeString, "  Vs. hardware type");
                }

                // Byte 14
                builder.AppendLine(header2.MiscellaneousROMs, "  Number of miscellaneous ROMs");

                // Byte 15
                string defaultExpansionDevice = header2.DefaultExpansionDevice.FromDefaultExpansionDevice();
                builder.AppendLine(defaultExpansionDevice, "  Default expansion device");
            }
            else
            {
                builder.AppendLine("  Unrecognized header type!");
            }

            builder.AppendLine();
        }
    }
}
