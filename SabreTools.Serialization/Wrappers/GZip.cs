using System.IO;
using SabreTools.Data.Models.GZIP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class GZip : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "gzip Archive";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Offset to the compressed data
        /// </summary>
        /// <remarks>Returns -1 on error</remarks>
        public long DataOffset
        {
            get
            {
                if (_dataOffset > -1)
                    return _dataOffset;

                if (Header == null)
                    return -1;

                // Minimum offset is 10 bytes:
                // - ID1 (1)
                // - ID2 (1)
                // - CompressionMethod (1)
                // - Flags (1)
                // - LastModifiedTime (4)
                // - ExtraFlags (1)
                // - OperatingSystem (1)
                _dataOffset = 10;

                // Add extra lengths
                _dataOffset += Header.ExtraLength;
                if (Header.OriginalFileName != null)
                    _dataOffset += Header.OriginalFileName.Length + 1;
                if (Header.FileComment != null)
                    _dataOffset += Header.FileComment.Length + 1;
                if (Header.CRC16 != null)
                    _dataOffset += 2;

                return _dataOffset;
            }
        }

        /// <inheritdoc cref="Archive.Header"/>
        public Header? Header => Model.Header;

        /// <inheritdoc cref="Archive.Trailer"/>
        public Trailer? Trailer => Model.Trailer;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Offset to the compressed data
        /// </summary>
        private long _dataOffset = -1;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GZip(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public GZip(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GZip(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public GZip(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public GZip(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GZip(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a GZip archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A GZip wrapper on success, null on failure</returns>
        public static GZip? Create(byte[]? data, int offset)
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
        /// Create a GZip archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A GZip wrapper on success, null on failure</returns>
        public static GZip? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.GZip().Deserialize(data);
                if (model == null)
                    return null;

                return new GZip(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
