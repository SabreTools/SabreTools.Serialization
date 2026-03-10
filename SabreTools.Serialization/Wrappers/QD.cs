using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public partial class QD : WrapperBase<Data.Models.NES.QD>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Quick Disk Famicom Disk System image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public QD(Data.Models.NES.QD model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public QD(Data.Models.NES.QD model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public QD(Data.Models.NES.QD model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public QD(Data.Models.NES.QD model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public QD(Data.Models.NES.QD model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public QD(Data.Models.NES.QD model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an NES cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static QD? Create(byte[]? data, int offset)
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
        public static QD? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.QD().Deserialize(data);
                if (model is null)
                    return null;

                return new QD(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
