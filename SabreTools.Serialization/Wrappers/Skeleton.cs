using System.IO;
using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Serialization.Wrappers
{
    public partial class Skeleton : ISO9660
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Redumper Skeleton";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public Skeleton(Volume model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public Skeleton(Volume model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an Skeleton Volume from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An Skeleton Volume wrapper on success, null on failure</returns>
        public new static Skeleton? Create(byte[]? data, int offset)
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
        /// Create an Skeleton Volume from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An Skeleton Volume wrapper on success, null on failure</returns>
        public new static Skeleton? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.ISO9660().Deserialize(data);
                if (model == null)
                    return null;

                return new Skeleton(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
