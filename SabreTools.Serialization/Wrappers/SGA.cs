using System;
using System.IO;
using SabreTools.Models.SGA;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SGA : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "SGA";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Directory data
        /// </summary>
        public Models.SGA.Directory? Directory => Model.Directory;

        /// <summary>
        /// Number of files in the directory
        /// </summary>
        public int FileCount
        {
            get
            {
                return Directory switch
                {
                    Directory4 d4 => d4.Files?.Length ?? 0,
                    Directory5 d5 => d5.Files?.Length ?? 0,
                    Directory6 d6 => d6.Files?.Length ?? 0,
                    Directory7 d7 => d7.Files?.Length ?? 0,
                    _ => 0,
                };
            }
        }

        /// <summary>
        /// Offset to the file data
        /// </summary>
        public long FileDataOffset
        {
            get
            {
                return Model.Header switch
                {
                    Header4 h4 => h4.FileDataOffset,
                    Header6 h6 => h6.FileDataOffset,
                    _ => -1,
                };
            }
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public SGA(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public SGA(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SGA(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public SGA(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public SGA(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SGA(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an SGA from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the SGA</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
        public static SGA? Create(byte[]? data, int offset)
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
        /// Create a SGA from a Stream
        /// </summary>
        /// <param name="data">Stream representing the SGA</param>
        /// <returns>An SGA wrapper on success, null on failure</returns>
        public static SGA? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.SGA().Deserialize(data);
                if (model == null)
                    return null;

                return new SGA(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region File

        /// <summary>
        /// Get the compressed size of a file
        /// </summary>
        public long GetCompressedSize(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return -1;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.Size ?? -1L;
        }

        /// <summary>
        /// Get the uncompressed size of a file
        /// </summary>
        public long GetUncompressedSize(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return -1;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.SizeOnDisk ?? -1L;
        }

        /// <summary>
        /// Get a file header from the archive
        /// </summary>
        public Models.SGA.File? GetFile(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return null;

            return Directory switch
            {
                Directory4 d4 => d4.Files![index],
                Directory5 d5 => d5.Files![index],
                Directory6 d6 => d6.Files![index],
                Directory7 d7 => d7.Files![index],
                _ => null,
            };
        }

        /// <summary>
        /// Get a file name from the archive
        /// </summary>
        public string? GetFileName(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return null;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.Name;
        }

        /// <summary>
        /// Get a file offset from the archive
        /// </summary>
        public long GetFileOffset(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return -1;

            // Get the file and return the name
            var file = GetFile(index);
            return file?.Offset ?? -1L;
        }

        /// <summary>
        /// Get the parent name for a file
        /// </summary>
        public string? GetParentName(int index)
        {
            // If the index is invalid
            if (index < 0 || index >= FileCount)
                return null;

            // Get the folder
            Folder? folder = Directory switch
            {
                Directory4 d4 => Array.Find(d4.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                Directory5 d5 => Array.Find(d5.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                Directory6 d6 => Array.Find(d6.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                Directory7 d7 => Array.Find(d7.Folders ?? [], f => f != null && index >= f.FileStartIndex && index <= f.FileEndIndex),
                _ => default,
            };

            // Get the folder name
            return folder switch
            {
                Folder4 f4 => f4.Name,
                Folder5 f5 => f5.Name,
                _ => null,
            };
        }

        #endregion
    }
}
