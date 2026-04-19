using System.IO;
using SabreTools.Data.Models.GCZ;
using SabreTools.Data.Models.NintendoDisc;
#if !NET20 && !NET35
using System.IO.Compression;
#endif

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

        /// <summary>
        /// Total decompressed size of the disc image in bytes
        /// </summary>
        public ulong DataSize => Model.Header.DataSize;

        /// <summary>
        /// Number of compressed blocks in this image
        /// </summary>
        public uint NumBlocks => Model.Header.NumBlocks;

        /// <summary>
        /// Size of each uncompressed block in bytes
        /// </summary>
        public uint BlockSize => Model.Header.BlockSize;

        /// <summary>
        /// Block pointer table — top bit indicates uncompressed flag
        /// </summary>
        public ulong[] BlockPointers => Model.BlockPointers;

        /// <summary>
        /// Adler-32 hashes of each uncompressed block
        /// </summary>
        public uint[] BlockHashes => Model.BlockHashes;

        /// <summary>
        /// Disc header parsed by decompressing the first block of the GCZ image.
        /// </summary>
        public DiscHeader? DiscHeader
        {
            get
            {
                if (_discHeaderCached)
                    return _discHeader;
                _discHeader = ReadDiscHeader();
                _discHeaderCached = true;
                return _discHeader;
            }
        }

        private DiscHeader? _discHeader;
        private bool _discHeaderCached;

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

        #region Inner Wrapper

        /// <summary>
        /// Returns a NintendoDisc wrapper backed by a virtual stream that decompresses
        /// GCZ blocks on demand, avoiding loading the entire ISO into memory.
        /// </summary>
        public NintendoDisc? GetInnerWrapper()
        {
            if (Model.BlockPointers is null || Model.BlockPointers.Length == 0)
                return null;

            if (Model.Header.DataSize == 0)
                return null;

            var vStream = new GczVirtualStream(this);
            return NintendoDisc.Create(vStream);
        }

        /// <summary>
        /// Decompresses a single GCZ block by index and returns its raw bytes.
        /// Returns null on failure; returns a zero-filled block if the compressed size is zero.
        /// </summary>
        internal byte[]? DecompressBlock(int blockIndex)
        {
            const ulong UncompressedFlag = 0x8000000000000000UL;

            if (blockIndex < 0 || blockIndex >= Model.BlockPointers.Length)
                return null;

            ulong ptr = Model.BlockPointers[blockIndex];
            bool uncompressed = (ptr & UncompressedFlag) != 0;
            long blockFileOffset = Model.DataOffset + (long)(ptr & ~UncompressedFlag);

            ulong nextRaw = (blockIndex + 1 < Model.BlockPointers.Length)
                ? Model.BlockPointers[blockIndex + 1] & ~UncompressedFlag
                : Model.Header.CompressedDataSize;
            int compSize = (int)(nextRaw - (ptr & ~UncompressedFlag));

            if (compSize <= 0)
                return new byte[Model.Header.BlockSize];

            byte[] raw = ReadRangeFromSource(blockFileOffset, compSize);
            if (raw is null || raw.Length != compSize)
                return null;

            if (uncompressed)
                return raw;

            if (raw.Length < 6)
                return null;

#if NET20 || NET35
            return null;
#else
            try
            {
                using var cs = new MemoryStream(raw, 2, raw.Length - 6);
                using var ds = new DeflateStream(cs, CompressionMode.Decompress);
                using var os = new MemoryStream();
                ds.CopyTo(os);
                return os.ToArray();
            }
            catch
            {
                return null;
            }
#endif
        }

        /// <summary>
        /// Decompresses just the first block of the GCZ image to read the disc header,
        /// without decompressing the entire image.
        /// </summary>
        private DiscHeader? ReadDiscHeader()
        {
            const ulong UncompressedFlag = 0x8000000000000000UL;

            if (Model.BlockPointers is null || Model.BlockPointers.Length == 0)
                return null;

            ulong ptr = Model.BlockPointers[0];
            bool uncompressed = (ptr & UncompressedFlag) != 0;
            long blockFileOffset = Model.DataOffset + (long)(ptr & ~UncompressedFlag);

            ulong nextRaw = Model.BlockPointers.Length > 1
                ? Model.BlockPointers[1] & ~UncompressedFlag
                : Model.Header.CompressedDataSize;
            int compSize = (int)(nextRaw - (ptr & ~UncompressedFlag));

            if (compSize <= 0)
                return null;

            byte[] raw = ReadRangeFromSource(blockFileOffset, compSize);
            if (raw is null || raw.Length != compSize)
                return null;

            byte[] block;
            if (uncompressed)
            {
                block = raw;
            }
            else
            {
#if NET20 || NET35
                return null;
#else
                try
                {
                    using var cs = new MemoryStream(raw, 2, raw.Length - 6);
                    using var ds = new DeflateStream(cs, CompressionMode.Decompress);
                    using var os = new MemoryStream();
                    ds.CopyTo(os);
                    block = os.ToArray();
                }
                catch
                {
                    return null;
                }
#endif
            }

            using var ms = new MemoryStream(block);
            var disc = new Serialization.Readers.NintendoDisc().Deserialize(ms);
            return disc?.Header;
        }

        #endregion
    }
}
