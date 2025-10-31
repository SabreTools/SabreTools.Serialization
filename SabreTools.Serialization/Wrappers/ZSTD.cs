using System.IO;
using SabreTools.Data.Models.ZSTD;

namespace SabreTools.Serialization.Wrappers
{
    public partial class ZSTD : WrapperBase<Header>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "ZSTD file";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header.VersionByte"/>
        public byte VersionByte => Model.VersionByte;

        /// <inheritdoc cref="Header.Magic"/>
        public byte[] Magic => Model.Magic;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public ZSTD(Data.Models.ZSTD.Header model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public ZSTD(Data.Models.ZSTD.Header model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public ZSTD(Data.Models.ZSTD.Header model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public ZSTD(Data.Models.ZSTD.Header model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public ZSTD(Data.Models.ZSTD.Header model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public ZSTD(Data.Models.ZSTD.Header model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a ZSTD file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the ZSTD file</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A ZSTD wrapper on success, null on failure</returns>
        public static ZSTD? Create(byte[]? data, int offset)
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
        /// Create a ZSTD file from a Stream
        /// </summary>
        /// <param name="data">Stream representing the ZSTD file </param>
        /// <returns>A ZSTD wrapper on success, null on failure</returns>
        public static ZSTD? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.ZSTD().Deserialize(data);
                if (model == null)
                    return null;

                return new ZSTD(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
