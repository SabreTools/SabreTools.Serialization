using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CHD;

namespace SabreTools.Serialization.Deserializers
{
    // TODO: Expand this to full CHD files eventually
    public class CHD : BaseBinaryDeserializer<Header>
    {
        /// <inheritdoc/>
        public override Header? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Determine the header version
            uint version = GetVersion(data);

            // Read and return the current CHD
            return version switch
            {
                1 => ParseHeaderV1(data),
                2 => ParseHeaderV2(data),
                3 => ParseHeaderV3(data),
                4 => ParseHeaderV4(data),
                5 => ParseHeaderV5(data),
                _ => null,
            };
        }

        /// <summary>
        /// Get the matching CHD version, if possible
        /// </summary>
        /// <returns>Matching version, 0 if none</returns>
        private static uint GetVersion(Stream data)
        {
            // Read the header values
            byte[] tagBytes = data.ReadBytes(8);
            string tag = Encoding.ASCII.GetString(tagBytes);
            uint length = data.ReadUInt32BigEndian();
            uint version = data.ReadUInt32BigEndian();

            // Seek back to start
            data.SeekIfPossible();

            // Check the signature
            if (!string.Equals(tag, Constants.SignatureString, StringComparison.Ordinal))
                return 0;

            // Match the version to header length
            return (version, length) switch
            {
                (1, Constants.HeaderV1Size) => version,
                (2, Constants.HeaderV2Size) => version,
                (3, Constants.HeaderV3Size) => version,
                (4, Constants.HeaderV4Size) => version,
                (5, Constants.HeaderV5Size) => version,
                _ => 0,
            };
        }

        /// <summary>
        /// Parse a Stream into a V1 header
        /// </summary>
        private static HeaderV1? ParseHeaderV1(Stream data)
        {
            var header = new HeaderV1();

            byte[] tagBytes = data.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = data.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV1Size)
                return null;

            header.Version = data.ReadUInt32BigEndian();
            header.Flags = (Flags)data.ReadUInt32BigEndian();
            header.Compression = (CompressionType)data.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            header.HunkSize = data.ReadUInt32BigEndian();
            header.TotalHunks = data.ReadUInt32BigEndian();
            header.Cylinders = data.ReadUInt32BigEndian();
            header.Heads = data.ReadUInt32BigEndian();
            header.Sectors = data.ReadUInt32BigEndian();
            header.MD5 = data.ReadBytes(16);
            header.ParentMD5 = data.ReadBytes(16);

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V2 header
        /// </summary>
        private static HeaderV2? ParseHeaderV2(Stream data)
        {
            var header = new HeaderV2();

            byte[] tagBytes = data.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = data.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV2Size)
                return null;

            header.Version = data.ReadUInt32BigEndian();
            header.Flags = (Flags)data.ReadUInt32BigEndian();
            header.Compression = (CompressionType)data.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            header.HunkSize = data.ReadUInt32BigEndian();
            header.TotalHunks = data.ReadUInt32BigEndian();
            header.Cylinders = data.ReadUInt32BigEndian();
            header.Heads = data.ReadUInt32BigEndian();
            header.Sectors = data.ReadUInt32BigEndian();
            header.MD5 = data.ReadBytes(16);
            header.ParentMD5 = data.ReadBytes(16);
            header.BytesPerSector = data.ReadUInt32BigEndian();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V3 header
        /// </summary>
        private static HeaderV3? ParseHeaderV3(Stream data)
        {
            var header = new HeaderV3();

            byte[] tagBytes = data.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = data.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV3Size)
                return null;

            header.Version = data.ReadUInt32BigEndian();
            header.Flags = (Flags)data.ReadUInt32BigEndian();
            header.Compression = (CompressionType)data.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB_PLUS)
                return null;

            header.TotalHunks = data.ReadUInt32BigEndian();
            header.LogicalBytes = data.ReadUInt64BigEndian();
            header.MetaOffset = data.ReadUInt64BigEndian();
            header.MD5 = data.ReadBytes(16);
            header.ParentMD5 = data.ReadBytes(16);
            header.HunkBytes = data.ReadUInt32BigEndian();
            header.SHA1 = data.ReadBytes(20);
            header.ParentSHA1 = data.ReadBytes(20);

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V4 header
        /// </summary>
        private static HeaderV4? ParseHeaderV4(Stream data)
        {
            var header = new HeaderV4();

            byte[] tagBytes = data.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = data.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV4Size)
                return null;

            header.Version = data.ReadUInt32BigEndian();
            header.Flags = (Flags)data.ReadUInt32BigEndian();
            header.Compression = (CompressionType)data.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_AV)
                return null;

            header.TotalHunks = data.ReadUInt32BigEndian();
            header.LogicalBytes = data.ReadUInt64BigEndian();
            header.MetaOffset = data.ReadUInt64BigEndian();
            header.HunkBytes = data.ReadUInt32BigEndian();
            header.SHA1 = data.ReadBytes(20);
            header.ParentSHA1 = data.ReadBytes(20);
            header.RawSHA1 = data.ReadBytes(20);

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V5 header
        /// </summary>
        private static HeaderV5? ParseHeaderV5(Stream data)
        {
            var header = new HeaderV5();

            byte[] tagBytes = data.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = data.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV5Size)
                return null;

            header.Version = data.ReadUInt32BigEndian();
            header.Compressors = new uint[4];
            for (int i = 0; i < header.Compressors.Length; i++)
            {
                header.Compressors[i] = data.ReadUInt32BigEndian();
            }

            header.LogicalBytes = data.ReadUInt64BigEndian();
            header.MapOffset = data.ReadUInt64BigEndian();
            header.MetaOffset = data.ReadUInt64BigEndian();
            header.HunkBytes = data.ReadUInt32BigEndian();
            header.UnitBytes = data.ReadUInt32BigEndian();
            header.RawSHA1 = data.ReadBytes(20);
            header.SHA1 = data.ReadBytes(20);
            header.ParentSHA1 = data.ReadBytes(20);

            return header;
        }
    }
}