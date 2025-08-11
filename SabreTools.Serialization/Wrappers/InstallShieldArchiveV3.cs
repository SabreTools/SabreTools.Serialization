using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Compression.Blast;
using SabreTools.Models.InstallShieldArchiveV3;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    /// <remarks>
    /// Reference (de)compressor: https://www.sac.sk/download/pack/icomp95.zip
    /// </remarks>
    /// <see href="https://github.com/wfr/unshieldv3"/>
    public partial class InstallShieldArchiveV3 : WrapperBase<Archive>, IExtractable
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Archive V3";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header.DirCount"/>
        public ushort DirCount => Model.Header?.DirCount ?? 0;

        /// <inheritdoc cref="Header.FileCount"/>
        public ushort FileCount => Model.Header?.FileCount ?? 0;

        /// <inheritdoc cref="Archive.Directories"/>
        public Models.InstallShieldArchiveV3.Directory[] Directories => Model.Directories ?? [];

        /// <inheritdoc cref="Archive.Files"/>
        public Models.InstallShieldArchiveV3.File[] Files => Model.Files ?? [];

        /// <summary>
        /// Map of all files to their parent directories by index
        /// </summary>
        public Dictionary<int, int> FileDirMap
        {
            get
            {
                // Return the prebuilt map
                if (_fileDirMap != null)
                    return _fileDirMap;

                // Build the file map
                _fileDirMap = [];

                int fileId = 0;
                for (int i = 0; i < Directories.Length; i++)
                {
                    var dir = Directories[i];
                    for (int j = 0; j < dir.FileCount; j++)
                    {
                        _fileDirMap[fileId++] = i;
                    }
                }

                return _fileDirMap;
            }
        }
        private Dictionary<int, int>? _fileDirMap = null;

        /// <summary>
        /// Map of all files found in the archive
        /// </summary>
        public Dictionary<string, Models.InstallShieldArchiveV3.File> FileNameMap
        {
            get
            {
                // Return the prebuilt map
                if (_fileNameMap != null)
                    return _fileNameMap;

                // Build the file map
                _fileNameMap = [];
                for (int fileIndex = 0; fileIndex < Files.Length; fileIndex++)
                {
                    // Get the current file
                    var file = Files[fileIndex];

                    // Get the parent directory
                    int dirIndex = FileDirMap[fileIndex];
                    if (dirIndex < 0 || dirIndex >= DirCount)
                        continue;

                    // Create the filename
                    string filename = Path.Combine(
                        Directories[dirIndex]?.Name ?? $"dir_{dirIndex}",
                        file.Name ?? $"file_{fileIndex}"
                    );

                    // Add to the map
                    _fileNameMap[filename] = file;
                }

                return _fileNameMap;
            }
        }
        private Dictionary<string, Models.InstallShieldArchiveV3.File>? _fileNameMap = null;

        /// <summary>
        /// Data offset for all archives
        /// </summary>
        private const uint DataStart = 255;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public InstallShieldArchiveV3(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public InstallShieldArchiveV3(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an InstallShield Archive V3 from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A archive wrapper on success, null on failure</returns>
        public static InstallShieldArchiveV3? Create(byte[]? data, int offset)
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
        /// Create a InstallShield Archive V3 from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A archive wrapper on success, null on failure</returns>
        public static InstallShieldArchiveV3? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var archive = Deserializers.InstallShieldArchiveV3.DeserializeStream(data);
                if (archive == null)
                    return null;

                return new InstallShieldArchiveV3(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            // Get the file count
            int fileCount = Files.Length;
            if (fileCount == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < fileCount; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the ISAv3 to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If the files index is invalid
            if (index < 0 || index >= FileCount)
                return false;

            // Get the file
            var file = Files[index];
            if (file == null)
                return false;

            // Create the filename
            var filename = file.Name;
            if (filename == null)
                return false;

            // Get the directory index
            int dirIndex = FileDirMap[index];
            if (dirIndex < 0 || dirIndex > DirCount)
                return false;

            // Get the directory name
            var dirName = Directories[dirIndex].Name;
            if (dirName != null)
                filename = Path.Combine(dirName, filename);

            // Get and adjust the file offset
            long fileOffset = file.Offset + DataStart;
            if (fileOffset < 0 || fileOffset >= Length)
                return false;

            // Get the file sizes
            long fileSize = file.CompressedSize;
            long outputFileSize = file.UncompressedSize;

            // Read the compressed data directly
            var compressedData = ReadFromDataSource((int)fileOffset, (int)fileSize);
            if (compressedData == null)
                return false;

            // If the compressed and uncompressed sizes match
            byte[] data;
            if (fileSize == outputFileSize)
            {
                data = compressedData;
            }
            else
            {
                // Decompress the data
                var decomp = Decompressor.Create();
                using var outData = new MemoryStream();
                decomp.CopyTo(compressedData, outData);
                data = outData.ToArray();
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !System.IO.Directory.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = System.IO.File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return false;
        }

        #endregion
    }
}
