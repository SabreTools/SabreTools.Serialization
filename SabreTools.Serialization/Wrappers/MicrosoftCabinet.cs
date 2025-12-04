using System;
using System.IO;
using SabreTools.Data.Models.MicrosoftCabinet;
using SabreTools.IO.Compression.MSZIP;

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
        public CFFILE[] Files => Model.Files;

        /// <inheritdoc cref="CFHEADER.FileCount"/>
        public int FileCount => Header.FileCount;

        /// <inheritdoc cref="Cabinet.Folders"/>
        public CFFOLDER[] Folders => Model.Folders;

        /// <inheritdoc cref="CFHEADER.FolderCount"/>
        public int FolderCount => Header.FolderCount;

        /// <inheritdoc cref="Cabinet.Header"/>
        public CFHEADER Header => Model.Header;

        /// <inheritdoc cref="CFHEADER.CabinetNext"/>
        public string? CabinetNext => Header.CabinetNext;

        /// <inheritdoc cref="CFHEADER.CabinetPrev"/>
        public string? CabinetPrev => Header.CabinetPrev;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public MicrosoftCabinet(Cabinet model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

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
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.MicrosoftCabinet().Deserialize(data);
                if (model == null)
                    return null;

                return new MicrosoftCabinet(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
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
                FolderIndex.CONTINUED_TO_NEXT => Header.FolderCount - 1,
                FolderIndex.CONTINUED_PREV_AND_NEXT => 0,
                _ => (int)file.FolderIndex,
            };
        }

        #endregion

        #region Folders

        /// <summary>
        /// Decompress all blocks for a folder
        /// </summary>
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Stream representing the decompressed data on success, null otherwise</returns>
        public Stream? DecompressBlocks(string? filename, CFFOLDER? folder, int folderIndex, bool includeDebug)
        {
            // Ensure data blocks
            var dataBlocks = GetDataBlocks(filename, folder, folderIndex);
            if (dataBlocks == null || dataBlocks.Length == 0)
                return null;

            // Get the compression type
            var compressionType = GetCompressionType(folder!);

            // Setup decompressors
            var mszip = Decompressor.Create();
            //uint quantumWindowBits = (uint)(((ushort)folder.CompressionType >> 8) & 0x1f);

            // Loop through the data blocks
            var ms = new MemoryStream();
            for (int i = 0; i < dataBlocks.Length; i++)
            {
                var db = dataBlocks[i];

                // Get the data to be processed
                byte[] blockData = db.CompressedData;

                // If the block is continued, append
                bool continuedBlock = false;
                if (db.UncompressedSize == 0)
                {
                    var nextBlock = dataBlocks[i + 1];
                    byte[]? nextData = nextBlock.CompressedData;
                    if (nextData == null)
                        continue;

                    continuedBlock = true;
                    blockData = [.. blockData, .. nextData];
                    db.CompressedSize += nextBlock.CompressedSize;
                    db.UncompressedSize = nextBlock.UncompressedSize;
                }

                // Get the uncompressed data block
                byte[] data = compressionType switch
                {
                    CompressionType.TYPE_NONE => blockData,
                    CompressionType.TYPE_MSZIP => DecompressMSZIPBlock(folderIndex, mszip, i, db, blockData, includeDebug),

                    // TODO: Unsupported
                    CompressionType.TYPE_QUANTUM => [],
                    CompressionType.TYPE_LZX => [],

                    // Should be impossible
                    _ => [],
                };

                // Write the uncompressed data block
                ms.Write(data, 0, data.Length);
                ms.Flush();

                // Increment additionally if we had a continued block
                if (continuedBlock) i++;
            }

            return ms;
        }

        /// <summary>
        /// Decompress an MS-ZIP block using an existing decompressor
        /// </summary>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="mszip">MS-ZIP decompressor with persistent state</param>
        /// <param name="blockIndex">Index of the block within the folder</param>
        /// <param name="block">Block data to be used for decompression</param>
        /// <param name="blockData">Block data to be used for decompression</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Byte array representing the decompressed data, empty on error</returns>
        private static byte[] DecompressMSZIPBlock(int folderIndex, Decompressor mszip, int blockIndex, CFDATA block, byte[] blockData, bool includeDebug)
        {
            try
            {
                // Decompress to a temporary stream
                using var stream = new MemoryStream();
                mszip.CopyTo(blockData, stream);

                // Pad to the correct size but throw a warning about this
                if (stream.Length < block.UncompressedSize)
                {
                    if (includeDebug)
                        Console.Error.WriteLine($"Data block {blockIndex} in folder {folderIndex} had mismatching sizes. Expected: {block.UncompressedSize}, Got: {stream.Length}");

                    byte[] padding = new byte[block.UncompressedSize - stream.Length];
                    stream.Write(padding, 0, padding.Length);
                }

                // Return the byte array data
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return [];
            }
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
        /// <param name="filename">Filename for one cabinet in the set, if available</param>
        /// <param name="folder">Folder containing the blocks to decompress</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="skipPrev">Indicates if previous cabinets should be ignored</param>
        /// <param name="skipNext">Indicates if next cabinets should be ignored</param>
        /// <returns>Array of data blocks on success, null otherwise</returns>
        private CFDATA[]? GetDataBlocks(string? filename, CFFOLDER? folder, int folderIndex, bool skipPrev = false, bool skipNext = false)
        {
            // Skip invalid folders
            if (folder?.DataBlocks == null || folder.DataBlocks.Length == 0)
                return null;

            GetData(folder);

            // Get all files for the folder
            var files = GetFiles(filename, folderIndex);
            if (files.Length == 0)
                return folder.DataBlocks;

            // Check if the folder spans in either direction
            bool spanPrev = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);
            bool spanNext = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_TO_NEXT || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);

            // If the folder spans backward and Prev is not being skipped
            CFDATA[] prevBlocks = [];
            if (!skipPrev && spanPrev)
            {
                // Try to get Prev if it doesn't exist
                if (Prev?.Header == null)
                    Prev = OpenPrevious(filename);

                // Get all blocks from Prev
                if (Prev?.Header != null && Prev.Folders != null)
                {
                    int prevFolderIndex = Prev.FolderCount - 1;
                    var prevFolder = Prev.Folders[prevFolderIndex - 1];
                    prevBlocks = Prev.GetDataBlocks(filename, prevFolder, prevFolderIndex, skipNext: true) ?? [];
                }
            }

            // If the folder spans forward and Next is not being skipped
            CFDATA[] nextBlocks = [];
            if (!skipNext && spanNext)
            {
                // Try to get Next if it doesn't exist
                if (Next?.Header == null)
                    Next = OpenNext(filename);

                // Get all blocks from Prev
                if (Next?.Header != null && Next.Folders != null)
                {
                    var nextFolder = Next.Folders[0];
                    nextBlocks = Next.GetDataBlocks(filename, nextFolder, 0, skipPrev: true) ?? [];
                }
            }

            // Return all found blocks in order
            return [.. prevBlocks, .. folder.DataBlocks, .. nextBlocks];
        }
        
        public void GetData(CFFOLDER folder)
        {
            if (folder.CabStartOffset > 0)
            {
                uint offset = folder.CabStartOffset;

                for (int i = 0; i < folder.DataCount; i++)
                {
                    offset += 8;

                    if (Header.DataReservedSize > 0)
                    {
                        folder.DataBlocks[i].ReservedData = ReadRangeFromSource(offset, Header.DataReservedSize);
                        offset += Header.DataReservedSize;
                    }

                    if (folder.DataBlocks[i].CompressedSize > 0)
                    {
                        folder.DataBlocks[i].CompressedData = ReadRangeFromSource(offset, folder.DataBlocks[i].CompressedSize);
                        offset += folder.DataBlocks[i].CompressedSize;
                    }
                }
            }
        }
        
        /// <summary>
        /// Get all files for the current folder index
        /// </summary>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="ignorePrev">True to ignore previous links, false otherwise</param>
        /// <returns>Array of all files for the folder</returns>
        private CFFILE[] GetFiles(string? filename, int folderIndex, bool ignorePrev = false, bool skipPrev = false, bool skipNext = false)
        {
            // Ignore invalid archives
            if (Files == null)
                return [];

            // Get all files with a name and matching index
            var files = Array.FindAll(Files, f =>
            {
                if (string.IsNullOrEmpty(f.Name))
                    return false;

                // Ignore links to previous cabinets, if required
                if (ignorePrev)
                {
                    if (f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV)
                        return false;
                    else if (f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT)
                        return false;
                }

                int fileFolder = GetFolderIndex(f);
                return fileFolder == folderIndex;
            });
            
            // Check if the folder spans in either direction
            bool spanPrev = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);
            bool spanNext = Array.Exists(files, f => f.FolderIndex == FolderIndex.CONTINUED_TO_NEXT || f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT);

            // If the folder spans backward and Prev is not being skipped
            CFFILE[] prevFiles = [];
            if (!skipPrev && spanPrev)
            {
                // Try to get Prev if it doesn't exist
                if (Prev?.Header == null)
                    Prev = OpenPrevious(filename);

                // Get all files from Prev
                if (Prev?.Header != null && Prev.Folders != null)
                {
                    int prevFolderIndex = Prev.FolderCount - 1;
                    prevFiles = Prev.GetFiles(filename, prevFolderIndex, skipNext: true) ?? [];
                }
            }
            
            // If the folder spans forward and Next is not being skipped
            CFFILE[] nextFiles = [];
            if (!skipNext && spanNext)
            {
                // Try to get Next if it doesn't exist
                if (Next?.Header == null)
                    Next = OpenNext(filename);

                // Get all files from Prev
                if (Next?.Header != null && Next.Folders != null)
                {
                    var nextFolder = Next.Folders[0];
                    nextFiles = Next.GetFiles(filename, 0, skipPrev: true) ?? [];
                }
            }
            
            // Return all found files in order
            return [.. prevFiles, .. files, .. nextFiles];
        }

        /// <summary>
        /// Get all files for the current folder index
        /// </summary>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="ignorePrev">True to ignore previous links, false otherwise</param>
        /// <returns>Array of all files for the folder</returns>
        private CFFILE[] GetFilesOld(int folderIndex, bool ignorePrev = false)
        {
            // Ignore invalid archives
            if (Files == null)
                return [];

            // Get all files with a name and matching index
            return Array.FindAll(Files, f =>
            {
                if (string.IsNullOrEmpty(f.Name))
                    return false;

                // Ignore links to previous cabinets, if required
                if (ignorePrev)
                {
                    if (f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV)
                        return false;
                    else if (f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT)
                        return false;
                }

                int fileFolder = GetFolderIndex(f);
                return fileFolder == folderIndex;
            });
        }

        #endregion
    }
}
