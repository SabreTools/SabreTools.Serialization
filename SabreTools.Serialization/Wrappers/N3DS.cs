using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class N3DS : WrapperBase<Models.N3DS.Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo 3DS Cart Image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public N3DS(Models.N3DS.Cart? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public N3DS(Models.N3DS.Cart? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a 3DS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS? Create(byte[]? data, int offset)
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
        /// Create a 3DS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Deserializers.N3DS.DeserializeStream(data);
            if (archive == null)
                return null;

            try
            {
                return new N3DS(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}