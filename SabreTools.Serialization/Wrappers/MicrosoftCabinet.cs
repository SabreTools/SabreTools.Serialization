using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.MicrosoftCabinet;

namespace SabreTools.Serialization.Wrappers
{
    public partial class MicrosoftCabinet : WrapperBase<Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Microsoft Cabinet";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Cabinet.Files"/>
        public CFFILE[]? Files => Model.Files;

        /// <inheritdoc cref="CFHEADER.FileCount"/>
        public int FileCount => Model.Header?.FileCount ?? 0;

        /// <inheritdoc cref="Cabinet.Folders"/>
        public CFFOLDER[]? Folders => Model.Folders;

        /// <inheritdoc cref="CFHEADER.FolderCount"/>
        public int FolderCount => Model.Header?.FolderCount ?? 0;

        /// <inheritdoc cref="Cabinet.Header"/>
        public CFHEADER? Header => Model.Header;

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

        #region Constructors

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a Microsoft Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static MicrosoftCabinet? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Microsoft Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static MicrosoftCabinet? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var cabinet = Deserializers.MicrosoftCabinet.DeserializeStream(data);
                if (cabinet == null)
                    return null;

                return new MicrosoftCabinet(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract a cabinet set to an output directory, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Indicates if all files were able to be extracted</returns>
        public static bool ExtractAll(string filename, string outDir, bool includeDebug)
        {
            // Get a wrapper for the set
            var current = OpenSet(filename);
            if (current?.Header == null)
                return false;

            try
            {
                // Loop through the cabinets
                do
                {
                    current.ExtractCabinet(filename, outDir, forwardOnly: true, includeDebug);
                    current = current.Next;
                }
                while (current?.Header != null);
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extract a cabinet file to an output directory, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="forwardOnly">Indicates if the cabinet set should only be read forward</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Indicates if all files were able to be extracted</returns>
        private bool ExtractCabinet(string filename, string outDir, bool forwardOnly, bool includeDebug)
        {
            // If the archive is invalid
            if (Folders == null || Folders.Length == 0)
                return false;

            try
            {
                // Loop through the folders
                for (int f = 0; f < Folders.Length; f++)
                {
                    var folder = Folders[f];
                    ExtractFolder(filename, outDir, folder, f, forwardOnly, includeDebug);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract the contents of a single folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="forwardOnly">Indicates if the cabinet set should only be read forward</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        private void ExtractFolder(string filename, string outDir, CFFOLDER? folder, int folderIndex, bool forwardOnly, bool includeDebug)
        {
            // Decompress the blocks, if possible
            using var blockStream = DecompressBlocks(filename, folder, folderIndex, forwardOnly);
            if (blockStream == null || blockStream.Length == 0)
                return;

            // Ensure files
            var files = GetFiles(folderIndex);
            if (files.Length == 0)
                return;

            // Loop through the files
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    var compressedFile = files[i];
                    blockStream.Seek(compressedFile.FolderStartOffset, SeekOrigin.Begin);
                    byte[] fileData = blockStream.ReadBytes((int)compressedFile.FileSize);

                    // Ensure directory separators are consistent
                    string fileName = compressedFile.Name!;
                    if (Path.DirectorySeparatorChar == '\\')
                        fileName = fileName.Replace('/', '\\');
                    else if (Path.DirectorySeparatorChar == '/')
                        fileName = fileName.Replace('\\', '/');

                    string tempFile = Path.Combine(outDir, fileName);
                    var directoryName = Path.GetDirectoryName(tempFile);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using var of = File.OpenWrite(tempFile);
                    of.Write(fileData, 0, fileData.Length);
                    of.Flush();
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.WriteLine(ex);
                }
            }
        }

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
            while (current.Header.CabinetPrev != null)
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
            while (current.Header.CabinetNext != null)
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
        private MicrosoftCabinet? OpenNext(string filename)
        {
            // Ignore invalid archives
            if (Header == null)
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a next part
            string? next = Header.CabinetNext;
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
        private MicrosoftCabinet? OpenPrevious(string filename)
        {
            // Ignore invalid archives
            if (Header == null)
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a previous part
            string? prev = Header.CabinetPrev;
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

        #region Files

        /// <summary>
        /// Get the DateTime for a particular file index
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <returns>DateTime representing the file time, null on error</returns>
        public DateTime? GetDateTime(int fileIndex)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || Files == null || fileIndex >= Files.Length)
                return null;

            // If we have an invalid DateTime
            var file = Files[fileIndex];
            if (file.Date == 0 && file.Time == 0)
                return null;

            try
            {
                // Date property
                int year = (file.Date >> 9) + 1980;
                int month = (file.Date >> 5) & 0x0F;
                int day = file.Date & 0x1F;

                // Time property
                int hour = file.Time >> 11;
                int minute = (file.Time >> 5) & 0x3F;
                int second = (file.Time << 1) & 0x3E;

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Get the corrected folder index
        /// </summary>
        /// <param name="file">File to get the corrected index for</param>
        /// <returns>Corrected folder index for the current archive</returns>
        private int GetFolderIndex(CFFILE file)
        {
            return file.FolderIndex switch
            {
                FolderIndex.CONTINUED_FROM_PREV => 0,
                FolderIndex.CONTINUED_TO_NEXT => (Header?.FolderCount ?? 1) - 1,
                FolderIndex.CONTINUED_PREV_AND_NEXT => 0,
                _ => (int)file.FolderIndex,
            };
        }

        #endregion

        #region Folders

        /// <summary>
        /// Decompress all blocks for a folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="forwardOnly">Indicates if the cabinet set should only be read forward</param>
        /// <returns>Stream representing the decompressed data on success, null otherwise</returns>
        public Stream? DecompressBlocks(string filename, CFFOLDER? folder, int folderIndex, bool forwardOnly)
        {
            // Ensure data blocks
            var dataBlocks = GetDataBlocks(filename, folder, folderIndex, skipPrev: forwardOnly, skipNext: false);
            if (dataBlocks == null || dataBlocks.Length == 0)
                return null;

            // Setup decompressors
            var mszip = IO.Compression.MSZIP.Decompressor.Create();
            //uint quantumWindowBits = (uint)(((ushort)folder.CompressionType >> 8) & 0x1f);

            // Loop through the data blocks
            var ms = new MemoryStream();
            for (int i = 0; i < dataBlocks.Length; i++)
            {
                var db = dataBlocks[i];
                if (db?.CompressedData == null)
                    continue;

                // Get the compression type
                var compressionType = GetCompressionType(folder!);
                switch (compressionType)
                {
                    // Uncompressed data
                    case CompressionType.TYPE_NONE:
                        ms.Write(db.CompressedData, 0, db.CompressedData.Length);
                        ms.Flush();
                        break;

                    // MS-ZIP
                    case CompressionType.TYPE_MSZIP:
                        long position = ms.Position;
                        mszip.CopyTo(db.CompressedData, ms);
                        long decompressedSize = ms.Position - position;

                        // Pad to the correct size but throw a warning about this
                        if (decompressedSize < db.UncompressedSize)
                        {
                            Console.Error.WriteLine($"Data block {i} in folder {folderIndex} had mismatching sizes. Expected: {db.UncompressedSize}, Got: {decompressedSize}");
                            byte[] padding = new byte[db.UncompressedSize - decompressedSize];
                            ms.Write(padding, 0, padding.Length);
                        }

                        break;

                    // Quantum
                    case CompressionType.TYPE_QUANTUM:
                        // TODO: Unsupported
                        break;

                    // LZX
                    case CompressionType.TYPE_LZX:
                        // TODO: Unsupported
                        break;
                }
            }

            return ms;
        }

        /// <summary>
        /// Get the unmasked compression type for a folder
        /// </summary>
        /// <param name="folder">Folder to get the compression type for</param>
        /// <returns>Compression type on success, <see cref="ushort.MaxValue"/> on error</returns>
        private static CompressionType GetCompressionType(CFFOLDER folder)
        {
            if ((folder!.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_NONE)
                return CompressionType.TYPE_NONE;
            else if ((folder.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_MSZIP)
                return CompressionType.TYPE_MSZIP;
            else if ((folder.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_QUANTUM)
                return CompressionType.TYPE_QUANTUM;
            else if ((folder.CompressionType & CompressionType.MASK_TYPE) == CompressionType.TYPE_LZX)
                return CompressionType.TYPE_LZX;
            else
                return (CompressionType)ushort.MaxValue;
        }

        /// <summary>
        /// Get the set of data blocks for a folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="skipPrev">Indicates if previous cabinets should be ignored</param>
        /// <param name="skipNext">Indicates if next cabinets should be ignored</param>
        /// <returns>Array of data blocks on success, null otherwise</returns>
        private CFDATA[]? GetDataBlocks(string filename, CFFOLDER? folder, int folderIndex, bool skipPrev = false, bool skipNext = false)
        {
            // Skip invalid folders
            if (folder?.DataBlocks == null || folder.DataBlocks.Length == 0)
                return null;

            // Get all files for the folder
            var files = GetFiles(folderIndex);
            if (files.Length == 0)
                return folder.DataBlocks;

            // Check if the folder spans in either direction
            bool spanPrev = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);
            bool spanNext = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_TO_NEXT || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);

            // Check if the folder spans backward
            CFDATA[] prevBlocks = [];
            if (!skipPrev && spanPrev)
            {
                if (Prev?.Header != null && Prev.Folders != null)
                {
                    int prevFolderIndex = Prev.FolderCount;
                    var prevFolder = Prev.Folders[prevFolderIndex - 1];
                    prevBlocks = Prev.GetDataBlocks(filename, prevFolder, prevFolderIndex, skipNext: true) ?? [];
                }
            }

            // Check if the folder spans forward
            CFDATA[] nextBlocks = [];
            if (!skipNext && spanNext)
            {
                if (Next?.Header != null && Next.Folders != null)
                {
                    var nextFolder = Next.Folders[0];
                    nextBlocks = Next.GetDataBlocks(filename, nextFolder, 0, skipPrev: true) ?? [];
                }
            }

            // Return all found blocks in order
            return [.. prevBlocks, .. folder.DataBlocks, .. nextBlocks];
        }

        /// <summary>
        /// Get all files for the current folder index
        /// </summary>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <returns>Array of all files for the folder</returns>
        private CFFILE[] GetFiles(int folderIndex)
        {
            // Ignore invalid archives
            if (Files == null)
                return [];

            // Get all files with a name and matching index
            return Array.FindAll(Files, f =>
            {
                if (string.IsNullOrEmpty(f.Name))
                    return false;

                int fileFolder = GetFolderIndex(f);
                return fileFolder == folderIndex;
            });
        }

        #endregion
    }
}
