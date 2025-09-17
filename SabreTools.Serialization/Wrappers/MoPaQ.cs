using System.IO;
using SabreTools.Models.MoPaQ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class MoPaQ : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "MoPaQ Archive";

        #endregion

        #region No-Model Constructors

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(byte[] data) : base(new Archive(), data) { }

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(byte[] data, int offset) : base(new Archive(), data, offset) { }

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(byte[] data, int offset, int length) : base(new Archive(), data, offset, length) { }

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(Stream data) : base(new Archive(), data) { }

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(Stream data, long offset) : base(new Archive(), data, offset) { }

        /// <inheritdoc/>
        /// <remarks>This should only be used for until MPQ parsing is fixed</remarks>
        public MoPaQ(Stream data, long offset, long length) : base(new Archive(), data, offset, length) { }

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public MoPaQ(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public MoPaQ(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public MoPaQ(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public MoPaQ(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public MoPaQ(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public MoPaQ(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a MoPaQ archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A MoPaQ archive wrapper on success, null on failure</returns>
        public static MoPaQ? Create(byte[]? data, int offset)
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
        /// Create a MoPaQ archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A MoPaQ archive wrapper on success, null on failure</returns>
        public static MoPaQ? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.MoPaQ.DeserializeStream(data);
                if (model == null)
                    return null;

                return new MoPaQ(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
