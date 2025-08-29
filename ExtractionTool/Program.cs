using System;
using System.IO;
using System.Text.RegularExpressions;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace ExtractionTool
{
    class Program
    {
        static void Main(string[] args)
        {
#if NET462_OR_GREATER || NETCOREAPP
            // Register the codepages
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

            // Get the options from the arguments
            var options = Options.ParseOptions(args);

            // If we have an invalid state
            if (options == null)
            {
                Options.DisplayHelp();
                return;
            }

            // Loop through the input paths
            foreach (string inputPath in options.InputPaths)
            {
                ExtractPath(inputPath, options.OutputPath, options.Debug);
            }
        }

        /// <summary>
        /// Wrapper to extract data for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="outputDirectory">Output directory path</param>
        /// <param name="includeDebug">Enable including debug information</param>
        private static void ExtractPath(string path, string outputDirectory, bool includeDebug)
        {
            // Normalize by getting the full path
            path = Path.GetFullPath(path);
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                ExtractFile(path, outputDirectory, includeDebug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    ExtractFile(file, outputDirectory, includeDebug);
                }
            }
            else
            {
                Console.WriteLine($"{path} does not exist, skipping...");
            }
        }

        /// <summary>
        /// Print information for a single file, if possible
        /// </summary>
        private static void ExtractFile(string file, string outputDirectory, bool includeDebug)
        {
            Console.WriteLine($"Attempting to extract all files from {file}");
            using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Get the first 16 bytes for matching
            byte[] magic = new byte[16];
            try
            {
                int read = stream.Read(magic, 0, 16);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return;
            }

            // Get the file type
            WrapperType ft = WrapperFactory.GetFileType(magic, extension);
            var wrapper = WrapperFactory.CreateWrapper(ft, stream);

            // Create the output directory
            Directory.CreateDirectory(outputDirectory);

            // 7-zip
            if (wrapper is SevenZip sz)
            {
                Console.WriteLine("Extracting 7-zip contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                sz.Extract(outputDirectory, includeDebug);
#endif
            }

            // BFPK archive
            else if (wrapper is BFPK bfpk)
            {
                Console.WriteLine("Extracting BFPK contents");
                Console.WriteLine();

                bfpk.Extract(outputDirectory, includeDebug);
            }

            // BSP
            else if (wrapper is BSP bsp)
            {
                Console.WriteLine("Extracting BSP contents");
                Console.WriteLine();

                bsp.Extract(outputDirectory, includeDebug);
            }

            // bzip2
            else if (wrapper is BZip2 bzip2)
            {
                Console.WriteLine("Extracting bzip2 contents");
                Console.WriteLine();

                bzip2.Extract(outputDirectory, includeDebug);
            }

            // CFB
            else if (wrapper is CFB cfb)
            {
                // Build the installer information
                Console.WriteLine("Extracting CFB contents");
                Console.WriteLine();

#if NET20 || NET35
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                cfb.Extract(outputDirectory, includeDebug);
#endif
            }

            // GCF
            else if (wrapper is GCF gcf)
            {
                Console.WriteLine("Extracting GCF contents");
                Console.WriteLine();

                gcf.Extract(outputDirectory, includeDebug);
            }

            // gzip
            else if (wrapper is GZip gzip)
            {
                Console.WriteLine("Extracting gzip contents");
                Console.WriteLine();

                gzip.Extract(outputDirectory, includeDebug);
            }

            // InstallShield Archive V3 (Z)
            else if (wrapper is InstallShieldArchiveV3 isv3)
            {
                Console.WriteLine("Extracting InstallShield Archive V3 contents");
                Console.WriteLine();

                isv3.Extract(outputDirectory, includeDebug);
            }

            // IS-CAB archive
            else if (wrapper is InstallShieldCabinet)
            {
                Console.WriteLine("Extracting IS-CAB contents");
                Console.WriteLine();

                // TODO: Move this handling to Serialization directly
                ExtractInstallShieldCabinet(file, outputDirectory, includeDebug);
            }

            // LZ-compressed file, KWAJ variant
            else if (wrapper is LZKWAJ kwaj)
            {
                Console.WriteLine("Extracting LZ-compressed file, KWAJ variant contents");
                Console.WriteLine();

                kwaj.Extract(outputDirectory, includeDebug);
            }

            // LZ-compressed file, QBasic variant
            else if (wrapper is LZQBasic qbasic)
            {
                Console.WriteLine("Extracting LZ-compressed file, QBasic variant contents");
                Console.WriteLine();

                qbasic.Extract(outputDirectory, includeDebug);
            }

            // LZ-compressed file, SZDD variant
            else if (wrapper is LZSZDD szdd)
            {
                Console.WriteLine("Extracting LZ-compressed file, SZDD variant contents");
                Console.WriteLine();

                szdd.Extract(outputDirectory, includeDebug);
            }

            // Microsoft Cabinet archive
            else if (wrapper is MicrosoftCabinet mscab)
            {
                Console.WriteLine("Extracting MS-CAB contents");
                Console.WriteLine("WARNING: LZX and Quantum compression schemes are not supported so some files may be skipped!");
                Console.WriteLine();

                MicrosoftCabinet.ExtractSet(file, outputDirectory, includeDebug);
            }

            // MoPaQ (MPQ) archive -- Reimplement
            // else if (wrapper is MoPaQ mpq)
            // {
            //     Console.WriteLine("Extracting MoPaQ contents");
            //     Console.WriteLine();

#if NET20 || NET35 || !(WINX86 || WINX64)
            //     Console.WriteLine("Extraction is not supported for this framework!");
            //     Console.WriteLine();
#else
            //     mpq.Extract(stream, file, outputDirectory, includeDebug: true);
#endif
            // }

            // New Executable
            else if (wrapper is NewExecutable nex)
            {
                Console.WriteLine("Extracting New Executable contents");
                Console.WriteLine();

                nex.Extract(outputDirectory, includeDebug);
            }

            // PAK
            else if (wrapper is PAK pak)
            {
                Console.WriteLine("Extracting PAK contents");
                Console.WriteLine();

                pak.Extract(outputDirectory, includeDebug);
            }

            // PFF
            else if (wrapper is PFF pff)
            {
                Console.WriteLine("Extracting PFF contents");
                Console.WriteLine();

                pff.Extract(outputDirectory, includeDebug);
            }

            // PKZIP
            else if (wrapper is PKZIP pkzip)
            {
                Console.WriteLine("Extracting PKZIP contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                pkzip.Extract(outputDirectory, includeDebug);
#endif
            }

            // Portable Executable
            else if (wrapper is PortableExecutable pex)
            {
                Console.WriteLine("Extracting Portable Executable contents");
                Console.WriteLine();

                pex.Extract(outputDirectory, includeDebug);
            }

            // Quantum
            else if (wrapper is Quantum quantum)
            {
                Console.WriteLine("Extracting Quantum contents");
                Console.WriteLine();

                quantum.Extract(outputDirectory, includeDebug);
            }

            // RAR
            else if (wrapper is RAR rar)
            {
                Console.WriteLine("Extracting RAR contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                rar.Extract(outputDirectory, includeDebug);
#endif
            }

            // SGA
            else if (wrapper is SGA sga)
            {
                Console.WriteLine("Extracting SGA contents");
                Console.WriteLine();

                sga.Extract(outputDirectory, includeDebug);
            }

            // Tape Archive
            else if (wrapper is TapeArchive tar)
            {
                Console.WriteLine("Extracting Tape Archive contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                tar.Extract(outputDirectory, includeDebug);
#endif
            }

            // VBSP
            else if (wrapper is VBSP vbsp)
            {
                Console.WriteLine("Extracting VBSP contents");
                Console.WriteLine();

                vbsp.Extract(outputDirectory, includeDebug);
            }

            // VPK
            else if (wrapper is VPK vpk)
            {
                Console.WriteLine("Extracting VPK contents");
                Console.WriteLine();

                vpk.Extract(outputDirectory, includeDebug);
            }

            // WAD3
            else if (wrapper is WAD3 wad)
            {
                Console.WriteLine("Extracting WAD3 contents");
                Console.WriteLine();

                wad.Extract(outputDirectory, includeDebug);
            }

            // xz
            else if (wrapper is XZ xz)
            {
                Console.WriteLine("Extracting xz contents");
                Console.WriteLine();

#if NET20 || NET35 || NET40 || NET452
                Console.WriteLine("Extraction is not supported for this framework!");
                Console.WriteLine();
#else
                xz.Extract(outputDirectory, includeDebug);
#endif
            }

            // XZP
            else if (wrapper is XZP xzp)
            {
                Console.WriteLine("Extracting XZP contents");
                Console.WriteLine();

                xzp.Extract(outputDirectory, includeDebug);
            }

            // Everything else
            else
            {
                Console.WriteLine("Not a supported extractable file format, skipping...");
                Console.WriteLine();
                return;
            }
        }

        /// <summary>
        /// Handle IS-CAB archives
        /// </summary>
        private static bool ExtractInstallShieldCabinet(string file, string outputDirectory, bool includeDebug)
        {
            // Get the name of the first cabinet file or header
            var directory = Path.GetDirectoryName(file);
            string noExtension = Path.GetFileNameWithoutExtension(file);

            bool shouldScanCabinet;
            if (directory == null)
            {
                string filenamePattern = noExtension;
                filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);
                bool cabinetHeaderExists = File.Exists(filenamePattern + "1.hdr");
                shouldScanCabinet = cabinetHeaderExists
                    ? file.Equals(filenamePattern + "1.hdr", StringComparison.OrdinalIgnoreCase)
                    : file.Equals(filenamePattern + "1.cab", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                string filenamePattern = Path.Combine(directory, noExtension);
                filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);
                bool cabinetHeaderExists = File.Exists(Path.Combine(directory, filenamePattern + "1.hdr"));
                shouldScanCabinet = cabinetHeaderExists
                    ? file.Equals(Path.Combine(directory, filenamePattern + "1.hdr"), StringComparison.OrdinalIgnoreCase)
                    : file.Equals(Path.Combine(directory, filenamePattern + "1.cab"), StringComparison.OrdinalIgnoreCase);
            }

            // If we have anything but the first file
            if (!shouldScanCabinet)
                return false;

            try
            {
                if (!File.Exists(file))
                    return false;

                var cabfile = UnshieldSharp.InstallShieldCabinet.Open(file);
                if (cabfile?.HeaderList == null)
                    return false;

                for (int i = 0; i < cabfile.HeaderList.FileCount; i++)
                {
                    try
                    {
                        // Check if the file is valid first
                        if (!cabfile.HeaderList.FileIsValid(i))
                            continue;

                        // Ensure directory separators are consistent
                        string filename = cabfile.HeaderList.GetFileName(i) ?? $"BAD_FILENAME{i}";
                        if (Path.DirectorySeparatorChar == '\\')
                            filename = filename.Replace('/', '\\');
                        else if (Path.DirectorySeparatorChar == '/')
                            filename = filename.Replace('\\', '/');

                        // Ensure the full output directory exists
                        filename = Path.Combine(outputDirectory, filename);
                        var directoryName = Path.GetDirectoryName(filename);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        cabfile.FileSave(i, filename);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.Error.WriteLine(ex);
                    }
                }

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
