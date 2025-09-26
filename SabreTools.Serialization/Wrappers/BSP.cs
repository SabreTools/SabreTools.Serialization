using System.IO;
using SabreTools.Serialization.Models.BSP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class BSP : WrapperBase<BspFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Level (BSP)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="BspHeader.Lumps"/>
        public BspLumpEntry[]? Lumps => Model.Header?.Lumps;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public BSP(BspFile model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public BSP(BspFile model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public BSP(BspFile model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public BSP(BspFile model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public BSP(BspFile model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public BSP(BspFile model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a BSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the BSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BSP wrapper on success, null on failure</returns>
        public static BSP? Create(byte[]? data, int offset)
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
        /// Create a BSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the BSP</param>
        /// <returns>An BSP wrapper on success, null on failure</returns>
        public static BSP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.BSP().Deserialize(data);
                if (model == null)
                    return null;

                return new BSP(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
