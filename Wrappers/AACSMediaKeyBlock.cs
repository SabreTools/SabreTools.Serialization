using System.IO;
using SabreTools.Models.AACS;

namespace SabreTools.Serialization.Wrappers
{
    public class AACSMediaKeyBlock : WrapperBase<MediaKeyBlock>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "AACS Media Key Block";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public AACSMediaKeyBlock(MediaKeyBlock? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an AACS media key block from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
        public static AACSMediaKeyBlock? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an AACS media key block from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
        public static AACSMediaKeyBlock? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var mediaKeyBlock = new Streams.AACS().Deserialize(data);
            if (mediaKeyBlock == null)
                return null;

            try
            {
                return new AACSMediaKeyBlock(mediaKeyBlock, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}