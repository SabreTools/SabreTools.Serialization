using System.IO;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
{
    public partial class CDROM : ISO9660
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "CD-ROM Data Track";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public CDROM(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public CDROM(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public CDROM(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public CDROM(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public CDROM(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public CDROM(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an CDROM data track from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A CDROM data track wrapper on success, null on failure</returns>
        public new static CDROM? Create(byte[]? data, int offset)
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
        /// Create an CDROM data track from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A CDROM data track wrapper on success, null on failure</returns>
        public new static CDROM? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.CDROMVolume().Deserialize(data);
                if (model == null)
                    return null;

                return new CDROM(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
