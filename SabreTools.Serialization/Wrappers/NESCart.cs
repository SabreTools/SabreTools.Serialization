using System.IO;
using SabreTools.Data.Models.NES;

namespace SabreTools.Serialization.Wrappers
{
    // TODO: Add printer class
    // TODO: Add extension properties
    public class NESCart : WrapperBase<Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo Entertainment System Cart Image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public NESCart(Cart model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public NESCart(Cart model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NESCart(Cart model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public NESCart(Cart model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public NESCart(Cart model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NESCart(Cart model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an NES cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static NESCart? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an NES cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static NESCart? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.NESCart().Deserialize(data);
                if (model is null)
                    return null;

                return new NESCart(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
