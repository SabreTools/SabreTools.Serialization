using System.IO;
using SabreTools.Data.Models.ZArchive;

namespace SabreTools.Wrappers
{
    public partial class ZArchive : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "ZArchive";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.OffsetRecords"/>
        public OffsetRecord[] OffsetRecords => Model.OffsetRecords;

        /// <inheritdoc cref="Archive.NameTable"/>
        public NameTable NameTable => Model.NameTable;

        /// <inheritdoc cref="Archive.FileTree"/>
        public FileDirectoryEntry[] FileTree => Model.FileTree;

        /// <inheritdoc cref="Archive.Footer"/>
        public Footer Footer => Model.Footer;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public ZArchive(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public ZArchive(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public ZArchive(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public ZArchive(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public ZArchive(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public ZArchive(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a ZArchive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A ZArchive wrapper on success, null on failure</returns>
        public static ZArchive? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a ZArchive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A ZArchive wrapper on success, null on failure</returns>
        public static ZArchive? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Serialization.Readers.ZArchive().Deserialize(data);
                if (model is null)
                    return null;

                return new ZArchive(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
