using System;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XboxExecutable;

namespace SabreTools.Wrappers
{
    public partial class XboxExecutable : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "XBox Executable";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Certificate.AlternativeTitleIDs"/>
        public uint[]? AlternativeTitleIDs => Certificate?.AlternativeTitleIDs;

        /// <inheritdoc cref="Certificate.AlternativeTitleIDs"/>
        public string[]? AlternativeTitleIDsStrings
        {
            get
            {
                // Ignore invalid alternative title IDs
                if (AlternativeTitleIDs is null)
                    return null;

                return Array.ConvertAll(AlternativeTitleIDs, ba => ba.ToFormattedXBETitleID());
            }
        }

        /// <inheritdoc cref="Header.BaseAddress"/>
        public uint BaseAddress => Model.Header?.BaseAddress ?? 0;

        /// <inheritdoc cref="Executable.Certificate"/>
        public Certificate? Certificate => Model.Certificate;

        /// <inheritdoc cref="Header.DigitalSignature"/>
        public byte[] DigitalSignature => Model.Header?.DigitalSignature ?? [];

        /// <inheritdoc cref="Header.EntryPoint"/>
        /// <remarks>Adjusted with debug shift</remarks>
        public uint EntryPointDebug => (Model.Header?.EntryPoint ?? 0) ^ 0x94859D4B;

        /// <inheritdoc cref="Header.EntryPoint"/>
        /// <remarks>Adjusted with debug shift</remarks>
        public uint EntryPointRetail => (Model.Header?.EntryPoint ?? 0) ^ 0xA8FC57AB;

        /// <inheritdoc cref="Executable.KernelLibraryVersion"/>
        public LibraryVersion? KernelLibraryVersion => Model.KernelLibraryVersion;

        /// <inheritdoc cref="Header.KernelImageThunkAddress"/>
        /// <remarks>Adjusted with debug shift</remarks>
        public uint KernelImageThunkAddressDebug => (Model.Header?.KernelImageThunkAddress ?? 0) ^ 0xEFB1F152;

        /// <inheritdoc cref="Header.KernelImageThunkAddress"/>
        /// <remarks>Adjusted with debug shift</remarks>
        public uint KernelImageThunkAddressRetail => (Model.Header?.KernelImageThunkAddress ?? 0) ^ 0x5B6D40B6;

        /// <inheritdoc cref="Executable.LibraryVersions"/>
        public LibraryVersion[] LibraryVersions => Model.LibraryVersions;

        /// <inheritdoc cref="Executable.SectionHeaders"/>
        public SectionHeader[] SectionHeaders => Model.SectionHeaders;

        /// <inheritdoc cref="Executable.ThreadLocalStorage"/>
        public ThreadLocalStorage? ThreadLocalStorage => Model.ThreadLocalStorage;

        /// <inheritdoc cref="Certificate.TitleID"/>
        public uint TitleID => Certificate?.TitleID ?? 0;

        /// <inheritdoc cref="Certificate.TitleID"/>
        public string TitleIDString => TitleID.ToFormattedXBETitleID();

        /// <inheritdoc cref="Certificate.TitleName"/>
        public string? TitleName
        {
            get
            {
                // Ignore invalid certificates
                if (Certificate is null)
                    return null;

                return Encoding.Unicode.GetString(Certificate.TitleName);
            }
        }

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

                var model = new Serialization.Readers.XboxExecutable().Deserialize(data);
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
