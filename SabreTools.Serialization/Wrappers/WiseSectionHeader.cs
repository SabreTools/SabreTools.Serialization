using System.IO;
using SabreTools.Data.Models.WiseInstaller;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WiseSectionHeader : WrapperBase<SectionHeader>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Self-Extracting Wise Installer Header";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Returns the offset relative to the start of the header
        /// where the compressed data lives
        /// </summary>
        /// TODO: Find a way for this to be automatically found based on the model
        ///     Likely would be able to replace most of the unknown values with
        ///     an array of values followed by padding bytes. This should be sufficient
        ///     to ensure that all possible values before the temp string are found
        ///     and read properly
        public long CompressedDataOffset { get; private set; }

        /// <inheritdoc cref="SectionHeader.UnknownDataSize"/>
        public uint UnknownDataSize => Model.UnknownDataSize;

        /// <inheritdoc cref="SectionHeader.SecondExecutableFileEntryLength"/> // TODO: VERIFY ON CHANGE
        public uint SecondExecutableFileEntryLength => Model.SecondExecutableFileEntryLength;

        /// <inheritdoc cref="SectionHeader.UnknownValue2"/>
        public uint UnknownValue2 => Model.UnknownValue2;

        /// <inheritdoc cref="SectionHeader.UnknownValue3"/>
        public uint UnknownValue3 => Model.UnknownValue3;

        /// <inheritdoc cref="SectionHeader.UnknownValue4"/>
        public uint UnknownValue4 => Model.UnknownValue4;

        /// <inheritdoc cref="SectionHeader.FirstExecutableFileEntryLength"/>
        public uint FirstExecutableFileEntryLength => Model.FirstExecutableFileEntryLength; // TODO: VERIFY ON CHANGE

        /// <inheritdoc cref="SectionHeader.MsiFileEntryLength"/>
        public uint MsiFileEntryLength => Model.MsiFileEntryLength;

        /// <inheritdoc cref="SectionHeader.UnknownValue7"/>
        public uint UnknownValue7 => Model.UnknownValue7;

        /// <inheritdoc cref="SectionHeader.UnknownValue8"/>
        public uint UnknownValue8 => Model.UnknownValue8;

        /// <inheritdoc cref="SectionHeader.ThirdExecutableFileEntryLength"/>
        public uint ThirdExecutableFileEntryLength => Model.ThirdExecutableFileEntryLength;

        /// <inheritdoc cref="SectionHeader.UnknownValue10"/>
        public uint UnknownValue10 => Model.UnknownValue10;

        /// <inheritdoc cref="SectionHeader.UnknownValue11"/>
        public uint UnknownValue11 => Model.UnknownValue11;

        /// <inheritdoc cref="SectionHeader.UnknownValue12"/>
        public uint UnknownValue12 => Model.UnknownValue12;

        /// <inheritdoc cref="SectionHeader.UnknownValue13"/>
        public uint UnknownValue13 => Model.UnknownValue13;

        /// <inheritdoc cref="SectionHeader.UnknownValue14"/>
        public uint UnknownValue14 => Model.UnknownValue14;

        /// <inheritdoc cref="SectionHeader.UnknownValue15"/>
        public uint UnknownValue15 => Model.UnknownValue15;

        /// <inheritdoc cref="SectionHeader.UnknownValue16"/>
        public uint UnknownValue16 => Model.UnknownValue16;

        /// <inheritdoc cref="SectionHeader.UnknownValue17"/>
        public uint UnknownValue17 => Model.UnknownValue17;

        /// <inheritdoc cref="SectionHeader.UnknownValue18"/>
        public uint UnknownValue18 => Model.UnknownValue18;

        /// <inheritdoc cref="SectionHeader.Version"/>
        public byte[]? Version => Model.Version;

        /// <inheritdoc cref="SectionHeader.PreStringValues"/>
        public byte[]? PreStringValues => Model.PreStringValues;

        /// <inheritdoc cref="SectionHeader.Strings"/>
        public byte[][]? Strings => Model.Strings;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WiseSectionHeader(SectionHeader model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public WiseSectionHeader(SectionHeader model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WiseSectionHeader(SectionHeader model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public WiseSectionHeader(SectionHeader model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public WiseSectionHeader(SectionHeader model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WiseSectionHeader(SectionHeader model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a Wise Self-Extracting installer .WISE section from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the section</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Wise Self-Extracting installer .WISE section wrapper on success, null on failure</returns>
        public static WiseSectionHeader? Create(byte[]? data, int offset)
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
        /// Create a Wise Self-Extracting installer .WISE section from a Stream
        /// </summary>
        /// <param name="data">Stream representing the section</param>
        /// <returns>A Wise Self-Extracting installer .WISE section wrapper on success, null on failure</returns>
        public static WiseSectionHeader? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.WiseSectionHeader().Deserialize(data);
                if (model == null)
                    return null;

                // HACK: Cache the end-of-header offset
                long endOffset = data.Position - currentOffset;

                return new WiseSectionHeader(model, data, currentOffset) { CompressedDataOffset = endOffset };
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
