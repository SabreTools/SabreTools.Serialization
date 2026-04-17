using System.IO;
using SabreTools.Data.Models.WIA;

namespace SabreTools.Wrappers
{
    public partial class WIA : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "WIA / RVZ Compressed GameCube / Wii Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Header1"/>
        public WiaHeader1 Header1 => Model.Header1;

        /// <inheritdoc cref="Archive.Header2"/>
        public WiaHeader2 Header2 => Model.Header2;

        /// <inheritdoc cref="Archive.IsRvz"/>
        public bool IsRvz => Model.IsRvz;

        /// <inheritdoc cref="Archive.PartitionEntries"/>
        public PartitionEntry[]? PartitionEntries => Model.PartitionEntries;

        /// <inheritdoc cref="Archive.RawDataEntries"/>
        public RawDataEntry[] RawDataEntries => Model.RawDataEntries;

        /// <summary>
        /// Total uncompressed ISO size in bytes
        /// </summary>
        public ulong IsoFileSize => Model.Header1.IsoFileSize;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WIA(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public WIA(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WIA(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public WIA(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public WIA(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WIA(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a WIA/RVZ wrapper from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WIA or RVZ image</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WIA wrapper on success, null on failure</returns>
        public static WIA? Create(byte[]? data, int offset)
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
        /// Create a WIA/RVZ wrapper from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WIA or RVZ image</param>
        /// <returns>A WIA wrapper on success, null on failure</returns>
        public static WIA? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                long currentOffset = data.Position;

                var model = new Serialization.Readers.WIA().Deserialize(data);
                if (model is null)
                    return null;

                return new WIA(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Inner Wrapper

        /// <summary>
        /// Decompress the full WIA/RVZ image to a MemoryStream and return a NintendoDisc wrapper.
        /// Returns null if decompression fails or the decompressed data is not a valid disc image.
        /// </summary>
        public NintendoDisc? GetInnerWrapper()
        {
            // TODO: Implement full group-level decompression into a seekable MemoryStream,
            // then pass to NintendoDisc.Create(). Use WiaRvzCompressionHelper.Decompress()
            // for each group entry, assembling the groups in order according to the
            // RawDataEntries and PartitionEntries offset/size tables.
            return null;
        }

        #endregion
    }
}
