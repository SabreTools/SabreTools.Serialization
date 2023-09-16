using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class BFPK : WrapperBase<Models.BFPK.Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "BFPK Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public BFPK(Models.BFPK.Archive model, byte[] data, int offset)
#else
        public BFPK(Models.BFPK.Archive? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public BFPK(Models.BFPK.Archive model, Stream data)
#else
        public BFPK(Models.BFPK.Archive? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BFPK archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
#if NET48
        public static BFPK Create(byte[] data, int offset)
#else
        public static BFPK? Create(byte[]? data, int offset)
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
        /// Create a BFPK archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
#if NET48
        public static BFPK Create(Stream data)
#else
        public static BFPK? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new Streams.BFPK().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new BFPK(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}