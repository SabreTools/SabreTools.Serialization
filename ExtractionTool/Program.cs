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

            // Print the preamble
            Console.WriteLine($"Attempting to extract from '{wrapper?.Description() ?? "UNKNOWN"}'");
            Console.WriteLine();

            switch (wrapper)
            {
                // 7-zip
                case SevenZip sz:
                    sz.Extract(outputDirectory, includeDebug);
                    break;

                // BFPK archive
                case BFPK bfpk:
                    bfpk.Extract(outputDirectory, includeDebug);
                    break;

                // BSP
                case BSP bsp:
                    bsp.Extract(outputDirectory, includeDebug);
                    break;

                // bzip2
                case BZip2 bzip2:
                    bzip2.Extract(outputDirectory, includeDebug);
                    break;

                // CFB
                case CFB cfb:
                    cfb.Extract(outputDirectory, includeDebug);
                    break;

                // GCF
                case GCF gcf:
                    gcf.Extract(outputDirectory, includeDebug);
                    break;

                // gzip
                case GZip gzip:
                    gzip.Extract(outputDirectory, includeDebug);
                    break;

                // InstallShield Archive V3 (Z)
                case InstallShieldArchiveV3 isv3:
                    isv3.Extract(outputDirectory, includeDebug);
                    break;

                // IS-CAB archive
                case InstallShieldCabinet:
                    // TODO: Move this handling to Serialization directly
                    ExtractInstallShieldCabinet(file, outputDirectory, includeDebug);
                    break;

                // LZ-compressed file, KWAJ variant
                case LZKWAJ kwaj:
                    kwaj.Extract(outputDirectory, includeDebug);
                    break;

                // LZ-compressed file, QBasic variant
                case LZQBasic qbasic:
                    qbasic.Extract(outputDirectory, includeDebug);
                    break;

                // LZ-compressed file, SZDD variant
                case LZSZDD szdd:
                    szdd.Extract(outputDirectory, includeDebug);
                    break;

                // Microsoft Cabinet archive
                case MicrosoftCabinet mscab:
                    mscab.Extract(outputDirectory, includeDebug);
                    break;

                // MoPaQ (MPQ) archive
                case MoPaQ mpq:
                    mpq.Extract(outputDirectory, includeDebug);
                    break;

                // New Executable
                case NewExecutable nex:
                    nex.Extract(outputDirectory, includeDebug);
                    break;

                // PAK
                case PAK pak:
                    pak.Extract(outputDirectory, includeDebug);
                    break;

                // PFF
                case PFF pff:
                    pff.Extract(outputDirectory, includeDebug);
                    break;

                // PKZIP
                case PKZIP pkzip:
                    pkzip.Extract(outputDirectory, includeDebug);
                    break;

                // Portable Executable
                case PortableExecutable pex:
                    pex.Extract(outputDirectory, includeDebug);
                    break;

                // Quantum
                case Quantum quantum:
                    quantum.Extract(outputDirectory, includeDebug);
                    break;

                // RAR
                case RAR rar:
                    rar.Extract(outputDirectory, includeDebug);
                    break;

                // SGA
                case SGA sga:
                    sga.Extract(outputDirectory, includeDebug);
                    break;

                // Tape Archive
                case TapeArchive tar:
                    tar.Extract(outputDirectory, includeDebug);
                    break;

                // VBSP
                case VBSP vbsp:
                    vbsp.Extract(outputDirectory, includeDebug);
                    break;

                // VPK
                case VPK vpk:
                    vpk.Extract(outputDirectory, includeDebug);
                    break;

                // WAD3
                case WAD3 wad:
                    wad.Extract(outputDirectory, includeDebug);
                    break;

                // xz
                case XZ xz:
                    xz.Extract(outputDirectory, includeDebug);
                    break;

                // XZP
                case XZP xzp:
                    xzp.Extract(outputDirectory, includeDebug);
                    break;

                // Everything else
                default:
                    Console.WriteLine("Not a supported extractable file format, skipping...");
                    Console.WriteLine();
                    break;
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

                var cabfile = UnshieldSharpInternal.InstallShieldCabinet.Open(file);
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
