using System.IO;
using SabreTools.Data.Models.XboxExecutable;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XboxExecutable : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "XBox Executable";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Header.BaseAddress"/>
        public uint BaseAddress => Model.Header?.BaseAddress ?? 0;

        /// <inheritdoc cref="Executable.Certificate"/>
        public Certificate? Certificate => Model.Certificate;

        /// <inheritdoc cref="Header.DigitalSignature"/>
        public byte[] DigitalSignature => Model.Header?.DigitalSignature ?? [];

        /// <inheritdoc cref="Executable.KernelLibraryVersion"/>
        public LibraryVersion? KernelLibraryVersion => Model.KernelLibraryVersion;

        /// <inheritdoc cref="Executable.LibraryVersions"/>
        public LibraryVersion[] LibraryVersions => Model.LibraryVersions;

        /// <inheritdoc cref="Executable.SectionHeaders"/>
        public SectionHeader[] SectionHeaders => Model.SectionHeaders;

        /// <inheritdoc cref="Executable.ThreadLocalStorage"/>
        public ThreadLocalStorage? ThreadLocalStorage => Model.ThreadLocalStorage;

        /// <inheritdoc cref="Executable.XAPILibraryVersion"/>
        public LibraryVersion? XAPILibraryVersion => Model.XAPILibraryVersion;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public XboxExecutable(Executable model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public XboxExecutable(Executable model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XboxExecutable(Executable model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public XboxExecutable(Executable model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public XboxExecutable(Executable model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public XboxExecutable(Executable model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an XBox Executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An XBox Executable wrapper on success, null on failure</returns>
        public static XboxExecutable? Create(byte[]? data, int offset)
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
        /// Create an XBox Executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An XBox Executable wrapper on success, null on failure</returns>
        public static XboxExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.XboxExecutable().Deserialize(data);
                if (model is null)
                    return null;

                return new XboxExecutable(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
