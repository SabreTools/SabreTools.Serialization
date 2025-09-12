using System.IO;
using SabreTools.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZSZDD : WrapperBase<SZDDFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "LZ-compressed file, SZDD variant";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="SZDDHeader.LastChar"/>
        public char LastChar => Model.Header?.LastChar ?? '\0';

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LZSZDD(SZDDFile? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public LZSZDD(SZDDFile? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an LZ (SZDD variant) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the LZ (SZDD variant)</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LZ (SZDD variant) wrapper on success, null on failure</returns>
        public static LZSZDD? Create(byte[]? data, int offset)
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
        /// Create a LZ (SZDD variant) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the LZ (SZDD variant)</param>
        /// <returns>An LZ (SZDD variant) wrapper on success, null on failure</returns>
        public static LZSZDD? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.LZSZDD.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new LZSZDD(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
