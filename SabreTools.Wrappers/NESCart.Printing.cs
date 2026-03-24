using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.NES;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
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
            builder.AppendLine(Model.PrgRomData.Length, "PRG-ROM Data Length");
            //builder.AppendLine(Model.CHRROMData, "CHR-ROM Data");
            builder.AppendLine(Model.ChrRomData.Length, "CHR-ROM Data Length");
            //builder.AppendLine(Model.PlayChoiceINSTROM, "PlayChoice INST-ROM Data");
            builder.AppendLine(Model.PlayChoiceInstRom.Length, "PlayChoice INST-ROM Data Length");
            //builder.AppendLine(Model.PlayChoicePROM, "PlayChoice PROM Data");
            builder.AppendLine(Model.PlayChoiceProm.Length, "PlayChoice PROM Data Length");
            builder.AppendLine(Model.Title, "Title");
        }

        private static void Print(StringBuilder builder, CartHeader? header)
        {
            builder.AppendLine("  Common Header Information:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No header present");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.IdentificationString, "  Identification string");
            builder.AppendLine(header.PrgRomSize, "  PRG-ROM size in 16KiB units");
            builder.AppendLine(header.ChrRomSize, "  CHR-ROM size in 8KiB units");

            #region Flag 6

            builder.AppendLine("  Flag 6:");

            // Bit 0
            string nametableArrangement = header.NametableArrangement.FromNametableArrangement();
            builder.AppendLine(nametableArrangement, "    Nametable Arrangement");

            // Bit 1
            string batteryBackedPrgRam = header.BatteryBackedPrgRam ? "Present" : "Not present";
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
            builder.AppendLine(mapperNumber, "  Mapper");
            builder.AppendLine();

            if (header is CartHeader1 header1)
            {
                builder.AppendLine("  NES 1.0 Header Information:");
                builder.AppendLine("  -------------------------");

                // Byte 8
                builder.AppendLine(header1.PrgRamSize, prefixString: "  PRG-RAM size in 8KiB units");

                // Byte 9
                string tvSystem = header1.TVSystem.FromTVSystem();
                builder.AppendLine(tvSystem, "  TV system");

                // Byte 10
                #region Flag 10

                builder.AppendLine("  Flag 10:");

                // Bits 0-1
                string tvSystemExtended = header1.TVSystemExtended.FromTVSystemExtended();
                builder.AppendLine(tvSystemExtended, "    TV System Extended");

                // Bits 2-3
                builder.AppendLine(header1.Byte10ReservedBits23, "    Reserved bits 1-2");

                // Bit 4
                string prgRamPresent = header1.PrgRamPresent ? "Present" : "Not present";
                builder.AppendLine(prgRamPresent, "    PRG-RAM");

                // Bit 5
                builder.AppendLine(header1.HasBusConflicts, "    Has Bus Conflicts");

                // Bits 6-7
                builder.AppendLine(header1.Byte10ReservedBits67, "    Reserved bits 6-7");

                #endregion

                // Bytes 11-15
                builder.AppendLine(header1.Padding, "  Padding");
            }
            else if (header is CartHeader2 header2)
            {
                builder.AppendLine("  NES 2.0 Header Information:");
                builder.AppendLine("  -------------------------");

                // Byte 8
                ushort extendedMapperNumber = (ushort)((header2.MapperMSB << 8)
                    | (byte)((header.MapperUpperNibble << 4)
                    | header.MapperLowerNibble));

                builder.AppendLine(header2.MapperMSB, "  Mapper MSB");
                builder.AppendLine(extendedMapperNumber, "  Extended mapper number");
                builder.AppendLine(header2.Submapper, "  Submapper");

                // Byte 9
                ushort extendedPrgRomSize = (ushort)((header2.PrgRomSizeMSB << 8) | header.PrgRomSize);
                ushort extendedChrRomSize = (ushort)((header2.ChrRomSizeMSB << 8) | header.ChrRomSize);

                builder.AppendLine(header2.PrgRomSizeMSB, "  PRG-ROM size MSB");
                builder.AppendLine(extendedPrgRomSize, "  Extended PRG-ROM size");
                builder.AppendLine(header2.ChrRomSizeMSB, "  CHR-ROM size MSB");
                builder.AppendLine(extendedChrRomSize, "  Extended CHR-ROM size");

                // Byte 10
                int prgRamSize = header2.PrgRamShiftCount > 0 ? 64 << header2.PrgRamShiftCount : 0;
                int eepromSize = header2.PrgNvramEepromShiftCount > 0 ? 64 << header2.PrgNvramEepromShiftCount : 0;

                builder.AppendLine(header2.PrgRamShiftCount, "  PRG-RAM shift count");
                builder.AppendLine(prgRamSize, "  PRG-RAM size");
                builder.AppendLine(header2.PrgNvramEepromShiftCount, "  PRG-NVRAM/EEPROM shift count");
                builder.AppendLine(eepromSize, "  PRG-NVRAM/EEPROM size");

                // Byte 11
                int chrRamSize = header2.ChrRamShiftCount > 0 ? 64 << header2.ChrRamShiftCount : 0;
                int chrNvramSize = header2.ChrNvramShiftCount > 0 ? 64 << header2.ChrNvramShiftCount : 0;

                builder.AppendLine(header2.ChrRamShiftCount, "  CHR-RAM shift count");
                builder.AppendLine(chrRamSize, "  CHR-RAM size");
                builder.AppendLine(header2.ChrNvramShiftCount, "  CHR-NVRAM shift count");
                builder.AppendLine(chrNvramSize, "  CHR-NVRAM size");

                // Byte 12
                string cpuTiming = header2.CPUPPUTiming.FromCPUPPUTiming();
                builder.AppendLine(cpuTiming, "  CPU timing");

                // Byte 13
                if (header.ConsoleType == ConsoleType.ExtendedConsoleType)
                {
                    string extendedConsoleTypeString = header2.ExtendedConsoleType.FromExtendedConsoleType();
                    builder.AppendLine(extendedConsoleTypeString, "  Extended console type");

                    builder.AppendLine(extendedConsoleTypeString, "  Reserved bits");
                }
                else if (header.ConsoleType == ConsoleType.VSUnisystem)
                {
                    string vsSystemTypeString = header2.VsSystemType.FromVsSystemType();
                    builder.AppendLine(vsSystemTypeString, "  Vs. system type");

                    string vsHardwareTypeString = header2.VsHardwareType.FromVsHardwareType();
                    builder.AppendLine(vsHardwareTypeString, "  Vs. hardware type");
                }
                else
                {
                    builder.AppendLine(header2.Reserved13, "  Reserved");
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
