using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    /// TODO: Hook up the models to a proper deserializer
    public class SFFS : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "StarForce File System";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public SFFS(byte[] data) : base(data) { }

        /// <inheritdoc/>
        public SFFS(byte[] data, int offset) : base(data, offset) { }

        /// <inheritdoc/>
        public SFFS(byte[] data, int offset, int length) : base(data, offset, length) { }

        /// <inheritdoc/>
        public SFFS(Stream data) : base(data) { }

        /// <inheritdoc/>
        public SFFS(Stream data, long offset) : base(data, offset) { }

        /// <inheritdoc/>
        public SFFS(Stream data, long offset, long length) : base(data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a SFFS file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A SFFS wrapper on success, null on failure</returns>
        public static SFFS? Create(byte[]? data, int offset)
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
        /// Create a SFFS file (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A SFFS wrapper on success, null on failure</returns>
        public static SFFS? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new SFFS(data);
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
