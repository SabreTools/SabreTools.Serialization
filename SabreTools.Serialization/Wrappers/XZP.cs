using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XZP : WrapperBase<SabreTools.Models.XZP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Package File (XZP)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.XZP.File.DirectoryEntries"/>
        public SabreTools.Models.XZP.DirectoryEntry[]? DirectoryEntries => Model.DirectoryEntries;

        /// <inheritdoc cref="Models.XZP.File.DirectoryItems"/>
        public SabreTools.Models.XZP.DirectoryItem[]? DirectoryItems => Model.DirectoryItems;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XZP(SabreTools.Models.XZP.File model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XZP(SabreTools.Models.XZP.File model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XZP(SabreTools.Models.XZP.File model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XZP(SabreTools.Models.XZP.File model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XZP(SabreTools.Models.XZP.File model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XZP(SabreTools.Models.XZP.File model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

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

                var model = new Deserializers.XZP().Deserialize(data);
                if (model == null)
                    return null;

                return new XZP(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
