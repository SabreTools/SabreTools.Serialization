using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class PAK : WrapperBase<Models.PAK.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Package File (PAK)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public PAK(Models.PAK.File model, byte[] data, int offset)
#else
        public PAK(Models.PAK.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public PAK(Models.PAK.File model, Stream data)
#else
        public PAK(Models.PAK.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PAK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the PAK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
#if NET48
        public static PAK Create(byte[] data, int offset)
#else
        public static PAK? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a PAK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the PAK</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
#if NET48
        public static PAK Create(Stream data)
#else
        public static PAK? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new Streams.PAK().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new PAK(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}