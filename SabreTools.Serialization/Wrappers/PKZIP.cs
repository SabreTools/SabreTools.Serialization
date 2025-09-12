using System.IO;
using SabreTools.Models.PKZIP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PKZIP : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PKZIP Archive (or Derived Format)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.CentralDirectoryHeaders"/>
        public CentralDirectoryFileHeader[]? CentralDirectoryHeader => Model.CentralDirectoryHeaders;

        /// <inheritdoc cref="Archive.EndOfCentralDirectoryRecord"/>
        public EndOfCentralDirectoryRecord? EndOfCentralDirectoryRecord => Model.EndOfCentralDirectoryRecord;

        /// <inheritdoc cref="Archive.ZIP64EndOfCentralDirectoryLocator"/>
        public EndOfCentralDirectoryLocator64? ZIP64EndOfCentralDirectoryLocator => Model.ZIP64EndOfCentralDirectoryLocator;

        /// <inheritdoc cref="Archive.ZIP64EndOfCentralDirectoryRecord"/>
        public EndOfCentralDirectoryRecord64? ZIP64EndOfCentralDirectoryRecord => Model.ZIP64EndOfCentralDirectoryRecord;

        /// <inheritdoc cref="Archive.LocalFiles"/>
        public LocalFile[]? LocalFiles => Model.LocalFiles;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        /// <remarks>This should only be used for WinZipSFX</remarks>
        public PKZIP(byte[]? data, int offset)
            : base(new Archive(), data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        /// <remarks>This should only be used for WinZipSFX</remarks>
        public PKZIP(Stream? data)
            : base(new Archive(), data)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PKZIP(Archive? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public PKZIP(Archive? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PKZIP archive (or derived format) from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PKZIP wrapper on success, null on failure</returns>
        public static PKZIP? Create(byte[]? data, int offset)
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
        /// Create a PKZIP archive (or derived format) from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PKZIP wrapper on success, null on failure</returns>
        public static PKZIP? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.PKZIP.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new PKZIP(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
