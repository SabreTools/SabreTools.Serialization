using System;
using System.IO;
using SabreTools.Data.Models.Atari7800;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Wrappers
{
    public partial class Atari7800Cart : IWritable
    {
        /// <inheritdoc/>
        public bool Write(string outputPath, bool includeDebug)
        {
            // Ensure an output path
            if (string.IsNullOrEmpty(outputPath))
            {
                string outputFilename = Filename is null
                    ? (Guid.NewGuid().ToString() + ".a78")
                    : (Filename + ".new");
                outputPath = Path.GetFullPath(outputFilename);
            }

            // Check for invalid data
            if (Header is null || Model.Data is null || Model.Data.Length == 0)
            {
                if (includeDebug) Console.WriteLine("Model was invalid, cannot write!");
                return false;
            }

            // Open the output file for writing
            using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

            // Header data
            if (!WriteHeader(fs, includeDebug))
                return false;

            // ROM data
            if (!WriteRom(fs, includeDebug))
                return false;

            return true;
        }

        /// <summary>
        /// Write header data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteHeader(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to extract header data");

            if (Header is null)
            {
                if (includeDebug) Console.WriteLine("Header was invalid!");
                return false;
            }

            // Try to write the data
            try
            {
                // Byte 0
                stream.Write(Header.HeaderVersion);
                stream.Flush();

                // Bytes 1-16
                stream.Write(Header.MagicText);
                stream.Flush();

                // Bytes 17-48
                stream.Write(Header.CartTitle);
                stream.Flush();

                // Bytes 49-52
                stream.Write(Header.RomSizeWithoutHeader);
                stream.Flush();

                // Bytes 53-54
                stream.Write((ushort)Header.CartType);
                stream.Flush();

                // Bytes 55-56
                stream.Write((byte)Header.Controller1Type);
                stream.Write((byte)Header.Controller2Type);
                stream.Flush();

                // Byte 57
                stream.Write((byte)Header.TVType);
                stream.Flush();

                // Byte 58
                stream.Write((byte)Header.SaveDevice);
                stream.Flush();

                // Bytes 59-62
                stream.Write(Header.Reserved);
                stream.Flush();

                // Byte 63
                stream.Write((byte)Header.SlotPassthroughDevice);
                stream.Flush();

                if (HeaderVersion >= 4)
                {
                    // Byte 64
                    stream.Write((byte)Header.Mapper);
                    stream.Flush();

                    // Byte 65
                    stream.Write((byte)Header.MapperOptions);
                    stream.Flush();

                    // Byte 66-67
                    stream.Write((ushort)Header.AudioDevice);
                    stream.Flush();

                    // Byte 68-69
                    stream.Write((ushort)Header.Interrupt);
                    stream.Flush();

                    // Bytes 70-99
                    stream.Write(Header.Padding);
                    stream.Flush();
                }
                else
                {
                    // Bytes 64-99
                    stream.Write(Header.Padding);
                    stream.Flush();
                }

                // Bytes 100-127
                stream.Write(Header.EndMagicText);
                stream.Flush();

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
        /// Write rom data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the writing was successful, false otherwise</returns>
        private bool WriteRom(Stream stream, bool includeDebug)
        {
            if (includeDebug) Console.WriteLine("Attempting to extract ROM data");

            // Try to write the data
            try
            {
                stream.Write(Model.Data, 0, Model.Data.Length);
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
