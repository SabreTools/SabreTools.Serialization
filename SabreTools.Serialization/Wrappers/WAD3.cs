using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class WAD3 : WrapperBase<Models.WAD3.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Texture Package File (WAD3)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a WAD3 from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WAD3</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WAD3 wrapper on success, null on failure</returns>
        public static WAD3? Create(byte[]? data, int offset)
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
        /// Create a WAD3 from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WAD3</param>
        /// <returns>An WAD3 wrapper on success, null on failure</returns>
        public static WAD3? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Deserializers.WAD3.DeserializeStream(data);
            if (file == null)
                return null;

            try
            {
                return new WAD3(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}