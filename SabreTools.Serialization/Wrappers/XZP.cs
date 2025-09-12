using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XZP : WrapperBase<Models.XZP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Package File (XZP)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.XZP.File.DirectoryEntries"/>
        public Models.XZP.DirectoryEntry[]? DirectoryEntries => Model.DirectoryEntries;

        /// <inheritdoc cref="Models.XZP.File.DirectoryItems"/>
        public Models.XZP.DirectoryItem[]? DirectoryItems => Model.DirectoryItems;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XZP(Models.XZP.File? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public XZP(Models.XZP.File? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a XZP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XZP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
        public static XZP? Create(byte[]? data, int offset)
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
        /// Create a XZP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XZP</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
        public static XZP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.XZP.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new XZP(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
