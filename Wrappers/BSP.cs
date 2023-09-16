using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class BSP : WrapperBase<Models.BSP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Level (BSP)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public BSP(Models.BSP.File model, byte[] data, int offset)
#else
        public BSP(Models.BSP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public BSP(Models.BSP.File model, Stream data)
#else
        public BSP(Models.BSP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the BSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BSP wrapper on success, null on failure</returns>
#if NET48
        public static BSP Create(byte[] data, int offset)
#else
        public static BSP? Create(byte[]? data, int offset)
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
        /// Create a BSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the BSP</param>
        /// <returns>An BSP wrapper on success, null on failure</returns>
#if NET48
        public static BSP Create(Stream data)
#else
        public static BSP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new Streams.BSP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new BSP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}