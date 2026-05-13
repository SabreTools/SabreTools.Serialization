using System.IO;
using System.IO.Compression;
using SabreTools.Data.Models.GCZ;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.GCZ.Constants;

namespace SabreTools.Wrappers
{
    public partial class GCZ : WrapperBase<DiscImage>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "GCZ Compressed GameCube / Wii Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="DiscImage.Header"/>
        public GczHeader Header => Model.Header;

        /// <inheritdoc cref="GczHeader.CompressedDataSize"/>
        public ulong CompressedDataSize => Header.CompressedDataSize;

        /// <inheritdoc cref="GczHeader.DataSize"/>
        public ulong DataSize => Header.DataSize;

        /// <inheritdoc cref="GczHeader.NumBlocks"/>
        public uint NumBlocks => Header.NumBlocks;

        /// <inheritdoc cref="GczHeader.BlockSize"/>
        public uint BlockSize => Header.BlockSize;

        /// <inheritdoc cref="DiscImage.BlockPointers"/>
        public ulong[] BlockPointers => Model.BlockPointers;

        /// <inheritdoc cref="DiscImage.BlockHashes"/>
        public uint[] BlockHashes => Model.BlockHashes;

        /// <summary>
        /// Disc header parsed by decompressing the first block of the GCZ image.
        /// </summary>
        public DiscHeader? DiscHeader
        {
            get
            {
                if (field is not null)
                    return field;

                field = ReadDiscHeader();
                return field;
            }
        }

        /// <summary>
        /// Byte offset within the GCZ file where the compressed block data begins.
        /// Computed as: HeaderSize + (NumBlocks * 8) + (NumBlocks * 4).
        /// </summary>
        public long DataOffset => HeaderSize + (NumBlocks * 8) + (NumBlocks * 4);

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GCZ(DiscImage model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public GCZ(DiscImage model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GCZ(DiscImage model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public GCZ(DiscImage model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public GCZ(DiscImage model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GCZ(DiscImage model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a GCZ wrapper from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the GCZ image</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A GCZ wrapper on success, null on failure</returns>
        public static GCZ? Create(byte[]? data, int offset)
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
        /// Create a GCZ wrapper from a Stream
        /// </summary>
        /// <param name="data">Stream representing the GCZ image</param>
        /// <returns>A GCZ wrapper on success, null on failure</returns>
        public static GCZ? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                long currentOffset = data.Position;

                var model = new Serialization.Readers.GCZ().Deserialize(data);
                if (model is null)
                    return null;

                return new GCZ(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Header

        /// <summary>
        /// Decompresses just the first block of the GCZ image to read the disc header,
        /// without decompressing the entire image.
        /// </summary>
        private DiscHeader? ReadDiscHeader()
        {
            if (BlockPointers is null || BlockPointers.Length == 0)
                return null;

            ulong pointer = BlockPointers[0];

            ulong nextRaw = BlockPointers.Length > 1
                ? BlockPointers[1] & ~UncompressedFlag
                : Header.CompressedDataSize;

            int compSize = (int)(nextRaw - (pointer & ~UncompressedFlag));
            if (compSize <= 0)
                return null;

            long blockFileOffset = DataOffset + (long)(pointer & ~UncompressedFlag);
            byte[] raw = ReadRangeFromSource(blockFileOffset, compSize);
            if (raw.Length != compSize)
                return null;

            bool uncompressed = (pointer & UncompressedFlag) != 0;
            if (uncompressed)
            {
                var disc = new Serialization.Readers.NintendoDisc().Deserialize(raw, offset: 0);
                return disc?.Header;
            }
            else
            {
                if (raw.Length < 6)
                    return null;

                byte[] block;
                try
                {
                    using var cs = new MemoryStream(raw, 2, raw.Length - 6);
                    using var ds = new DeflateStream(cs, CompressionMode.Decompress);
                    using var os = new MemoryStream();

                    ds.BlockCopy(os, blockSize: 4096);

                    block = os.ToArray();
                }
                catch
                {
                    return null;
                }

                var disc = new Serialization.Readers.NintendoDisc().Deserialize(block, offset: 0);
                return disc?.Header;
            }
        }

        #endregion
    }
}
