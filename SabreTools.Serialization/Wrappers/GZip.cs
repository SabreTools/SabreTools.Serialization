using System.IO;
using SabreTools.Data.Models.GZIP;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class GZip : WrapperBase<Archive>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "gzip Archive";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Content CRC-32 as stored in the extra field
        /// </summary>
        /// <remarks>Only guaranteed for Torrent GZip format</remarks>
        public byte[]? ContentCrc32
        {
            get
            {
                // Only valid for Torrent GZip
                if (!IsTorrentGZip)
                    return null;

                // CRC-32 is the second packed field
                int extraIndex = 0x10;
                return Header.ExtraFieldBytes.ReadBytes(ref extraIndex, 0x04);
            }
        }

        /// <summary>
        /// Content MD5 as stored in the extra field
        /// </summary>
        /// <remarks>Only guaranteed for Torrent GZip format</remarks>
        public byte[]? ContentMd5
        {
            get
            {
                // Only valid for Torrent GZip
                if (!IsTorrentGZip)
                    return null;

                // MD5 is the first packed field
                int extraIndex = 0x00;
                return Header.ExtraFieldBytes.ReadBytes(ref extraIndex, 0x10);
            }
        }

        /// <summary>
        /// Content size as stored in the extra field
        /// </summary>
        /// <remarks>Only guaranteed for Torrent GZip format</remarks>
        public ulong ContentSize
        {
            get
            {
                // Only valid for Torrent GZip
                if (!IsTorrentGZip)
                    return 0;

                // MD5 is the first packed field
                int extraIndex = 0x00;
                return Header.ExtraFieldBytes.ReadUInt64LittleEndian(ref extraIndex);
            }
        }

        /// <summary>
        /// Offset to the compressed data
        /// </summary>
        /// <remarks>Returns -1 on error</remarks>
        public long DataOffset
        {
            get
            {
                if (field > -1)
                    return field;

                // Minimum offset is 10 bytes:
                // - ID1 (1)
                // - ID2 (1)
                // - CompressionMethod (1)
                // - Flags (1)
                // - LastModifiedTime (4)
                // - ExtraFlags (1)
                // - OperatingSystem (1)
                field = 10;

                // Add extra lengths
                field += Header.ExtraLength;
                if (Header.OriginalFileName != null)
                    field += Header.OriginalFileName.Length + 1;
                if (Header.FileComment != null)
                    field += Header.FileComment.Length + 1;
                if (Header.CRC16 != null)
                    field += 2;

                return field;
            }
        } = -1;

        /// <inheritdoc cref="Archive.Header"/>
        public Header Header => Model.Header;

        /// <summary>
        /// Indicates if the archive is in the standard
        /// "Torrent GZip" format. This format is used by
        /// some programs to store extended hashes in the
        /// header while maintaining the format otherwise.
        /// </summary>
        public bool IsTorrentGZip
        {
            get
            {
                // Torrent GZip uses normal deflate, not GZIP deflate
                if (Header.CompressionMethod != CompressionMethod.Deflate)
                    return false;

                // Only the extra field should be present
                if (Header.Flags != Flags.FEXTRA)
                    return false;

                // The modification should be 0x00000000, but some implementations
                // do not set this correctly, so it is skipped.

                // No extra flags are set
                if (Header.ExtraFlags != 0x00)
                    return false;

                // The OS should be FAT, regardless of the original platform, but
                // some implementations do not set this correctly, so it is skipped.

                // The extra field is non-standard, using the following format:
                // - 0x00-0x0F - MD5 hash of the internal file
                // - 0x10-0x13 - CRC-32 checksum of the internal file
                // - 0x14-0x1B - Little-endian file size of the internal file
                if (Header.ExtraLength != 0x1C)
                    return false;
                if (Header.ExtraFieldBytes == null || Header.ExtraFieldBytes.Length != 0x1C)
                    return false;

                return true;
            }
        }

        /// <inheritdoc cref="Archive.Trailer"/>
        public Trailer Trailer => Model.Trailer;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public GZip(Archive model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public GZip(Archive model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GZip(Archive model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public GZip(Archive model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public GZip(Archive model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public GZip(Archive model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a GZip archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A GZip wrapper on success, null on failure</returns>
        public static GZip? Create(byte[]? data, int offset)
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
        /// Create a GZip archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A GZip wrapper on success, null on failure</returns>
        public static GZip? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.GZip().Deserialize(data);
                if (model == null)
                    return null;

                return new GZip(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
