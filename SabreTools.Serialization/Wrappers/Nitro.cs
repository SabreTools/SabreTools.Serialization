using System.IO;
using SabreTools.Models.Nitro;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Nitro : WrapperBase<Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo DS/DSi Cart Image";

        #endregion

        #region Extension Methods

        /// <inheritdoc cref="Cart.CommonHeader"/>
        public CommonHeader? CommonHeader => Model.CommonHeader;

        /// <inheritdoc cref="CommonHeader.GameCode"/>
        public uint GameCode => Model.CommonHeader?.GameCode ?? 0;

        /// <inheritdoc cref="Cart.SecureArea"/>
        public byte[]? SecureArea => Model.SecureArea;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Nitro(Cart model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public Nitro(Cart model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Nitro(Cart model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public Nitro(Cart model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public Nitro(Cart model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Nitro(Cart model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a NDS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A NDS cart image wrapper on success, null on failure</returns>
        public static Nitro? Create(byte[]? data, int offset)
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
        /// Create a NDS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A NDS cart image wrapper on success, null on failure</returns>
        public static Nitro? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.Nitro().Deserialize(data);
                if (model == null)
                    return null;

                return new Nitro(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
