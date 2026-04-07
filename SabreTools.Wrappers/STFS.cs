using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.STFS;

namespace SabreTools.Wrappers
{
    public partial class STFS : WrapperBase<Volume>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Secure Transacted File System";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Volume.Header"/>
        public Header Header => Model.Header;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public STFS(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public STFS(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public STFS(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public STFS(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public STFS(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public STFS(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an STFS Volume from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the STFS Volume</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An STFS Volume wrapper on success, null on failure</returns>
        public static STFS? Create(byte[]? data, int offset)
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
        /// Create an STFS Volume from a Stream
        /// </summary>
        /// <param name="data">Stream representing the STFS Volume</param>
        /// <returns>An STFS Volume wrapper on success, null on failure</returns>
        public static STFS? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.STFS().Deserialize(data);
                if (model is null)
                    return null;

                return new STFS(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
