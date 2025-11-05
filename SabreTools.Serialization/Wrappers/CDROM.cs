using System.IO;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
{
    public partial class CDROM : ISO9660
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "CD-ROM ISO 9660 Volume";

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
        /// Create a CDROM data track from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the CDROM data track</param>
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
        /// Create a CDROM data track from a Stream
        /// </summary>
        /// <param name="data">Seekable Stream representing the CDROM data track</param>
        /// <returns>A CDROM data track wrapper on success, null on failure</returns>
        public new static CDROM? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead || !data.CanSeek)
                return null;

            try
            {
                // Create user data sub-stream
                SabreTools.Data.Extensions.CDROM.ISO9660Stream userData = new(data);

                // Cache the current offset
                long currentOffset = userData.Position;

                // Deserialize just the sub-stream
                var model = new Readers.ISO9660().Deserialize(userData);
                if (model == null)
                    return null;

                // Reset stream
                userData.Seek(currentOffset, SeekOrigin.Begin);

                return new CDROM(model, userData, userData.Position);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
