using System.IO;
using SabreTools.Data.Models.PIC;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PIC : WrapperBase<DiscInformation>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Disc Information";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="DiscInformation.Units"/>
        public DiscInformationUnit[] Units => Model.Units ?? [];

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PIC(DiscInformation model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PIC(DiscInformation model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PIC(DiscInformation model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PIC(DiscInformation model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PIC(DiscInformation model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PIC(DiscInformation model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a PIC disc information object from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the information</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PIC disc information wrapper on success, null on failure</returns>
        public static PIC? Create(byte[]? data, int offset)
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
        /// Create a PIC disc information object from a Stream
        /// </summary>
        /// <param name="data">Stream representing the information</param>
        /// <returns>A PIC disc information wrapper on success, null on failure</returns>
        public static PIC? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {

                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.PIC().Deserialize(data);
                if (model == null)
                    return null;

                return new PIC(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
