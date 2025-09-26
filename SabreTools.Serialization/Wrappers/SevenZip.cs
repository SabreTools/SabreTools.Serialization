using System.IO;
using SabreTools.Serialization.Models.SevenZip;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public partial class SevenZip : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "7-zip Archive (or Derived Format)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public SevenZip(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public SevenZip(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SevenZip(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public SevenZip(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public SevenZip(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SevenZip(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a SevenZip archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A SevenZip wrapper on success, null on failure</returns>
        public static SevenZip? Create(byte[]? data, int offset)
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
        /// Create a SevenZip archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A SevenZip wrapper on success, null on failure</returns>
        public static SevenZip? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new SevenZip(new Archive(), data);
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
