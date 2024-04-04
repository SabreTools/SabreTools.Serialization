using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    // TODO: Figure out extension properties
    public partial class MoPaQ : WrapperBase<Models.MoPaQ.Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "MoPaQ Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public MoPaQ(Models.MoPaQ.Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public MoPaQ(Models.MoPaQ.Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a MoPaQ archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A MoPaQ archive wrapper on success, null on failure</returns>
        public static MoPaQ? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a MoPaQ archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A MoPaQ archive wrapper on success, null on failure</returns>
        public static MoPaQ? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = Deserializers.MoPaQ.DeserializeStream(data);
            if (cabinet == null)
                return null;

            try
            {
                return new MoPaQ(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
