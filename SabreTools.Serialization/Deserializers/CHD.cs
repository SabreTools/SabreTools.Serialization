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
            if (data == null || !data.CanRead)
                return null;

            try
            {
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
            catch
            {
                // Ignore the actual error
                return null;
            }
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
#if NET472_OR_GREATER || NETCOREAPP
            return (version, length) switch
            {
                (1, Constants.HeaderV1Size) => version,
                (2, Constants.HeaderV2Size) => version,
                (3, Constants.HeaderV3Size) => version,
                (4, Constants.HeaderV4Size) => version,
                (5, Constants.HeaderV5Size) => version,
                _ => 0,
            };
#else
            return version switch
            {
                1 => length == Constants.HeaderV1Size ? version : 0,
                2 => length == Constants.HeaderV2Size ? version : 0,
                3 => length == Constants.HeaderV3Size ? version : 0,
                4 => length == Constants.HeaderV4Size ? version : 0,
                5 => length == Constants.HeaderV5Size ? version : 0,
                _ => 0,
            };
#endif
        }

        /// <summary>
        /// Parse a Stream into a V1 header
        /// </summary>
        private static HeaderV1? ParseHeaderV1(Stream data)
        {
            var header = data.ReadType<HeaderV1>();
            if (header?.Tag != Constants.SignatureString)
                return null;
            if (header.Length != Constants.HeaderV1Size)
                return null;
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V2 header
        /// </summary>
        private static HeaderV2? ParseHeaderV2(Stream data)
        {
            var header = data.ReadType<HeaderV2>();
            if (header?.Tag != Constants.SignatureString)
                return null;
            if (header.Length != Constants.HeaderV2Size)
                return null;
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V3 header
        /// </summary>
        private static HeaderV3? ParseHeaderV3(Stream data)
        {
            var header = data.ReadType<HeaderV3>();
            if (header?.Tag != Constants.SignatureString)
                return null;
            if (header.Length != Constants.HeaderV3Size)
                return null;
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB_PLUS)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V4 header
        /// </summary>
        private static HeaderV4? ParseHeaderV4(Stream data)
        {
            var header = data.ReadType<HeaderV4>();
            if (header?.Tag != Constants.SignatureString)
                return null;
            if (header.Length != Constants.HeaderV4Size)
                return null;
            if (header.Compression > CompressionType.CHDCOMPRESSION_AV)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a V5 header
        /// </summary>
        private static HeaderV5? ParseHeaderV5(Stream data)
        {
            var header = data.ReadType<HeaderV5>();
            if (header?.Tag != Constants.SignatureString)
                return null;
            if (header.Length != Constants.HeaderV5Size)
                return null;

            return header;
        }
    }
}