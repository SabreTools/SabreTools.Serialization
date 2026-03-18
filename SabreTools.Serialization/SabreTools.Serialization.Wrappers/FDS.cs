using System.IO;
using SabreTools.Data.Models.NES;

namespace SabreTools.Serialization.Wrappers
{
    public partial class FDS : WrapperBase<Data.Models.NES.FDS>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "fwNES FDS File";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Data.Models.NES.FDS.Header"/>
        public FDSHeader? Header => Model.Header;

        /// <inheritdoc cref="FDSHeader.DiskSides"/>
        public byte DiskSides => Header?.DiskSides ?? 0;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public FDS(Data.Models.NES.FDS model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public FDS(Data.Models.NES.FDS model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public FDS(Data.Models.NES.FDS model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public FDS(Data.Models.NES.FDS model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public FDS(Data.Models.NES.FDS model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public FDS(Data.Models.NES.FDS model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an NES cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static FDS? Create(byte[]? data, int offset)
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
        /// Create an NES cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static FDS? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.FDS().Deserialize(data);
                if (model is null)
                    return null;

                return new FDS(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
