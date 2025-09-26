using System.IO;
using SabreTools.Models.PAK;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PAK : WrapperBase<SabreTools.Models.PAK.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Package File (PAK)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.PAK.File.DirectoryItems"/>
        public DirectoryItem[] DirectoryItems => Model.DirectoryItems ?? [];

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PAK(SabreTools.Models.PAK.File model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PAK(SabreTools.Models.PAK.File model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PAK(SabreTools.Models.PAK.File model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PAK(SabreTools.Models.PAK.File model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PAK(SabreTools.Models.PAK.File model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PAK(SabreTools.Models.PAK.File model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a PAK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the PAK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
        public static PAK? Create(byte[]? data, int offset)
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
        /// Create a PAK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the PAK</param>
        /// <returns>A PAK wrapper on success, null on failure</returns>
        public static PAK? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.PAK().Deserialize(data);
                if (model == null)
                    return null;

                return new PAK(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
