using System.IO;
using SabreTools.Data.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZKWAJ : WrapperBase<KWAJFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, KWAJ variant";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="KWAJHeader.CompressionType"/>
        public KWAJCompressionType CompressionType => Model.Header?.CompressionType ?? KWAJCompressionType.NoCompression;

        /// <inheritdoc cref="KWAJHeader.DataOffset"/>
        public ushort DataOffset => Model.Header?.DataOffset ?? 0;

        /// <inheritdoc cref="KWAJHeaderExtensions.FileName"/>
        public string? FileName => Model.HeaderExtensions?.FileName;

        /// <inheritdoc cref="KWAJHeaderExtensions.FileExtension"/>
        public string? FileExtension => Model.HeaderExtensions?.FileExtension;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public LZKWAJ(KWAJFile model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an LZ (KWAJ variant) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the LZ (KWAJ variant)</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LZ (KWAJ variant) wrapper on success, null on failure</returns>
        public static LZKWAJ? Create(byte[]? data, int offset)
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
        /// Create a LZ (KWAJ variant) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the LZ (KWAJ variant)</param>
        /// <returns>An LZ (KWAJ variant) wrapper on success, null on failure</returns>
        public static LZKWAJ? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.LZKWAJ().Deserialize(data);
                if (model == null)
                    return null;

                return new LZKWAJ(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
