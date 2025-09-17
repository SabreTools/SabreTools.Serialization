using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WAD3 : WrapperBase<Models.WAD3.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Texture Package File (WAD3)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.WAD3.File.DirEntries"/>
        public Models.WAD3.DirEntry[]? DirEntries => Model.DirEntries;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WAD3(Models.WAD3.File model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a WAD3 from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WAD3</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WAD3 wrapper on success, null on failure</returns>
        public static WAD3? Create(byte[]? data, int offset)
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
        /// Create a WAD3 from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WAD3</param>
        /// <returns>An WAD3 wrapper on success, null on failure</returns>
        public static WAD3? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.WAD3.DeserializeStream(data);
                if (model == null)
                    return null;

                return new WAD3(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
