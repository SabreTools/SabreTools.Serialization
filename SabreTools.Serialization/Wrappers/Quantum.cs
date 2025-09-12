using System.IO;
using SabreTools.Models.Quantum;

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
        public Quantum(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public Quantum(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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

                var model = Deserializers.Quantum.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new Quantum(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
