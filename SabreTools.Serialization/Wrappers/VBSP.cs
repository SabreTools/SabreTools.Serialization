using System.IO;
using SabreTools.Serialization.Models.BSP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class VBSP : WrapperBase<VbspFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life 2 Level (VBSP)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="VbspHeader.Lumps"/>
        public VbspLumpEntry[]? Lumps => Model.Header?.Lumps;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public VBSP(VbspFile model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public VBSP(VbspFile model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public VBSP(VbspFile model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public VBSP(VbspFile model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public VBSP(VbspFile model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public VBSP(VbspFile model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a VBSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VBSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VBSP wrapper on success, null on failure</returns>
        public static VBSP? Create(byte[]? data, int offset)
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
        /// Create a VBSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the VBSP</param>
        /// <returns>An VBSP wrapper on success, null on failure</returns>
        public static VBSP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.VBSP().Deserialize(data);
                if (model == null)
                    return null;

                return new VBSP(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
