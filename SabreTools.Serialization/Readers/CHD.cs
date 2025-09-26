using System;
using System.IO;
using System.Text;
using SabreTools.Data.Models.CHD;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
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
                // Cache the current offset
                long initialOffset = data.Position;

                // Determine the header version
                uint version = GetVersion(data, initialOffset);

                // Read and return the current CHD
                switch (version)
                {
                    case 1:
                        var headerV1 = ParseHeaderV1(data);

                        if (headerV1.Tag != Constants.SignatureString)
                            return null;
                        if (headerV1.Length != Constants.HeaderV1Size)
                            return null;
                        if (headerV1.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                            return null;

                        return headerV1;

                    case 2:
                        var headerV2 = ParseHeaderV2(data);

                        if (headerV2.Tag != Constants.SignatureString)
                            return null;
                        if (headerV2.Length != Constants.HeaderV2Size)
                            return null;
                        if (headerV2.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                            return null;

                        return headerV2;

                    case 3:
                        var headerV3 = ParseHeaderV3(data);

                        if (headerV3.Tag != Constants.SignatureString)
                            return null;
                        if (headerV3.Length != Constants.HeaderV3Size)
                            return null;
                        if (headerV3.Compression > CompressionType.CHDCOMPRESSION_ZLIB_PLUS)
                            return null;

                        return headerV3;

                    case 4:
                        var headerV4 = ParseHeaderV4(data);

                        if (headerV4.Tag != Constants.SignatureString)
                            return null;
                        if (headerV4.Length != Constants.HeaderV4Size)
                            return null;
                        if (headerV4.Compression > CompressionType.CHDCOMPRESSION_AV)
                            return null;

                        return headerV4;

                    case 5:
                        var headerV5 = ParseHeaderV5(data);

                        if (headerV5.Tag != Constants.SignatureString)
                            return null;
                        if (headerV5.Length != Constants.HeaderV5Size)
                            return null;

                        return headerV5;

                    default:
                        return null;

                }
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
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <returns>Matching version, 0 if none</returns>
        private static uint GetVersion(Stream data, long initialOffset)
        {
            // Read the header values
            byte[] tagBytes = data.ReadBytes(8);
            string tag = Encoding.ASCII.GetString(tagBytes);
            uint length = data.ReadUInt32BigEndian();
            uint version = data.ReadUInt32BigEndian();

            // Seek back to start
            data.SeekIfPossible(initialOffset);

            // Check the signature
            if (!string.Equals(tag, Constants.SignatureString, StringComparison.Ordinal))
                return 0;

            // Match the version to header length
#if NET472_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
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
        /// Parse a Stream into a HeaderV1
        /// </summary>
        public static HeaderV1 ParseHeaderV1(Stream data)
        {
            var obj = new HeaderV1();

            byte[] tag = data.ReadBytes(8);
            obj.Tag = Encoding.ASCII.GetString(tag);
            obj.Length = data.ReadUInt32BigEndian();
            obj.Version = data.ReadUInt32BigEndian();
            obj.Flags = (Flags)data.ReadUInt32BigEndian();
            obj.Compression = (CompressionType)data.ReadUInt32BigEndian();
            obj.HunkSize = data.ReadUInt32BigEndian();
            obj.TotalHunks = data.ReadUInt32BigEndian();
            obj.Cylinders = data.ReadUInt32BigEndian();
            obj.Heads = data.ReadUInt32BigEndian();
            obj.Sectors = data.ReadUInt32BigEndian();
            obj.MD5 = data.ReadBytes(16);
            obj.ParentMD5 = data.ReadBytes(16);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a V2 header
        /// </summary>
        public static HeaderV2 ParseHeaderV2(Stream data)
        {
            var obj = new HeaderV2();

            byte[] tag = data.ReadBytes(8);
            obj.Tag = Encoding.ASCII.GetString(tag);
            obj.Length = data.ReadUInt32BigEndian();
            obj.Version = data.ReadUInt32BigEndian();
            obj.Flags = (Flags)data.ReadUInt32BigEndian();
            obj.Compression = (CompressionType)data.ReadUInt32BigEndian();
            obj.HunkSize = data.ReadUInt32BigEndian();
            obj.TotalHunks = data.ReadUInt32BigEndian();
            obj.Cylinders = data.ReadUInt32BigEndian();
            obj.Heads = data.ReadUInt32BigEndian();
            obj.Sectors = data.ReadUInt32BigEndian();
            obj.MD5 = data.ReadBytes(16);
            obj.ParentMD5 = data.ReadBytes(16);
            obj.BytesPerSector = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a V3 header
        /// </summary>
        public static HeaderV3 ParseHeaderV3(Stream data)
        {
            var obj = new HeaderV3();

            byte[] tag = data.ReadBytes(8);
            obj.Tag = Encoding.ASCII.GetString(tag);
            obj.Length = data.ReadUInt32BigEndian();
            obj.Version = data.ReadUInt32BigEndian();
            obj.Flags = (Flags)data.ReadUInt32BigEndian();
            obj.Compression = (CompressionType)data.ReadUInt32BigEndian();
            obj.TotalHunks = data.ReadUInt32BigEndian();
            obj.LogicalBytes = data.ReadUInt64BigEndian();
            obj.MetaOffset = data.ReadUInt64BigEndian();
            obj.MD5 = data.ReadBytes(16);
            obj.ParentMD5 = data.ReadBytes(16);
            obj.HunkBytes = data.ReadUInt32BigEndian();
            obj.SHA1 = data.ReadBytes(20);
            obj.ParentSHA1 = data.ReadBytes(20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a V4 header
        /// </summary>
        public static HeaderV4 ParseHeaderV4(Stream data)
        {
            var obj = new HeaderV4();

            byte[] tag = data.ReadBytes(8);
            obj.Tag = Encoding.ASCII.GetString(tag);
            obj.Length = data.ReadUInt32BigEndian();
            obj.Version = data.ReadUInt32BigEndian();
            obj.Flags = (Flags)data.ReadUInt32BigEndian();
            obj.Compression = (CompressionType)data.ReadUInt32BigEndian();
            obj.TotalHunks = data.ReadUInt32BigEndian();
            obj.LogicalBytes = data.ReadUInt64BigEndian();
            obj.MetaOffset = data.ReadUInt64BigEndian();
            obj.HunkBytes = data.ReadUInt32BigEndian();
            obj.SHA1 = data.ReadBytes(20);
            obj.ParentSHA1 = data.ReadBytes(20);
            obj.RawSHA1 = data.ReadBytes(20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a V5 header
        /// </summary>
        public static HeaderV5 ParseHeaderV5(Stream data)
        {
            var obj = new HeaderV5();

            byte[] tag = data.ReadBytes(8);
            obj.Tag = Encoding.ASCII.GetString(tag);
            obj.Length = data.ReadUInt32BigEndian();
            obj.Version = data.ReadUInt32BigEndian();
            obj.Compressors = new CodecType[4];
            for (int i = 0; i < 4; i++)
            {
                obj.Compressors[i] = (CodecType)data.ReadUInt32BigEndian();
            }
            obj.LogicalBytes = data.ReadUInt64BigEndian();
            obj.MapOffset = data.ReadUInt64BigEndian();
            obj.MetaOffset = data.ReadUInt64BigEndian();
            obj.HunkBytes = data.ReadUInt32BigEndian();
            obj.UnitBytes = data.ReadUInt32BigEndian();
            obj.RawSHA1 = data.ReadBytes(20);
            obj.SHA1 = data.ReadBytes(20);
            obj.ParentSHA1 = data.ReadBytes(20);

            return obj;
        }
    }
}
