using System.IO;
using SabreTools.Data.Models.SecuROM;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SecuROMMatroschkaPackage : WrapperBase<MatroshkaPackage>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "SecuROM Matroschka Package";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="MatroshkaPackage.Signature"/>
        public string? Signature => Model.Signature;

        /// <inheritdoc cref="MatroshkaPackage.EntryCount"/>
        public uint EntryCount => Model.EntryCount;

        /// <inheritdoc cref="MatroshkaPackage.UnknownRCValue1"/>
        public uint? UnknownRCValue1 => Model.UnknownRCValue1;

        /// <inheritdoc cref="MatroshkaPackage.UnknownRCValue2"/>
        public uint? UnknownRCValue2 => Model.UnknownRCValue2;

        /// <inheritdoc cref="MatroshkaPackage.UnknownRCValue3"/>
        public uint? UnknownRCValue3 => Model.UnknownRCValue3;

        /// <inheritdoc cref="MatroshkaPackage.KeyHexString"/>
        public string? KeyHexString => Model.KeyHexString;

        /// <inheritdoc cref="MatroshkaPackage.Padding"/>
        public uint? Padding => Model.Padding;

        /// <inheritdoc cref="MatroshkaPackage.Entries"/>
        public MatroshkaEntry[] Entries => Model.Entries;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public SecuROMMatroschkaPackage(MatroshkaPackage model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public SecuROMMatroschkaPackage(MatroshkaPackage model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SecuROMMatroschkaPackage(MatroshkaPackage model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public SecuROMMatroschkaPackage(MatroshkaPackage model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public SecuROMMatroschkaPackage(MatroshkaPackage model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public SecuROMMatroschkaPackage(MatroshkaPackage model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a SecuROM Matroschka package from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the package</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A SecuROM Matroschka package wrapper on success, null on failure</returns>
        public static SecuROMMatroschkaPackage? Create(byte[]? data, int offset)
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
        /// Create a SecuROM Matroschka package from a Stream
        /// </summary>
        /// <param name="data">Stream representing the package</param>
        /// <returns>A SecuROM Matroschka package wrapper on success, null on failure</returns>
        public static SecuROMMatroschkaPackage? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.SecuROMMatroschkaPackage().Deserialize(data);
                if (model == null)
                    return null;

                return new SecuROMMatroschkaPackage(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
