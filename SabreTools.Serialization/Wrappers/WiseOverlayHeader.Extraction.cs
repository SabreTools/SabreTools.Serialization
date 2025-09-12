using System;
using System.IO;
using SabreTools.IO.Compression.Deflate;
using SabreTools.IO.Streams;
using SabreTools.Models.WiseInstaller.Actions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WiseOverlayHeader
    {
        /// <summary>
        /// Extract the predefined, static files defined in the header
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the files extracted successfully, false otherwise</returns>
        /// <remarks>On success, this sets <see cref="InstallerDataOffset"/></remarks>
        public bool ExtractHeaderDefinedFiles(string outputDirectory, bool includeDebug)
        {
            lock (_dataSourceLock)
            {
                // Seek to the compressed data offset
                _dataSource.Seek(CompressedDataOffset, SeekOrigin.Begin);
                if (includeDebug) Console.WriteLine($"Beginning of header-defined files: {CompressedDataOffset}");

                // Extract WiseColors.dib, if it exists
                var expected = new DeflateInfo { InputSize = DibDeflatedSize, OutputSize = DibInflatedSize, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, "WiseColors.dib", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract WiseScript.bin
                expected = new DeflateInfo { InputSize = WiseScriptDeflatedSize, OutputSize = WiseScriptInflatedSize, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, "WiseScript.bin", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract WISE0001.DLL, if it exists
                expected = new DeflateInfo { InputSize = WiseDllDeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "WISE0001.DLL", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract CTL3D32.DLL, if it exists
                expected = new DeflateInfo { InputSize = Ctl3d32DeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "CTL3D32.DLL", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract FILE0004, if it exists
                expected = new DeflateInfo { InputSize = SomeData4DeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "FILE0004", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract Ocxreg32.EXE, if it exists
                expected = new DeflateInfo { InputSize = RegToolDeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "Ocxreg32.EXE", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract PROGRESS.DLL, if it exists
                expected = new DeflateInfo { InputSize = ProgressDllDeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "PROGRESS.DLL", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract FILE0007, if it exists
                expected = new DeflateInfo { InputSize = SomeData7DeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "FILE0007", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract FILE0008, if it exists
                expected = new DeflateInfo { InputSize = SomeData8DeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "FILE0008", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract FILE0009, if it exists
                expected = new DeflateInfo { InputSize = SomeData9DeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "FILE0009", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract FILE000A, if it exists
                expected = new DeflateInfo { InputSize = SomeData10DeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "FILE000A", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract install script, if it exists
                expected = new DeflateInfo { InputSize = InstallScriptDeflatedSize, OutputSize = -1, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "INSTALL_SCRIPT", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                // Extract FILE000{n}.DAT, if it exists
                expected = new DeflateInfo { InputSize = FinalFileDeflatedSize, OutputSize = FinalFileInflatedSize, Crc32 = 0 };
                if (InflateWrapper.ExtractFile(_dataSource, IsPKZIP ? null : "FILE00XX.DAT", outputDirectory, expected, IsPKZIP, includeDebug) == ExtractionStatus.FAIL)
                    return false;

                InstallerDataOffset = _dataSource.Position;
            }

            return true;
        }

        /// <summary>
        /// Attempt to extract a file defined by a file header
        /// </summary>
        /// <param name="obj">Deflate information</param>
        /// <param name="index">File index for automatic naming</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted successfully, false otherwise</returns>
        /// <remarks>Requires <see cref="InstallerDataOffset"/> to be set</remarks> 
        public ExtractionStatus ExtractFile(InstallFile obj, int index, string outputDirectory, bool includeDebug)
        {
            // Get expected values
            var expected = new DeflateInfo
            {
                InputSize = obj.DeflateEnd - obj.DeflateStart,
                OutputSize = obj.InflatedSize,
                Crc32 = obj.Crc32,
            };

            // Perform path replacements
            string filename = obj.DestinationPathname ?? $"WISE{index:X4}";
            filename = filename.Replace("%", string.Empty);

            lock (_dataSourceLock)
            {
                _dataSource.Seek(InstallerDataOffset + obj.DeflateStart, SeekOrigin.Begin);
                return InflateWrapper.ExtractFile(_dataSource,
                    filename,
                    outputDirectory,
                    expected,
                    IsPKZIP,
                    includeDebug);
            }
        }

        /// <summary>
        /// Attempt to extract a file defined by a file header
        /// </summary>
        /// <param name="obj">Deflate information</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted successfully, false otherwise</returns>
        /// <remarks>Requires <see cref="InstallerDataOffset"/> to be set</remarks> 
        public ExtractionStatus ExtractFile(DisplayBillboard obj, string outputDirectory, bool includeDebug)
        {
            // Get the generated base name
            string baseName = $"CustomBillboardSet_{obj.Flags:X4}-{obj.Operand_2}-{obj.Operand_3}";

            // If there are no deflate objects
            if (obj.DeflateInfo == null)
            {
                if (includeDebug) Console.WriteLine($"Skipping {baseName} because the deflate object array is null!");
                return ExtractionStatus.FAIL;
            }

            // Loop through the values
            for (int i = 0; i < obj.DeflateInfo.Length; i++)
            {
                // Get the deflate info object
                var info = obj.DeflateInfo[i];

                // Get expected values
                var expected = new DeflateInfo
                {
                    InputSize = info.DeflateEnd - info.DeflateStart,
                    OutputSize = info.InflatedSize,
                    Crc32 = 0,
                };

                // Perform path replacements
                string filename = $"{baseName}{i:X4}";

                lock (_dataSourceLock)
                {
                    _dataSource.Seek(InstallerDataOffset + info.DeflateStart, SeekOrigin.Begin);
                    _ = InflateWrapper.ExtractFile(_dataSource, filename, outputDirectory, expected, IsPKZIP, includeDebug);
                }
            }

            // Always return good -- TODO: Fix this
            return ExtractionStatus.GOOD;
        }

        /// <summary>
        /// Attempt to extract a file defined by a file header
        /// </summary>
        /// <param name="obj">Deflate information</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted successfully, false otherwise</returns>
        /// <remarks>Requires <see cref="InstallerDataOffset"/> to be set</remarks> 
        public ExtractionStatus ExtractFile(CustomDialogSet obj, string outputDirectory, bool includeDebug)
        {
            // Get expected values
            var expected = new DeflateInfo
            {
                InputSize = obj.DeflateEnd - obj.DeflateStart,
                OutputSize = obj.InflatedSize,
                Crc32 = 0,
            };

            // Perform path replacements
            string filename = $"CustomDialogSet_{obj.DisplayVariable}-{obj.Name}";
            filename = filename.Replace("%", string.Empty);

            lock (_dataSourceLock)
            {
                _dataSource.Seek(InstallerDataOffset + obj.DeflateStart, SeekOrigin.Begin);
                return InflateWrapper.ExtractFile(_dataSource, filename, outputDirectory, expected, IsPKZIP, includeDebug);
            }
        }

        /// <summary>
        /// Open a potential WISE installer file and any additional files
        /// </summary>
        /// <param name="filename">Input filename or base name to read from</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file could be opened, false otherwise</returns>
        public static bool OpenFile(string filename, bool includeDebug, out ReadOnlyCompositeStream? stream)
        {
            // If the file exists as-is
            if (File.Exists(filename))
            {
                var fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream = new ReadOnlyCompositeStream([fileStream]);

                // Debug statement
                if (includeDebug) Console.WriteLine($"File {filename} was found and opened");

                // Strip the extension and rebuild
                string? directory = Path.GetDirectoryName(filename);
                filename = Path.GetFileNameWithoutExtension(filename);
                if (directory != null)
                    filename = Path.Combine(directory, filename);
            }

            // If the base name was provided, try to open the associated exe
            else if (File.Exists($"{filename}.EXE"))
            {
                var fileStream = File.Open($"{filename}.EXE", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream = new ReadOnlyCompositeStream([fileStream]);

                // Debug statement
                if (includeDebug) Console.WriteLine($"File {filename}.EXE was found and opened");
            }
            else if (File.Exists($"{filename}.exe"))
            {
                var fileStream = File.Open($"{filename}.exe", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream = new ReadOnlyCompositeStream([fileStream]);

                // Debug statement
                if (includeDebug) Console.WriteLine($"File {filename}.exe was found and opened");
            }

            // Otherwise, the file cannot be opened
            else
            {
                stream = null;
                return false;
            }

            // Get the pattern for file naming
            string filePattern = string.Empty;
            bool longDigits = false;

            byte fileno = 0;
            bool foundStart = false;
            for (; fileno < 3; fileno++)
            {
                if (File.Exists($"{filename}.W0{fileno}"))
                {
                    foundStart = true;
                    filePattern = $"{filename}.W";
                    longDigits = false;
                    break;
                }
                else if (File.Exists($"{filename}.w0{fileno}"))
                {
                    foundStart = true;
                    filePattern = $"{filename}.w";
                    longDigits = false;
                    break;
                }
                else if (File.Exists($"{filename}.00{fileno}"))
                {
                    foundStart = true;
                    filePattern = $"{filename}.";
                    longDigits = true;
                    break;
                }
            }

            // If no starting part has been found
            if (!foundStart)
                return true;

            // Loop through and try to read all additional files
            for (; ; fileno++)
            {
                string nextPart = longDigits ? $"{filePattern}{fileno:D3}" : $"{filePattern}{fileno:D2}";
                if (!File.Exists(nextPart))
                {
                    if (includeDebug) Console.WriteLine($"Part {nextPart} was not found");
                    break;
                }

                // Debug statement
                if (includeDebug) Console.WriteLine($"Part {nextPart} was found and appended");

                var fileStream = File.Open(nextPart, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream.AddStream(fileStream);
            }

            return true;
        }
    }
}
