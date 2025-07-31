using System;
using System.Collections.Generic;
using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class GCF : WrapperBase<Models.GCF.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Game Cache File (GCF)";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Set of all files and their information
        /// </summary>
        public FileInfo[]? Files
        {
            get
            {
                // Use the cached value if we have it
                if (_files != null)
                    return _files;

                // If we don't have a required property
                if (Model.DirectoryEntries == null || Model.DirectoryMapEntries == null || Model.BlockEntries == null)
                    return null;

                // Otherwise, scan and build the files
                var files = new List<FileInfo>();
                for (int i = 0; i < Model.DirectoryEntries.Length; i++)
                {
                    // Get the directory entry
                    var directoryEntry = Model.DirectoryEntries[i];
                    var directoryMapEntry = Model.DirectoryMapEntries[i];

                    // If we have a directory, skip for now
#if NET20 || NET35
                    if ((directoryEntry.DirectoryFlags & Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_FILE) == 0)
#else
                    if (!directoryEntry.DirectoryFlags.HasFlag(Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_FILE))
#endif
                        continue;

                    // Otherwise, start building the file info
                    var fileInfo = new FileInfo()
                    {
                        Size = directoryEntry.ItemSize,
#if NET20 || NET35
                        Encrypted = (directoryEntry.DirectoryFlags & Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_ENCRYPTED) != 0,
#else
                        Encrypted = directoryEntry.DirectoryFlags.HasFlag(Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_ENCRYPTED),
#endif
                    };
                    var pathParts = new List<string> { Model.DirectoryNames![directoryEntry.NameOffset] ?? string.Empty };
                    var blockEntries = new List<Models.GCF.BlockEntry>();

                    // Traverse the parent tree
                    uint index = directoryEntry.ParentIndex;
                    while (index != 0xFFFFFFFF)
                    {
                        var parentDirectoryEntry = Model.DirectoryEntries[index];
                        pathParts.Add(Model.DirectoryNames![parentDirectoryEntry.NameOffset] ?? string.Empty);
                        index = parentDirectoryEntry.ParentIndex;
                    }

                    // Traverse the block entries
                    index = directoryMapEntry.FirstBlockIndex;
                    while (index != Model.DataBlockHeader?.BlockCount)
                    {
                        var nextBlock = Model.BlockEntries[index];
                        blockEntries.Add(nextBlock);
                        index = nextBlock.NextBlockEntryIndex;
                    }

                    // Reverse the path parts because of traversal
                    pathParts.Reverse();

                    // Build the remaining file info
#if NET20 || NET35
                    string[] pathArray = [.. pathParts];

                    string tempPath = string.Empty;
                    if (pathArray.Length == 0 || pathArray.Length == 1)
                    {
                        tempPath = pathArray[0];
                    }
                    else
                    {
                        for (int j = 0; j < pathArray.Length; j++)
                        {
                            if (j == 0)
                                tempPath = pathArray[j];
                            else
                                tempPath = Path.Combine(tempPath, pathArray[j]);
                        }
                    }
                    fileInfo.Path = tempPath;
#else
                    fileInfo.Path = Path.Combine([.. pathParts]);
#endif
                    fileInfo.BlockEntries = [.. blockEntries];

                    // Add the file info and continue
                    files.Add(fileInfo);
                }

                // Set and return the file infos
                _files = [.. files];
                return _files;
            }
        }

        /// <summary>
        /// Set of all data block offsets
        /// </summary>
        public long[]? DataBlockOffsets
        {
            get
            {
                // Use the cached value if we have it
                if (_dataBlockOffsets != null)
                    return _dataBlockOffsets;

                // If we don't have a block count, offset, or size
                if (Model.DataBlockHeader?.BlockCount == null || Model.DataBlockHeader?.FirstBlockOffset == null || Model.DataBlockHeader?.BlockSize == null)
                    return null;

                // Otherwise, build the data block set
                _dataBlockOffsets = new long[Model.DataBlockHeader.BlockCount];
                for (int i = 0; i < Model.DataBlockHeader.BlockCount; i++)
                {
                    long dataBlockOffset = Model.DataBlockHeader.FirstBlockOffset + (i * Model.DataBlockHeader.BlockSize);
                    _dataBlockOffsets[i] = dataBlockOffset;
                }

                // Return the set of data blocks
                return _dataBlockOffsets;
            }
        }

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockSize"/>
        public uint BlockSize => Model.DataBlockHeader?.BlockSize ?? 0;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Set of all files and their information
        /// </summary>
        private FileInfo[]? _files = null;

        /// <summary>
        /// Set of all data block offsets
        /// </summary>
        private long[]? _dataBlockOffsets = null;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GCF(Models.GCF.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public GCF(Models.GCF.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an GCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the GCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An GCF wrapper on success, null on failure</returns>
        public static GCF? Create(byte[]? data, int offset)
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
        /// Create a GCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the GCF</param>
        /// <returns>An GCF wrapper on success, null on failure</returns>
        public static GCF? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var file = Deserializers.GCF.DeserializeStream(data);
                if (file == null)
                    return null;

                return new GCF(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the GCF to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Files.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the GCF to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (Files == null || Files.Length == 0 || DataBlockOffsets == null)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the file
            var file = Files[index];
            if (file?.BlockEntries == null || file.Size == 0)
                return false;

            // If the file is encrypted -- TODO: Revisit later
            if (file.Encrypted)
                return false;

            // Get all data block offsets needed for extraction
            var dataBlockOffsets = new List<long>();
            for (int i = 0; i < file.BlockEntries.Length; i++)
            {
                var blockEntry = file.BlockEntries[i];

                uint dataBlockIndex = blockEntry.FirstDataBlockIndex;
                long blockEntrySize = blockEntry.FileDataSize;
                while (blockEntrySize > 0)
                {
                    long dataBlockOffset = DataBlockOffsets[dataBlockIndex++];
                    dataBlockOffsets.Add(dataBlockOffset);
                    blockEntrySize -= BlockSize;
                }
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = file.Path ?? $"file{index}";
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);

                // Now read the data sequentially and write out while we have data left
                long fileSize = file.Size;
                for (int i = 0; i < dataBlockOffsets.Count; i++)
                {
                    int readSize = (int)Math.Min(BlockSize, fileSize);
                    var data = ReadFromDataSource((int)dataBlockOffsets[i], readSize);
                    if (data == null)
                        return false;

                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Class to contain all necessary file information
        /// </summary>
        public sealed class FileInfo
        {
            /// <summary>
            /// Full item path
            /// </summary>
            public string? Path;

            /// <summary>
            /// File size
            /// </summary>
            public uint Size;

            /// <summary>
            /// Indicates if the block is encrypted
            /// </summary>
            public bool Encrypted;

            /// <summary>
            /// Array of block entries
            /// </summary>
            public Models.GCF.BlockEntry[]? BlockEntries;
        }

        #endregion
    }
}
