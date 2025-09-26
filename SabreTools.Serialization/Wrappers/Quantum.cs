using System.IO;
using SabreTools.Data.Models.Quantum;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Quantum : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Quantum Archive";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.CompressedDataOffset"/>
        public long CompressedDataOffset => Model.CompressedDataOffset;

        /// <inheritdoc cref="Header.FileCount"/>
        public ushort FileCount => Header?.FileCount ?? 0;

        /// <inheritdoc cref="Archive.FileList"/>
        public FileDescriptor[] FileList => Model.FileList ?? [];

        /// <inheritdoc cref="Archive.Header"/>
        public Header? Header => Model.Header;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Quantum(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public Quantum(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Quantum(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public Quantum(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public Quantum(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Quantum(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a Quantum archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum? Create(byte[]? data, int offset)
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
        /// Create a Quantum archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.Quantum().Deserialize(data);
                if (model == null)
                    return null;

                return new Quantum(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
