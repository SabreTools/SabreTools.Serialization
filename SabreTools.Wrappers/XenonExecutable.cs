using System;
using System.IO;
using System.Text;
using SabreTools.Data.Models.XenonExecutable;

namespace SabreTools.Wrappers
{
    public partial class XenonExecutable : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xenon (Xbox 360) Executable";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header"/>
        public Header Header => Model.Header;

        /// <inheritdoc cref="Certificate"/>
        public Certificate Certificate => Model.Certificate;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XenonExecutable(Executable model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XenonExecutable(Executable model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XenonExecutable(Executable model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XenonExecutable(Executable model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XenonExecutable(Executable model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XenonExecutable(Executable model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a Xenon (Xbox 360) Executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Xenon (Xbox 360) Executable wrapper on success, null on failure</returns>
        public static XenonExecutable? Create(byte[]? data, int offset)
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
        /// Create a Xenon (Xbox 360) Executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Xenon (Xbox 360) Executable wrapper on success, null on failure</returns>
        public static XenonExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.XenonExecutable().Deserialize(data);
                if (model is null)
                    return null;

                return new XenonExecutable(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
