using System.IO;
using SabreTools.Serialization.Models.BZip2;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public partial class BZip2 : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "bzip2 Archive";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public BZip2(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public BZip2(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public BZip2(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public BZip2(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public BZip2(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public BZip2(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a BZip2 archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BZip2 wrapper on success, null on failure</returns>
        public static BZip2? Create(byte[]? data, int offset)
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
        /// Create a BZip2 archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BZip2 wrapper on success, null on failure</returns>
        public static BZip2? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new BZip2(new Archive(), data);
        }

        #endregion

        #region JSON Export

#if NETCOREAPP
        /// <inheritdoc/>
        public override string ExportJSON() => throw new System.NotImplementedException();

#endif

        #endregion
    }
}
