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

            //builder.AppendLine(Model.Trainer, "Trainer Data");
            builder.AppendLine(Model.Trainer.Length, "Trainer Data Length");
            //builder.AppendLine(Model.PRGROMData, "PRG-ROM Data");
            builder.AppendLine(Model.PRGROMData.Length, "PRG-ROM Data Length");
            //builder.AppendLine(Model.CHRROMData, "CHR-ROM Data");
            builder.AppendLine(Model.CHRROMData.Length, "CHR-ROM Data Length");
            //builder.AppendLine(Model.PlayChoiceINSTROM, "PlayChoice INST-ROM Data");
            builder.AppendLine(Model.PlayChoiceINSTROM.Length, "PlayChoice INST-ROM Data Length");
            //builder.AppendLine(Model.PlayChoicePROM, "PlayChoice PROM Data");
            builder.AppendLine(Model.PlayChoicePROM.Length, "PlayChoice PROM Data Length");
            builder.AppendLine(Model.Title, "Title");
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
            string nametableArrangement = header.NametableArrangement.FromNametableArrangement();
            builder.AppendLine(nametableArrangement, "    Nametable Arrangement");

            // Bit 1
            string batteryBackedPrgRam = header.BatteryBackedPRGRAM ? "Present" : "Not present";
            builder.AppendLine(batteryBackedPrgRam, "    Battery-Backed PRG RAM");

            // Bit 2
            string trainerPresent = header.TrainerPresent ? "Present" : "Not present";
            builder.AppendLine(trainerPresent, "    Trainer");

            // Bit 3
            builder.AppendLine(header.AlternativeNametableLayout, "    Alternative Nametable Layout");

            #endregion

            #region Flag 7

            builder.AppendLine("  Flag 7:");

            // Bits 0-1
            string consoleType = header.ConsoleType.FromConsoleType();
            builder.AppendLine(consoleType, "    System Type");

            // Bits 2-3
            builder.AppendLine(header.NES20, "    NES 2.0");

            #endregion

            byte mapperNumber = (byte)((header.MapperUpperNibble << 4) | header.MapperLowerNibble);
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
                    | (byte)((header.MapperUpperNibble << 4)
                    | header.MapperLowerNibble));
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
                int chrNvramSize = chrNvramShiftCount > 0 ? 64 << chrNvramShiftCount : 0;

                builder.AppendLine(chrRamShiftCount, "  CHR-RAM shift count");
                builder.AppendLine(chrRamSize, "  CHR-RAM size");
                builder.AppendLine(chrNvramShiftCount, "  CHR-NVRAM shift count");
                builder.AppendLine(chrNvramSize, "  CHR-NVRAM size");

                // Byte 12
                string cpuTiming = header2.CPUPPUTiming.FromCPUPPUTiming();
                builder.AppendLine(cpuTiming, "  CPU timing");

                // Byte 13
                if (header.ConsoleType == ConsoleType.ExtendedConsoleType)
                {
                    ExtendedConsoleType extendedConsoleType = (ExtendedConsoleType)(header2.ExtendedSystemType & 0x0F);
                    string extendedConsoleTypeString = extendedConsoleType.FromExtendedConsoleType();
                    builder.AppendLine(extendedConsoleTypeString, "  Extended console type");
                }
                else if (header.ConsoleType == ConsoleType.VSUnisystem)
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
