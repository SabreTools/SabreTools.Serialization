using System.IO;
using SabreTools.Data.Models.MSDOS;

namespace SabreTools.Serialization.Wrappers
{
    public class MSDOS : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "MS-DOS Executable";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public MSDOS(Executable model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public MSDOS(Executable model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public MSDOS(Executable model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public MSDOS(Executable model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public MSDOS(Executable model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public MSDOS(Executable model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an MS-DOS executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
        public static MSDOS? Create(byte[]? data, int offset)
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
        /// Create an MS-DOS executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
        public static MSDOS? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.MSDOS().Deserialize(data);
                if (model == null)
                    return null;

                return new MSDOS(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
