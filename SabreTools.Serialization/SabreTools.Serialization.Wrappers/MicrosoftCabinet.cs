using System;
using System.IO;
using SabreTools.Data.Models.MicrosoftCabinet;
using SabreTools.IO.Compression.MSZIP;
using SabreTools.IO.Extensions;

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
            if (data is null || data.Length == 0)
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
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.MicrosoftCabinet().Deserialize(data);
                if (model is null)
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
            if (fileIndex < 0 || Files is null || fileIndex >= Files.Length)
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

                FolderIndex.FIRST_FOLDER => (int)file.FolderIndex,
                _ => (int)file.FolderIndex,
            };
        }

        #endregion

        #region Folders

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
        /// Get all files for the current folder, plus connected spanned folders.
        /// </summary>
        /// <param name="filename">Input filename of the cabinet to read from</param>
        /// <param name="folderIndex">Index of the folder in the cabinet</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="skipPrev">True if previous cabinets should be skipped, false otherwise.</param>
        /// <param name="skipNext">True if next cabinets should be skipped, false otherwise.</param>
        /// <returns>Array of all files for the folder</returns>
        private CFFILE[] GetSpannedFiles(string? filename,
            int folderIndex,
            bool includeDebug,
            bool skipPrev = false,
            bool skipNext = false)
        {
            // Ignore invalid archives
            if (Files.IsNullOrEmpty())
                return [];

            // Get all files with a name and matching index
            var files = Array.FindAll(Files, f =>
            {
                if (string.IsNullOrEmpty(f.Name))
                    return false;

                // Ignore links to previous cabinets
                if (f.FolderIndex == FolderIndex.CONTINUED_FROM_PREV)
                    return false;
                else if (f.FolderIndex == FolderIndex.CONTINUED_PREV_AND_NEXT)
                    return false;

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
                if (Prev?.Header is null)
                    Prev = OpenPrevious(filename, includeDebug);

                // Get all files from Prev
                if (Prev?.Header is not null && Prev.Folders is not null)
                {
                    int prevFolderIndex = Prev.FolderCount - 1;
                    prevFiles = Prev.GetSpannedFiles(filename, prevFolderIndex, includeDebug, skipNext: true) ?? [];
                }
            }

            // If the folder spans forward and Next is not being skipped
            CFFILE[] nextFiles = [];
            if (!skipNext && spanNext)
            {
                // Try to get Next if it doesn't exist
                if (Next?.Header is null)
                    Next = OpenNext(filename);

                // Get all files from Prev
                if (Next?.Header is not null && Next.Folders is not null)
                {
                    var nextFolder = Next.Folders[0];
                    nextFiles = Next.GetSpannedFiles(filename, 0, includeDebug, skipPrev: true) ?? [];
                }
            }

            // Return all found files in order
            return [.. prevFiles, .. files, .. nextFiles];
        }

        #endregion
    }
}
