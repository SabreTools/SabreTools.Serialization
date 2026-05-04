using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.OperaFS;

namespace SabreTools.Wrappers
{
    public partial class OperaFS : WrapperBase<FileSystem>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "3DO / M2 (Opera) Filesystem";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="FileSystem.VolumeDescriptor"/>
        public VolumeDescriptor VolumeDescriptor => Model.VolumeDescriptor;

        /// <inheritdoc cref="FileSystem.Directories"/>
        public Dictionary<uint, DirectoryDescriptor> Directories => Model.Directories;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public OperaFS(FileSystem model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public OperaFS(FileSystem model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public OperaFS(FileSystem model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public OperaFS(FileSystem model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public OperaFS(FileSystem model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public OperaFS(FileSystem model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an OperaFS FileSystem from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the OperaFS FileSystem</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An OperaFS FileSystem wrapper on success, null on failure</returns>
        public static OperaFS? Create(byte[]? data, int offset)
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
        /// Create an OperaFS FileSystem from a Stream
        /// </summary>
        /// <param name="data">Stream representing the OperaFS FileSystem</param>
        /// <returns>An OperaFS FileSystem wrapper on success, null on failure</returns>
        public static OperaFS? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.OperaFS().Deserialize(data);
                if (model is null)
                    return null;

                return new OperaFS(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
