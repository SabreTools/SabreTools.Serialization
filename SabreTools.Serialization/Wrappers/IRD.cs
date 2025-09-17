using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class IRD : WrapperBase<Models.IRD.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PS3 IRD file";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public IRD(Models.IRD.File model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public IRD(Models.IRD.File model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public IRD(Models.IRD.File model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public IRD(Models.IRD.File model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public IRD(Models.IRD.File model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public IRD(Models.IRD.File model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an IRD from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An IRD wrapper on success, null on failure</returns>
        public static IRD? Create(byte[]? data, int offset)
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
        /// Create an IRD from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An IRD wrapper on success, null on failure</returns>
        public static IRD? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.IRD().Deserialize(data);
                if (model == null)
                    return null;

                return new IRD(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
