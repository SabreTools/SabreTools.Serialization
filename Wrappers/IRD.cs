using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class IRD : WrapperBase<Models.IRD.IRD>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PS3 IRD file";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public IRD(Models.IRD.IRD model, byte[] data, int offset)
#else
        public IRD(Models.IRD.IRD? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public IRD(Models.IRD.IRD model, Stream data)
#else
        public IRD(Models.IRD.IRD? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an IRD from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An IRD wrapper on success, null on failure</returns>
#if NET48
        public static IRD Create(byte[] data, int offset)
#else
        public static IRD? Create(byte[]? data, int offset)
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
        /// Create an IRD from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An IRD wrapper on success, null on failure</returns>
#if NET48
        public static IRD Create(Stream data)
#else
        public static IRD? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var ird = new Streams.IRD().Deserialize(data);
            if (ird == null)
                return null;

            try
            {
                return new IRD(ird, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}