using System.IO;
using SabreTools.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZQBasic : WrapperBase<QBasicFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, QBasic variant";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LZQBasic(QBasicFile model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public LZQBasic(QBasicFile model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public LZQBasic(QBasicFile model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public LZQBasic(QBasicFile model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public LZQBasic(QBasicFile model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public LZQBasic(QBasicFile model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an LZ (QBasic variant) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the LZ (QBasic variant)</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LZ (QBasic variant) wrapper on success, null on failure</returns>
        public static LZQBasic? Create(byte[]? data, int offset)
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
        /// Create a LZ (QBasic variant) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the LZ (QBasic variant)</param>
        /// <returns>An LZ (QBasic variant) wrapper on success, null on failure</returns>
        public static LZQBasic? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.LZQBasic.DeserializeStream(data);
                if (model == null)
                    return null;

                return new LZQBasic(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
