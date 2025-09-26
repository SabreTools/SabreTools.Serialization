using System.IO;
using SabreTools.Serialization.Models.TAR;

namespace SabreTools.Serialization.Wrappers
{
    public partial class TapeArchive : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Tape Archive (or Derived Format)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Entries"/>
        public Entry[]? Entries => Model.Entries;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public TapeArchive(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public TapeArchive(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public TapeArchive(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public TapeArchive(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public TapeArchive(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public TapeArchive(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a tape archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A tape archive wrapper on success, null on failure</returns>
        public static TapeArchive? Create(byte[]? data, int offset)
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
        /// Create a tape archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A tape archive wrapper on success, null on failure</returns>
        public static TapeArchive? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.TapeArchive().Deserialize(data);
                if (model == null)
                    return null;

                return new TapeArchive(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
