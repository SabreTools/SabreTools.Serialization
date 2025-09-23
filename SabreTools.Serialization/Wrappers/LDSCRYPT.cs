using System.IO;
using SabreTools.Models.LDSCRYPT;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// This is a shell wrapper; one that does not contain
    /// any actual parsing. It is used as a placeholder for
    /// types that typically do not have models.
    /// </summary>
    public class LDSCRYPT : WrapperBase<EncryptedFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Link Data Security encrypted file";

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LDSCRYPT(EncryptedFile model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public LDSCRYPT(EncryptedFile model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public LDSCRYPT(EncryptedFile model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public LDSCRYPT(EncryptedFile model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public LDSCRYPT(EncryptedFile model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public LDSCRYPT(EncryptedFile model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a LDSCRYPT file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A LDSCRYPT wrapper on success, null on failure</returns>
        public static LDSCRYPT? Create(byte[]? data, int offset)
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
        /// Create a LDSCRYPT file from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A LDSCRYPT wrapper on success, null on failure</returns>
        public static LDSCRYPT? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            return new LDSCRYPT(new EncryptedFile(), data);
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
