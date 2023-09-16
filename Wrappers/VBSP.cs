using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class VBSP : WrapperBase<Models.VBSP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life 2 Level (VBSP)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public VBSP(Models.VBSP.File model, byte[] data, int offset)
#else
        public VBSP(Models.VBSP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public VBSP(Models.VBSP.File model, Stream data)
#else
        public VBSP(Models.VBSP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a VBSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VBSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VBSP wrapper on success, null on failure</returns>
#if NET48
        public static VBSP Create(byte[] data, int offset)
#else
        public static VBSP? Create(byte[]? data, int offset)
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
        /// Create a VBSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the VBSP</param>
        /// <returns>An VBSP wrapper on success, null on failure</returns>
#if NET48
        public static VBSP Create(Stream data)
#else
        public static VBSP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new Streams.VBSP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new VBSP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}