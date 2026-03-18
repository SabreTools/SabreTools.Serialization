using System;
using System.IO;
using SabreTools.Data.Models.Atari7800;
using SabreTools.IO.Extensions;

namespace SabreTools.Wrappers
{
    public partial class Atari7800Cart : IExtractable
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

                    // Byte 0
                    fs.Write(Header.HeaderVersion);
                    fs.Flush();

                    // Bytes 1-16
                    fs.Write(Header.MagicText);
                    fs.Flush();

                    // Bytes 17-48
                    fs.Write(Header.CartTitle);
                    fs.Flush();

                    // Bytes 49-52
                    fs.Write(Header.RomSizeWithoutHeader);
                    fs.Flush();

                    // Bytes 53-54
                    fs.Write((ushort)Header.CartType);
                    fs.Flush();

                    // Bytes 55-56
                    fs.Write((byte)Header.Controller1Type);
                    fs.Write((byte)Header.Controller2Type);
                    fs.Flush();

                    // Byte 57
                    fs.Write((byte)Header.TVType);
                    fs.Flush();

                    // Byte 58
                    fs.Write((byte)Header.SaveDevice);
                    fs.Flush();

                    // Bytes 59-62
                    fs.Write(Header.Reserved);
                    fs.Flush();

                    // Byte 63
                    fs.Write((byte)Header.SlotPassthroughDevice);
                    fs.Flush();

                    if (HeaderVersion >= 4)
                    {
                        // Byte 64
                        fs.Write((byte)Header.Mapper);
                        fs.Flush();

                        // Byte 65
                        fs.Write((byte)Header.MapperOptions);
                        fs.Flush();

                        // Byte 66-67
                        fs.Write((ushort)Header.AudioDevice);
                        fs.Flush();

                        // Byte 68-69
                        fs.Write((ushort)Header.Interrupt);
                        fs.Flush();

                        // Bytes 70-99
                        fs.Write(Header.Padding);
                        fs.Flush();
                    }
                    else
                    {
                        // Bytes 64-99
                        fs.Write(Header.Padding);
                        fs.Flush();
                    }

                    // Bytes 100-127
                    fs.Write(Header.EndMagicText);
                    fs.Flush();

                    // Header extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            // ROM data
            if (Model.Data.Length > 0)
            {
                string romPath = $"{basePath}.bin";
                if (includeDebug) Console.WriteLine($"Attempting to extract ROM data to {romPath}");

                // Try to write the data
                try
                {
                    // Open the output file for writing
                    using var fs = File.Open(romPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fs.Write(Model.Data, 0, Model.Data.Length);
                    fs.Flush();

                    // ROM extracted
                    success = true;
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            return success;
        }
    }
}
