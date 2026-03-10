using System;
using System.IO;
using SabreTools.Data.Models.NES;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class NESCart : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the base path
            string baseFilename = Filename is null
                ? Guid.NewGuid().ToString()
                : Path.GetFileNameWithoutExtension(Filename);
            string basePath = Path.Combine(outputDirectory, baseFilename);

            // Check if any data was extracted successfully
            bool success = false;

            // Header data
            if (Header is not null)
            {
                string headerPath = $"{basePath}.hdr";
                if (includeDebug) Console.WriteLine($"Attempting to extract header data to {headerPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(headerPath, FileMode.Create, FileAccess.Write, FileShare.None);

                    // Bytes 0-3
                    fs.Write(Header.IdentificationString);
                    fs.Flush();

                    // Byte 4
                    fs.Write(Header.PrgRomSize);
                    fs.Flush();

                    // Byte 5
                    fs.Write(Header.ChrRomSize);
                    fs.Flush();

                    // Byte 6
                    byte byte6 = 0;
                    byte6 |= (byte)NametableArrangement;
                    if (BatteryBackedPrgRam)
                        byte6 |= 0b00000010;
                    if (TrainerPresent)
                        byte6 |= 0b00000100;
                    if (AlternativeNametableLayout)
                        byte6 |= 0b00001000;
                    byte6 |= (byte)(Header.MapperLowerNibble << 4);
                    fs.Write(byte6);
                    fs.Flush();

                    // Byte 7
                    byte byte7 = 0;
                    byte7 |= (byte)ConsoleType;
                    if (NES20)
                        byte7 |= 0b00001000;
                    byte7 |= (byte)(Header.MapperUpperNibble << 4);
                    fs.Write(byte7);
                    fs.Flush();

                    if (Header is CartHeader1 header1)
                    {
                        // Byte 8
                        fs.Write(header1.PrgRamSize);
                        fs.Flush();

                        // Byte 9
                        fs.Write((byte)header1.TVSystem);
                        fs.Flush();

                        // Byte 10
                        byte byte10 = 0;
                        byte10 |= (byte)TVSystemExtended;
                        byte10 |= (byte)(header1.Byte10ReservedBits23 << 2);
                        if (PrgRamPresent)
                            byte10 |= 0b00010000;
                        if (HasBusConflicts)
                            byte10 |= 0b00100000;
                        byte10 |= (byte)(header1.Byte10ReservedBits67 << 6);
                        fs.Write(byte10);
                        fs.Flush();

                        // Bytes 11-15
                        fs.Write(header1.Padding);
                        fs.Flush();
                    }
                    else if (Header is CartHeader2 header2)
                    {
                        // Byte 8
                        byte byte8 = 0;
                        byte8 |= header2.MapperMSB;
                        byte8 |= (byte)(header2.Submapper << 4);
                        fs.Write(byte8);
                        fs.Flush();

                        // Byte 9
                        byte byte9 = 0;
                        byte9 |= header2.PrgRomSizeMSB;
                        byte9 |= (byte)(header2.ChrRomSizeMSB << 4);
                        fs.Write(byte9);
                        fs.Flush();

                        // Byte 10
                        byte byte10 = 0;
                        byte10 |= header2.PrgRamShiftCount;
                        byte10 |= (byte)(header2.PrgNvramEepromShiftCount << 4);
                        fs.Write(byte10);
                        fs.Flush();

                        // Byte 11
                        byte byte11 = 0;
                        byte11 |= header2.ChrRamShiftCount;
                        byte11 |= (byte)(header2.ChrNvramShiftCount << 4);
                        fs.Write(byte11);
                        fs.Flush();

                        // Byte 12
                        fs.Write((byte)header2.CPUPPUTiming);
                        fs.Flush();

                        // Byte 13
                        if (ConsoleType == ConsoleType.VSUnisystem)
                        {
                            byte byte13 = 0;
                            byte13 |= (byte)header2.VsSystemType;
                            byte13 |= (byte)((byte)header2.VsHardwareType << 4);
                            fs.Write(byte13);
                            fs.Flush();
                        }
                        else if (ConsoleType == ConsoleType.ExtendedConsoleType)
                        {
                            byte byte13 = 0;
                            byte13 |= (byte)header2.ExtendedConsoleType;
                            byte13 |= (byte)(header2.Byte13ReservedBits47 << 4);
                            fs.Write(byte13);
                            fs.Flush();
                        }
                        else
                        {
                            fs.Write(header2.Reserved13);
                            fs.Flush();
                        }

                        // Byte 14
                        fs.Write(header2.MiscellaneousROMs);
                        fs.Flush();

                        // Byte 15
                        fs.Write((byte)DefaultExpansionDevice);
                        fs.Flush();
                    }
                    else
                    {
                        if (includeDebug) Console.Error.WriteLine("Unknown header type, incomplete header data output");
                    }

                    // Header extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // Trainer data
            if (Trainer.Length > 0)
            {
                string trainerPath = $"{basePath}.trn";
                if (includeDebug) Console.WriteLine($"Attempting to extract trainer data to {trainerPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(trainerPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(Trainer, 0, Trainer.Length);
                    fs.Flush();

                    // Trainer extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // PRG-ROM data
            if (PrgRomData.Length > 0)
            {
                string prgRomPath = $"{basePath}.prg";
                if (includeDebug) Console.WriteLine($"Attempting to extract PRG-ROM data to {prgRomPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(prgRomPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(PrgRomData, 0, PrgRomData.Length);
                    fs.Flush();

                    // PRG-ROM extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // CHR-ROM data
            if (ChrRomData.Length > 0)
            {
                string chrRomPath = $"{basePath}.chr";
                if (includeDebug) Console.WriteLine($"Attempting to extract CHR-ROM data to {chrRomPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(chrRomPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(ChrRomData, 0, ChrRomData.Length);
                    fs.Flush();

                    // CHR-ROM extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // Headerless ROM data
            if (PrgRomData.Length > 0 && ChrRomData.Length > 0)
            {
                string unheaderedPath = $"{basePath}.unh";
                if (includeDebug) Console.WriteLine($"Attempting to extract unheadered ROM to {unheaderedPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(unheaderedPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(PrgRomData, 0, PrgRomData.Length);
                    fs.Flush();
                    fs.Write(ChrRomData, 0, ChrRomData.Length);
                    fs.Flush();

                    // Unheadered ROM extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // INST-ROM data
            if (PlayChoiceInstRom.Length > 0)
            {
                string instRomPath = $"{basePath}.inst";
                if (includeDebug) Console.WriteLine($"Attempting to extract PlayChoice-10 INST-ROM data to {instRomPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(instRomPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(PlayChoiceInstRom, 0, PlayChoiceInstRom.Length);
                    fs.Flush();

                    // PC10 INST-ROM extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // TODO: Add PlayChoice-10 PROM data extraction

            return success;
        }
    }
}
