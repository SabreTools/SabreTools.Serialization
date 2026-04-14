using System;
using System.IO;
using SabreTools.Data.Models.NES;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class NESCart : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Ensure an output path
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".nes")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            // Check for invalid data
            if (Header is null || PrgRomData is null || PrgRomData.Length == 0)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            // Open the output file for writing
            using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

            // Header data
            if (!WriteHeader(fs, includeDebug))
                return false;

            // Trainer data
            if (Trainer is not null && Trainer.Length > 0)
                WriteTrainer(fs, includeDebug);

            // PRG-ROM data
            if (!WritePrgRom(fs, includeDebug))
                return false;

            // CHR-ROM data
            if (ChrRomData is not null && ChrRomData.Length > 0)
                WriteChrRom(fs, includeDebug);

            // INST-ROM data
            if (PlayChoiceInstRom is not null && PlayChoiceInstRom.Length > 0)
                WritePlayChoiceInstRom(fs, includeDebug);

            // TODO: Add PlayChoice-10 PROM data writing

            return true;
        }

        /// <summary>
        /// Write CHR-ROM data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteChrRom(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write CHR-ROM data");

            // Try to write the data
            try
            {
                stream.Write(ChrRomData, 0, ChrRomData.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write header data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteHeader(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write header data");

            if (Header is null)
            {
                if (includeDebug) Console.WriteLine("Header was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                // Bytes 0-3
                stream.Write(Header.IdentificationString);
                stream.Flush();

                // Byte 4
                stream.Write(Header.PrgRomSize);
                stream.Flush();

                // Byte 5
                stream.Write(Header.ChrRomSize);
                stream.Flush();

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
                stream.Write(byte6);
                stream.Flush();

                // Byte 7
                byte byte7 = 0;
                byte7 |= (byte)ConsoleType;
                if (NES20)
                    byte7 |= 0b00001000;
                byte7 |= (byte)(Header.MapperUpperNibble << 4);
                stream.Write(byte7);
                stream.Flush();

                if (Header is CartHeader1 header1)
                {
                    // Byte 8
                    stream.Write(header1.PrgRamSize);
                    stream.Flush();

                    // Byte 9
                    stream.Write((byte)header1.TVSystem);
                    stream.Flush();

                    // Byte 10
                    byte byte10 = 0;
                    byte10 |= (byte)TVSystemExtended;
                    byte10 |= (byte)(header1.Byte10ReservedBits23 << 2);
                    if (PrgRamPresent)
                        byte10 |= 0b00010000;
                    if (HasBusConflicts)
                        byte10 |= 0b00100000;
                    byte10 |= (byte)(header1.Byte10ReservedBits67 << 6);
                    stream.Write(byte10);
                    stream.Flush();

                    // Bytes 11-15
                    stream.Write(header1.Padding);
                    stream.Flush();
                }
                else if (Header is CartHeader2 header2)
                {
                    // Byte 8
                    byte byte8 = 0;
                    byte8 |= header2.MapperMSB;
                    byte8 |= (byte)(header2.Submapper << 4);
                    stream.Write(byte8);
                    stream.Flush();

                    // Byte 9
                    byte byte9 = 0;
                    byte9 |= header2.PrgRomSizeMSB;
                    byte9 |= (byte)(header2.ChrRomSizeMSB << 4);
                    stream.Write(byte9);
                    stream.Flush();

                    // Byte 10
                    byte byte10 = 0;
                    byte10 |= header2.PrgRamShiftCount;
                    byte10 |= (byte)(header2.PrgNvramEepromShiftCount << 4);
                    stream.Write(byte10);
                    stream.Flush();

                    // Byte 11
                    byte byte11 = 0;
                    byte11 |= header2.ChrRamShiftCount;
                    byte11 |= (byte)(header2.ChrNvramShiftCount << 4);
                    stream.Write(byte11);
                    stream.Flush();

                    // Byte 12
                    stream.Write((byte)header2.CPUPPUTiming);
                    stream.Flush();

                    // Byte 13
                    if (ConsoleType == ConsoleType.VSUnisystem)
                    {
                        byte byte13 = 0;
                        byte13 |= (byte)header2.VsSystemType;
                        byte13 |= (byte)((byte)header2.VsHardwareType << 4);
                        stream.Write(byte13);
                        stream.Flush();
                    }
                    else if (ConsoleType == ConsoleType.ExtendedConsoleType)
                    {
                        byte byte13 = 0;
                        byte13 |= (byte)header2.ExtendedConsoleType;
                        byte13 |= (byte)(header2.Byte13ReservedBits47 << 4);
                        stream.Write(byte13);
                        stream.Flush();
                    }
                    else
                    {
                        stream.Write(header2.Reserved13);
                        stream.Flush();
                    }

                    // Byte 14
                    stream.Write(header2.MiscellaneousROMs);
                    stream.Flush();

                    // Byte 15
                    stream.Write((byte)DefaultExpansionDevice);
                    stream.Flush();
                }
                else
                {
                    if (includeDebug) Console.Error.WriteLine("Unknown header type, incomplete header data output");
                }

                // Header extracted
                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write INST-ROM data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WritePlayChoiceInstRom(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write INST-ROM data");

            // Try to write the data
            try
            {
                stream.Write(PlayChoiceInstRom, 0, PlayChoiceInstRom.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write PRG-ROM data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WritePrgRom(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write PRG-ROM data");

            // Try to write the data
            try
            {
                stream.Write(PrgRomData, 0, PrgRomData.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write trainer data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteTrainer(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to write trainer data");

            // Try to write the data
            try
            {
                stream.Write(Trainer, 0, Trainer.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
