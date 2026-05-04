using System.IO;
using SabreTools.Data.Models.OperaFS;

namespace SabreTools.Wrappers
{
    public partial class OperaDiscImage : OperaFS
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "3DO / M2 (Opera) Disc Image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public OperaDiscImage(FileSystem model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public OperaDiscImage(FileSystem model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public OperaDiscImage(FileSystem model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public OperaDiscImage(FileSystem model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public OperaDiscImage(FileSystem model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public OperaDiscImage(FileSystem model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a OperaDiscImage filesystem from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the OperaDiscImage filesystem</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A OperaDiscImage filesystem wrapper on success, null on failure</returns>
        public static new OperaDiscImage? Create(byte[]? data, int offset)
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
        /// Create a OperaDiscImage filesystem from a Stream
        /// </summary>
        /// <param name="data">Seekable Stream representing the OperaDiscImage filesystem</param>
        /// <returns>A OperaDiscImage filesystem wrapper on success, null on failure</returns>
        public static new OperaDiscImage? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead || !data.CanSeek)
                return null;

            try
            {
                // Create user data sub-stream
                var userData = new Data.Extensions.CDROMExtensions.UserDataStream(data);

                // Cache the current offset
                long currentOffset = userData.Position;

                // Deserialize just the sub-stream
                var model = new Serialization.Readers.OperaFS().Deserialize(userData);
                if (model is null)
                    return null;

                // Reset stream
                userData.Seek(currentOffset, SeekOrigin.Begin);

                return new OperaDiscImage(model, userData, userData.Position);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
