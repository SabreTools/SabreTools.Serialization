using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class NCF : WrapperBase<Models.NCF.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life No Cache File (NCF)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public NCF(Models.NCF.File model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public NCF(Models.NCF.File model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NCF(Models.NCF.File model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public NCF(Models.NCF.File model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public NCF(Models.NCF.File model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NCF(Models.NCF.File model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an NCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the NCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
        public static NCF? Create(byte[]? data, int offset)
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
        /// Create a NCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the NCF</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
        public static NCF? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.NCF().Deserialize(data);
                if (model == null)
                    return null;

                return new NCF(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
