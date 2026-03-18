using System.IO;
using System.Text;
using SabreTools.Data.Models.Atari7800;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Atari7800Cart : WrapperBase<Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Atari 7800 Cart Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Cart.Header"/>
        public Header? Header => Model.Header;

        /// <inheritdoc cref="Header.AudioDevice"/>
        public AudioDevice AudioDevice => Header?.AudioDevice ?? 0;

        /// <inheritdoc cref="Header.CartTitle"/>
        public string? CartTitle
        {
            get
            {
                // Missing header
                if (Header is null)
                    return null;

                return Encoding.ASCII.GetString(Header.CartTitle);
            }
        }

        /// <inheritdoc cref="Header.CartType"/>
        public CartType CartType => Header?.CartType ?? 0;

        /// <inheritdoc cref="Header.Controller1Type"/>
        public ControllerType Controller1Type => Header?.Controller1Type ?? 0;

        /// <inheritdoc cref="Header.Controller2Type"/>
        public ControllerType Controller2Type => Header?.Controller2Type ?? 0;

        /// <inheritdoc cref="Header.HeaderVersion"/>
        public byte HeaderVersion => Header?.HeaderVersion ?? 0;

        /// <inheritdoc cref="Header.Interrupt"/>
        public Interrupt Interrupt => Header?.Interrupt ?? 0;

        /// <inheritdoc cref="Header.Mapper"/>
        public Mapper Mapper => Header?.Mapper ?? 0;

        /// <inheritdoc cref="Header.MapperOptions"/>
        public MapperOptions MapperOptions => Header?.MapperOptions ?? 0;

        /// <inheritdoc cref="Header.RomSizeWithoutHeader"/>
        public uint RomSizeWithoutHeader => Header?.RomSizeWithoutHeader ?? 0;

        /// <inheritdoc cref="Header.SaveDevice"/>
        public SaveDevice SaveDevice => Header?.SaveDevice ?? 0;

        /// <inheritdoc cref="Header.SlotPassthroughDevice"/>
        public SlotPassthroughDevice SlotPassthroughDevice => Header?.SlotPassthroughDevice ?? 0;

        /// <inheritdoc cref="Header.TVType"/>
        public TVType TVType => Header?.TVType ?? 0;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Atari7800Cart(Cart model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public Atari7800Cart(Cart model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Atari7800Cart(Cart model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public Atari7800Cart(Cart model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public Atari7800Cart(Cart model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Atari7800Cart(Cart model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an Atari 7800 cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An Atari 7800 cart image wrapper on success, null on failure</returns>
        public static Atari7800Cart? Create(byte[]? data, int offset)
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
        /// Create an Atari 7800 cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An Atari 7800 cart image wrapper on success, null on failure</returns>
        public static Atari7800Cart? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.Atari7800Cart().Deserialize(data);
                if (model is null)
                    return null;

                return new Atari7800Cart(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
