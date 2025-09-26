using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.MicrosoftCabinet;

namespace SabreTools.Serialization.Wrappers
{
    public partial class MicrosoftCabinet : IExtractable
    {
        #region Extension Properties

        /// <summary>
        /// Reference to the next cabinet header
        /// </summary>
        /// <remarks>Only used in multi-file</remarks>
        public MicrosoftCabinet? Next { get; set; }

        /// <summary>
        /// Reference to the next previous header
        /// </summary>
        /// <remarks>Only used in multi-file</remarks>
        public MicrosoftCabinet? Prev { get; set; }

        #endregion

        #region Cabinet Set

        /// <summary>
        /// Open a cabinet set for reading, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <returns>Wrapper representing the set, null on error</returns>
        private static MicrosoftCabinet? OpenSet(string? filename)
        {
            // If the file is invalid
            if (string.IsNullOrEmpty(filename))
                return null;
            else if (!File.Exists(filename!))
                return null;

            // Get the full file path and directory
            filename = Path.GetFullPath(filename);

            // Read in the current file and try to parse
            var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var current = Create(stream);
            if (current?.Header == null)
                return null;

            // Seek to the first part of the cabinet set
            while (current.CabinetPrev != null)
            {
                // Attempt to open the previous cabinet
                var prev = current.OpenPrevious(filename);
                if (prev?.Header == null)
                    break;

                // Assign previous as new current
                current = prev;
            }

            // Cache the current start of the cabinet set
            var start = current;

            // Read in the cabinet parts sequentially
            while (current.CabinetNext != null)
            {
                // Open the next cabinet and try to parse
                var next = current.OpenNext(filename);
                if (next?.Header == null)
                    break;

                // Add the next and previous links, resetting current
                next.Prev = current;
                current.Next = next;
                current = next;
            }

            // Return the start of the set
            return start;
        }

        /// <summary>
        /// Open the next archive, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        private MicrosoftCabinet? OpenNext(string? filename)
        {
            // Ignore invalid archives
            if (Header == null || string.IsNullOrEmpty(filename))
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a next part
            string? next = CabinetNext;
            if (string.IsNullOrEmpty(next))
                return null;

            // Get the full next path
            string? folder = Path.GetDirectoryName(filename);
            if (folder != null)
                next = Path.Combine(folder, next);

            // Open and return the next cabinet
            var fs = File.Open(next, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Create(fs);
        }

        /// <summary>
        /// Open the previous archive, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        private MicrosoftCabinet? OpenPrevious(string? filename)
        {
            // Ignore invalid archives
            if (Header == null || string.IsNullOrEmpty(filename))
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a previous part
            string? prev = CabinetPrev;
            if (string.IsNullOrEmpty(prev))
                return null;

            // Get the full next path
            string? folder = Path.GetDirectoryName(filename);
            if (folder != null)
                prev = Path.Combine(folder, prev);

            // Open and return the previous cabinet
            var fs = File.Open(prev, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Create(fs);
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Display warning in debug runs
            if (includeDebug) Console.WriteLine("WARNING: LZX and Quantum compression schemes are not supported so some files may be skipped!");

            // Open the full set if possible
            var cabinet = this;
            if (Filename != null)
                cabinet = OpenSet(Filename);

            // If the archive is invalid
            if (cabinet?.Folders == null || cabinet.Folders.Length == 0)
                return false;

            try
            {
                // Loop through the folders
                bool allExtracted = true;
                for (int f = 0; f < cabinet.Folders.Length; f++)
                {
                    var folder = cabinet.Folders[f];
                    allExtracted &= cabinet.ExtractFolder(Filename, outputDirectory, folder, f, includeDebug);
                }

                return allExtracted;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract the contents of a single folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="outputDirectory">Path to the output directory</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        private bool ExtractFolder(string? filename,
            string outputDirectory,
            CFFOLDER? folder,
            int folderIndex,
            bool includeDebug)
        {
            // Decompress the blocks, if possible
            using var blockStream = DecompressBlocks(filename, folder, folderIndex, includeDebug);
            if (blockStream == null || blockStream.Length == 0)
                return false;

            // Loop through the files
            bool allExtracted = true;
            var files = GetFiles(folderIndex);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                allExtracted &= ExtractFile(outputDirectory, blockStream, file, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract the contents of a single file
        /// </summary>
        /// <param name="outputDirectory">Path to the output directory</param>
        /// <param name="blockStream">Stream representing the uncompressed block data</param>
        /// <param name="file">File information</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        private static bool ExtractFile(string outputDirectory, Stream blockStream, CFFILE file, bool includeDebug)
        {
            try
            {
                blockStream.Seek(file.FolderStartOffset, SeekOrigin.Begin);
                byte[] fileData = blockStream.ReadBytes((int)file.FileSize);

                // Ensure directory separators are consistent
                string filename = file.Name!;
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outputDirectory, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Open the output file for writing
                using var fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                fs.Write(fileData, 0, fileData.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Checksumming

        /// <summary>
        /// The computation and verification of checksums found in CFDATA structure entries cabinet files is
        /// done by using a function described by the following mathematical notation. When checksums are
        /// not supplied by the cabinet file creating application, the checksum field is set to 0 (zero). Cabinet
        /// extracting applications do not compute or verify the checksum if the field is set to 0 (zero).
        /// </summary>
        private static uint ChecksumData(byte[] data)
        {
            uint[] C =
            [
                S(data, 1, data.Length),
                S(data, 2, data.Length),
                S(data, 3, data.Length),
                S(data, 4, data.Length),
            ];

            return C[0] ^ C[1] ^ C[2] ^ C[3];
        }

        /// <summary>
        /// Individual algorithmic step
        /// </summary>
        private static uint S(byte[] a, int b, int x)
        {
            int n = a.Length;

            if (x < 4 && b > n % 4)
                return 0;
            else if (x < 4 && b <= n % 4)
                return a[n - b + 1];
            else // if (x >= 4)
                return a[n - x + b] ^ S(a, b, x - 4);
        }

        #endregion
    }
}
