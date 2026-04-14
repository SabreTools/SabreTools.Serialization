using System;
using System.IO;

namespace SabreTools.Wrappers
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
                    using var fs = File.Open(headerPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    success |= WriteHeader(fs, includeDebug);
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
                    using var fs = File.Open(trainerPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    success |= WriteTrainer(fs, includeDebug);
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
                    success |= WritePrgRom(fs, includeDebug);
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
                    success |= WriteChrRom(fs, includeDebug);
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
                    success |= WritePrgRom(fs, includeDebug);
                    success |= WriteChrRom(fs, includeDebug);

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
                    success |= WritePlayChoiceInstRom(fs, includeDebug);
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
