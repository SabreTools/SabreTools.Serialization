using System;
using System.IO;
using SabreTools.Data.Models.NES;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class NESCart : BaseBinaryWriter<Cart>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Cart? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();

            // Write header
            if (!WriteHeader(obj.Header, stream))
                return null;

            // Trainer data
            if (obj.Trainer is not null && obj.Trainer.Length > 0 && !WriteTrainer(obj.Trainer, stream))
                return null;

            // PRG-ROM data
            if (!WritePrgRom(obj.PrgRomData, stream))
                return null;

            // CHR-ROM data
            if (obj.ChrRomData is not null && obj.ChrRomData.Length > 0 && !WriteChrRom(obj.ChrRomData, stream))
                return null;

            // INST-ROM data
            if (obj.PlayChoiceInstRom is not null && obj.PlayChoiceInstRom.Length > 0 && !WritePlayChoiceInstRom(obj.PlayChoiceInstRom, stream))
                return null;

            // TODO: Add PlayChoice-10 PROM data writing

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write CHR-ROM data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WriteChrRom(byte[]? obj, Stream stream)
        {
            if (obj is null || obj.Length == 0)
            {
                if (Debug) Console.WriteLine("CHR-ROM data was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write CHR-ROM data");

                stream.Write(obj, 0, obj.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write header data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WriteHeader(CartHeader? obj, Stream stream)
        {
            if (obj is null)
            {
                if (Debug) Console.WriteLine("Header was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write header data");

                // Bytes 0-3
                stream.Write(obj.IdentificationString);
                stream.Flush();

                // Byte 4
                stream.Write(obj.PrgRomSize);
                stream.Flush();

                // Byte 5
                stream.Write(obj.ChrRomSize);
                stream.Flush();

                // Byte 6
                byte byte6 = 0;
                byte6 |= (byte)obj.NametableArrangement;
                if (obj.BatteryBackedPrgRam)
                    byte6 |= 0b00000010;
                if (obj.TrainerPresent)
                    byte6 |= 0b00000100;
                if (obj.AlternativeNametableLayout)
                    byte6 |= 0b00001000;
                byte6 |= (byte)(obj.MapperLowerNibble << 4);
                stream.Write(byte6);
                stream.Flush();

                // Byte 7
                byte byte7 = 0;
                byte7 |= (byte)obj.ConsoleType;
                if (obj.NES20)
                    byte7 |= 0b00001000;
                byte7 |= (byte)(obj.MapperUpperNibble << 4);
                stream.Write(byte7);
                stream.Flush();

                if (obj is CartHeader1 header1)
                {
                    // Byte 8
                    stream.Write(header1.PrgRamSize);
                    stream.Flush();

                    // Byte 9
                    stream.Write((byte)header1.TVSystem);
                    stream.Flush();

                    // Byte 10
                    byte byte10 = 0;
                    byte10 |= (byte)header1.TVSystemExtended;
                    byte10 |= (byte)(header1.Byte10ReservedBits23 << 2);
                    if (header1.PrgRamPresent)
                        byte10 |= 0b00010000;
                    if (header1.HasBusConflicts)
                        byte10 |= 0b00100000;
                    byte10 |= (byte)(header1.Byte10ReservedBits67 << 6);
                    stream.Write(byte10);
                    stream.Flush();

                    // Bytes 11-15
                    stream.Write(header1.Padding);
                    stream.Flush();
                }
                else if (obj is CartHeader2 header2)
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
                    if (obj.ConsoleType == ConsoleType.VSUnisystem)
                    {
                        byte byte13 = 0;
                        byte13 |= (byte)header2.VsSystemType;
                        byte13 |= (byte)((byte)header2.VsHardwareType << 4);
                        stream.Write(byte13);
                        stream.Flush();
                    }
                    else if (obj.ConsoleType == ConsoleType.ExtendedConsoleType)
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
                    stream.Write((byte)header2.DefaultExpansionDevice);
                    stream.Flush();
                }
                else
                {
                    if (Debug) Console.Error.WriteLine("Unknown header type, incomplete header data output");
                }

                // Header extracted
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write INST-ROM data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WritePlayChoiceInstRom(byte[]? obj, Stream stream)
        {
            if (obj is null || obj.Length == 0)
            {
                if (Debug) Console.WriteLine("INST-ROM data was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write INST-ROM data");

                stream.Write(obj, 0, obj.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write PRG-ROM data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WritePrgRom(byte[]? obj, Stream stream)
        {
            if (obj is null || obj.Length == 0)
            {
                if (Debug) Console.WriteLine("PRG-ROM data was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write PRG-ROM data");

                stream.Write(obj, 0, obj.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write trainer data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        public bool WriteTrainer(byte[]? obj, Stream stream)
        {
            if (obj is null || obj.Length == 0)
            {
                if (Debug) Console.WriteLine("Trainer data was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                if (Debug) Console.WriteLine("Attempting to write trainer data");

                stream.Write(obj, 0, obj.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                if (Debug) Console.Error.WriteLine(ex);
                return false;
            }
        }
    }
}
