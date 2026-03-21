using System.IO;
using System.Text;
using SabreTools.Data.Models.AtariLynx;

namespace SabreTools.Wrappers
{
    public partial class AtariLynxCart : WrapperBase<Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Atari Lynx Cart Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Cart.Header"/>
        public Header? Header => Model.Header;

        /// <inheritdoc cref="Header.Bank0PageSize"/>
        public ushort Bank0PageSize => Header?.Bank0PageSize ?? 0;

        /// <inheritdoc cref="Header.Bank1PageSize"/>
        public ushort Bank1PageSize => Header?.Bank1PageSize ?? 0;

        /// <inheritdoc cref="Header.CartName"/>
        public string? CartName => Header is null
            ? null
            : Encoding.ASCII.GetString(Header.CartName).TrimEnd('\0');

        /// <inheritdoc cref="Header.Manufacturer"/>
        public string? Manufacturer => Header is null
            ? null
            : Encoding.ASCII.GetString(Header.Manufacturer).TrimEnd('\0');

        /// <inheritdoc cref="Header.Rotation"/>
        public Rotation Rotation => Header?.Rotation ?? 0;

        /// <inheritdoc cref="Header.Version"/>
        public ushort Version => Header?.Version ?? 0;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public AtariLynxCart(Cart model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public AtariLynxCart(Cart model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public AtariLynxCart(Cart model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public AtariLynxCart(Cart model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public AtariLynxCart(Cart model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public AtariLynxCart(Cart model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an Atari Lynx cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An Atari Lynx cart image wrapper on success, null on failure</returns>
        public static AtariLynxCart? Create(byte[]? data, int offset)
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
        /// Create an Atari Lynx cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An Atari Lynx cart image wrapper on success, null on failure</returns>
        public static AtariLynxCart? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.AtariLynxCart().Deserialize(data);
                if (model is null)
                    return null;

                return new AtariLynxCart(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
