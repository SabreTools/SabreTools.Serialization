using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.MicrosoftCabinet;
using SabreTools.IO.Compression.MSZIP;
using SabreTools.IO.Extensions;

#pragma warning disable CA1822 // Mark members as static
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
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Wrapper representing the set, null on error</returns>
        private static MicrosoftCabinet? OpenSet(string? filename, bool includeDebug)
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
            if (current?.Header is null)
                return null;

            // Seek to the first part of the cabinet set
            while (current.CabinetPrev is not null)
            {
                // Attempt to open the previous cabinet
                var prev = current.OpenPrevious(filename, includeDebug);
                if (prev?.Header is null)
                    break;

                // Assign previous as new current
                current = prev;
            }

            // Cache the current start of the cabinet set
            var start = current;

            // Read in the cabinet parts sequentially
            while (current.CabinetNext is not null)
            {
                // If the current and next filenames match
                if (Path.GetFileName(filename) == current.CabinetNext)
                    break;

                // Open the next cabinet and try to parse
                var next = current.OpenNext(filename, includeDebug);
                if (next?.Header is null)
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
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        private MicrosoftCabinet? OpenNext(string? filename, bool includeDebug = false)
        {
            // Ignore invalid archives
            if (string.IsNullOrEmpty(filename))
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a next part
            string? next = CabinetNext;
            if (string.IsNullOrEmpty(next))
                return null;

            // Get the full next path
            string? folder = Path.GetDirectoryName(filename);
            if (folder is not null)
                next = Path.Combine(folder, next);

            // Open and return the next cabinet
            try
            {
                var fs = File.Open(next, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Create(fs);
            }
            catch
            {
                if (includeDebug) Console.WriteLine($"Error: Cabinet set part {next} could not be opened!");
                return null;
            }
        }

        /// <summary>
        /// Open the previous archive, if possible
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        private MicrosoftCabinet? OpenPrevious(string? filename, bool includeDebug)
        {
            // Ignore invalid archives
            if (string.IsNullOrEmpty(filename))
                return null;

            // Normalize the filename
            filename = Path.GetFullPath(filename);

            // Get if the cabinet has a previous part
            string? prev = CabinetPrev;
            if (string.IsNullOrEmpty(prev))
                return null;

            // Get the full next path
            string? folder = Path.GetDirectoryName(filename);
            if (folder is not null)
                prev = Path.Combine(folder, prev);

            // Open and return the previous cabinet
            try
            {
                var fs = File.Open(prev, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Create(fs);
            }
            catch
            {
                if (includeDebug) Console.WriteLine($"Error: Cabinet set part {prev} could not be opened!");
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Display warning in debug runs
            if (includeDebug) Console.WriteLine("WARNING: LZX and Quantum compression schemes are not supported so some files may be skipped!");

            MicrosoftCabinet? cabinet;
            if (Filename is not null)
            {
                // Open the full set if possible
                cabinet = OpenSet(Filename, includeDebug);
                if (cabinet is null)
                {
                    if (includeDebug) Console.WriteLine($"Cabinet set could not be opened!");
                    cabinet = this;
                }
                else
                {
                    // If we have anything but the first file, avoid extraction to avoid repeat extracts
                    // TODO: This is going to have to take missing parts into account for MSI support
                    if (Filename != cabinet.Filename)
                    {
                        string firstCabName = Path.GetFileName(cabinet.Filename) ?? string.Empty;
                        if (includeDebug) Console.WriteLine($"Only the first cabinet {firstCabName} will be extracted!");
                        return false;
                    }
                }
            }
            else
            {
                if (includeDebug) Console.WriteLine($"Cabinet set could not be opened!");
                cabinet = this;
            }

            // If the archive is invalid
            if (cabinet.Folders.Length == 0)
                return false;

            return cabinet.ExtractSet(Filename, outputDirectory, includeDebug);
        }

        /// <summary>
        /// Get filtered array of spanned files for a folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Filtered array of files</returns>
        private CFFILE[] GetSpannedFilesArray(string? filename, int folderIndex, bool includeDebug)
        {
            // Loop through the files
            var filterFiles = GetSpannedFiles(filename, folderIndex, includeDebug);
            List<CFFILE> fileList = [];

            // Filtering, add debug output eventually
            for (int i = 0; i < filterFiles.Length; i++)
            {
                var file = filterFiles[i];

                if (file.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT ||
                    file.FolderIndex == FolderIndex.CONTINUED_FROM_PREV)
                {
                    // debug output for inconsistencies would go here
                    continue;
                }

                fileList.Add(file);
            }

            return [.. fileList];
        }

        /// <summary>
        /// Extract the contents of a cabinet set
        /// </summary>
        /// <param name="cabFilename">Filename for one cabinet in the set, if available</param>
        /// <param name="outputDirectory">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        private bool ExtractSet(string? cabFilename, string outputDirectory, bool includeDebug)
        {
            var cabinet = this;
            var currentCabFilename = cabFilename;
            try
            {
                // Loop through the folders
                bool allExtracted = true;
                while (true)
                {
                    // Loop through the current folders
                    for (int f = 0; f < cabinet.Folders.Length; f++)
                    {
                        if (f == 0 && (cabinet.Files[0].FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT
                                       || cabinet.Files[0].FolderIndex == FolderIndex.CONTINUED_FROM_PREV))
                        {
                            continue;
                        }

                        var folder = cabinet.Folders[f];
                        var files = cabinet.GetSpannedFilesArray(currentCabFilename, f, includeDebug);

                        // Ensure folder contains data
                        if (folder.DataCount == 0)
                            return false;
                        if (folder.CabStartOffset <= 0)
                            return false;

                        // Skip unsupported compression types to avoid opening a blank filestream. This can be altered/removed if these types are ever supported.
                        var compressionType = GetCompressionType(folder);
                        if (compressionType == CompressionType.TYPE_QUANTUM || compressionType == CompressionType.TYPE_LZX)
                            continue;

                        var reader = new Reader(cabinet, folder, files);

                        reader.ExtractData(outputDirectory, compressionType, f, includeDebug);
                    }

                    // Move to the next cabinet, if possible
                    cabinet = cabinet.Next;
                    if (cabinet is null) // If the next cabinet is missing, there's no better way to handle this
                        return false;

                    currentCabFilename = cabinet.Filename;

                    if (cabinet.Folders.Length == 0)
                        break;
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
        /// Helper to extract files from a cabinet set
        /// </summary>
        private class Reader
        {
            #region Private Instance Variables

            /// <summary>
            /// Current cabinet file being read from
            /// </summary>
            private MicrosoftCabinet _cabinet;

            /// <summary>
            /// Current folder being read from
            /// </summary>
            private CFFOLDER _folder;

            /// <summary>
            /// Current array of files to be extracted
            /// </summary>
            private readonly CFFILE[] _files;

            /// <summary>
            /// Current number of bytes left to write of the current file
            /// </summary>
            private long _bytesLeft;

            /// <summary>
            /// Current index in the folder of files to extract
            /// </summary>
            private int _fileCounter;

            /// <summary>
            /// Current offset in the cabinet being read from
            /// </summary>
            private long _offset;

            /// <summary>
            /// Current output filestream being written to
            /// </summary>
            private FileStream? _fileStream;

            #endregion

            #region Constructors

            public Reader(MicrosoftCabinet cabinet, CFFOLDER folder, CFFILE[] files)
            {
                _cabinet = cabinet;
                _folder = folder;
                _files = files;
                _bytesLeft = _files[0].FileSize;
                _fileCounter = 0;
                _offset = folder.CabStartOffset;
                _fileStream = null;
            }

            #endregion

            /// <summary>
            /// Get stream representing the output file
            /// </summary>
            /// <param name="filename">Filename for the file that will be extracted to</param>
            /// <param name="outputDirectory">Path to the output directory</param>
            /// <returns>Filestream opened for the file</returns>
            private FileStream GetFileStream(string filename, string outputDirectory)
            {
                // Ensure directory separators are consistent
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outputDirectory, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName is not null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Open the output file for writing
                return File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            }

            /// <summary>
            /// Read a datablock from a cabinet
            /// </summary>
            /// <param name="includeDebug">True to include debug data, false otherwise</param>
            /// <returns>Read datablock</returns>
            private CFDATA? ReadBlock(bool includeDebug)
            {
                try
                {
                    lock (_cabinet._dataSourceLock)
                    {
                        _cabinet._dataSource.SeekIfPossible(_offset, SeekOrigin.Begin);
                        var dataBlock = new CFDATA();

                        var dataReservedSize = _cabinet.Header.DataReservedSize;

                        dataBlock.Checksum = _cabinet._dataSource.ReadUInt32LittleEndian();
                        dataBlock.CompressedSize = _cabinet._dataSource.ReadUInt16LittleEndian();
                        dataBlock.UncompressedSize = _cabinet._dataSource.ReadUInt16LittleEndian();

                        if (dataReservedSize > 0)
                            dataBlock.ReservedData = _cabinet._dataSource.ReadBytes(dataReservedSize);

                        if (dataBlock.CompressedSize > 0)
                            dataBlock.CompressedData = _cabinet._dataSource.ReadBytes(dataBlock.CompressedSize);

                        _offset = _cabinet._dataSource.Position;

                        return dataBlock;
                    }
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                    return null;
                }
            }

            /// <summary>
            /// Extract the data from a folder
            /// </summary>
            /// <param name="outputDirectory">Path to the output directory</param>
            /// <param name="compressionType">Type of compression that the folder uses</param>
            /// <param name="folderIndex">Index of the folder in the cabinet</param>
            /// <param name="includeDebug">True to include debug data, false otherwise</param>
            public void ExtractData(string outputDirectory, CompressionType compressionType, int folderIndex, bool includeDebug)
            {
                var mszip = Decompressor.Create();

                string filename = _files[_fileCounter].Name;
                try
                {
                    _fileStream = GetFileStream(filename, outputDirectory);

                    // Loop through the data blocks
                    // Has to be a while loop instead of a for loop due to cab spanning continue blocks
                    for (int j = 0; j < _folder.DataCount; j++)
                    {
                        var dataBlock = ReadBlock(includeDebug);
                        if (dataBlock is null)
                        {
                            if (includeDebug) Console.Error.WriteLine($"Error extracting file {filename}");
                            return;
                        }

                        // Get the data to be processed
                        byte[] blockData = dataBlock.CompressedData;

                        // If the block is continued, append
                        bool continuedBlock = false;
                        if (dataBlock.UncompressedSize == 0)
                        {
                            if (_cabinet.Next is null)
                                break; // Next cab is missing, continue

                            _cabinet = _cabinet.Next;

                            // CompressionType not updated because there's no way it's possible that it can swap on continued blocks
                            _folder = _cabinet.Folders[0];
                            _offset = _folder.CabStartOffset;
                            var nextBlock = ReadBlock(includeDebug);
                            if (nextBlock is null)
                            {
                                if (includeDebug) Console.Error.WriteLine($"Error extracting file {filename}");
                                return;
                            }

                            byte[] nextData = nextBlock.CompressedData;
                            if (nextData.Length == 0)
                                continue;

                            continuedBlock = true;
                            blockData = [.. blockData, .. nextData];
                            dataBlock.CompressedSize += nextBlock.CompressedSize;
                            dataBlock.UncompressedSize = nextBlock.UncompressedSize;
                        }

                        // Get the uncompressed data block
                        byte[] data = compressionType switch
                        {
                            CompressionType.TYPE_NONE => blockData,
                            CompressionType.TYPE_MSZIP => DecompressMSZIPBlock(folderIndex, mszip, j, dataBlock, blockData, includeDebug),

                            // TODO: Unsupported
                            CompressionType.TYPE_QUANTUM => [], //uint quantumWindowBits = (uint)(((ushort)folder.CompressionType >> 8) & 0x1f);
                            CompressionType.TYPE_LZX => [],

                            // Should be impossible
                            CompressionType.MASK_TYPE => [],
                            _ => [],
                        };

                        WriteData(data, outputDirectory);

                        if (continuedBlock)
                            j = 0;
                    }
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.Error.WriteLine(ex);
                }
            }

            /// <summary>
            /// Write extracted DataBlocks to a file
            /// </summary>
            /// <param name="data">Data to be written to the output file</param>
            /// <param name="outputDirectory">Path to the output directory</param>
            private void WriteData(byte[] data, string outputDirectory)
            {
                // If there are bytes left to write, and more bytes left to write than the length of the current data to be written.
                if (_bytesLeft > 0 && _bytesLeft >= data.Length)
                {
                    if (_fileStream is null)
                        return;

                    _fileStream.Write(data);
                    _bytesLeft -= data.Length;
                }
                else
                {
                    long tempBytesLeft = _bytesLeft;
                    if (_fileStream is null)
                        return;

                    // If there are still bytes left to write, but less bytes than the length of the current data to be written
                    if (_bytesLeft > 0 && _bytesLeft < data.Length)
                        _fileStream.Write(data, 0, (int)_bytesLeft);

                    // Close and iterate file.
                    if (EndFile(outputDirectory))
                        return;

                    // While the file still has bytes that need to be written to it, but less bytes than the input data still has to be written.
                    while (_bytesLeft < data.Length - tempBytesLeft)
                    {
                        _fileStream.Write(data, (int)tempBytesLeft, (int)_bytesLeft);
                        tempBytesLeft += _bytesLeft;
                        if (EndFile(outputDirectory))
                            break;
                    }

                    _fileStream.Write(data, (int)tempBytesLeft, data.Length - (int)tempBytesLeft);
                    _bytesLeft -= data.Length - tempBytesLeft;
                }

                // Top if block occurs on http://redump.org/disc/107833/ , middle on https://dbox.tools/titles/pc/57520FA0 , bottom still unobserved
                // While loop since this also handles 0 byte files. Example file seen in http://redump.org/disc/93312/ , cab Group17.cab, file TRACKSLOC6DYNTEX_BIN
                while (_bytesLeft == 0)
                {
                    if (EndFile(outputDirectory))
                        break;
                }
            }

            /// <summary>
            /// Finish handling a file and progress to the next file as necessary.
            /// </summary>
            /// <param name="outputDirectory">Path to the output directory</param>
            /// <returns>True the end of the folder has been reached, false otherwise</returns>
            private bool EndFile(string outputDirectory)
            {
                if (_fileStream is null)
                    return false;

                _fileStream.Flush();
                _fileStream.Close();

                // reached end of folder
                if (_fileCounter + 1 == _files.Length)
                    return true;

                ++_fileCounter;
                _bytesLeft = (int)_files[_fileCounter].FileSize;
                _fileStream = GetFileStream(_files[_fileCounter].Name, outputDirectory);

                return false;
            }
        }

        #endregion

        #region Checksumming

        /// <summary>
        /// The computation and verification of checksums found in CFDATA structure entries cabinet files is
        /// done by using a function described by the following mathematical notation. When checksums are
        /// not supplied by the cabinet file creating application, the checksum field is set to 0 (zero). Cabinet
        /// extracting applications do not compute or verify the checksum if the field is set to 0 (zero).
        /// </summary>
#pragma warning disable IDE0051
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
#pragma warning restore IDE0051

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
