using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class XZP : WrapperBase<Models.XZP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Package File (XZP)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public XZP(Models.XZP.File model, byte[] data, int offset)
#else
        public XZP(Models.XZP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public XZP(Models.XZP.File model, Stream data)
#else
        public XZP(Models.XZP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a XZP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XZP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
#if NET48
        public static XZP Create(byte[] data, int offset)
#else
        public static XZP? Create(byte[]? data, int offset)
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
        /// Create a XZP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XZP</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
#if NET48
        public static XZP Create(Stream data)
#else
        public static XZP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new Streams.XZP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new XZP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}