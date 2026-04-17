using System.IO;
using SabreTools.Data.Models.GCZ;

namespace SabreTools.Wrappers
{
    public partial class GCZ : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "GCZ Compressed GameCube / Wii Disc Image";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Archive.Header"/>
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

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GCZ(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public GCZ(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GCZ(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public GCZ(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public GCZ(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GCZ(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

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
        /// Decompress the full GCZ image to a MemoryStream and return a NintendoDisc wrapper.
        /// Returns null if decompression fails or the decompressed data is not a valid disc image.
        /// </summary>
        public NintendoDisc? GetInnerWrapper()
        {
            // TODO: Implement block-by-block zlib decompression into a MemoryStream,
            // then pass to NintendoDisc.Create(). Each block entry in BlockPointers encodes
            // the offset and compression flag (top bit). Use DeflateStream to decompress
            // blocks where the flag is clear, or copy raw bytes where the flag is set.
            return null;
        }

        #endregion
    }
}
