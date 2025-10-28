using System;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XZ;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.XZ.Constants;

namespace SabreTools.Serialization.Readers
{
    public class XZ : BaseBinaryReader<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new archive to fill
                var archive = new Archive();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (!header.Signature.EqualsExactly(HeaderSignatureBytes))
                    return null;

                // Set the stream header
                archive.Header = header;

                // Cache the current offset
                long endOfHeader = data.Position;

                #endregion

                #region Footer

                // Seek to the start of the footer
                data.SeekIfPossible(-12, SeekOrigin.End);

                // Cache the current offset
                long startOfFooter = data.Position;

                // Try to parse the footer
                var footer = ParseFooter(data);
                if (!footer.Signature.EqualsExactly(FooterSignatureBytes))
                    return null;

                // Set the footer
                archive.Footer = footer;

                #endregion

                #region Index

                // Seek to the start of the index
                long indexOffset = startOfFooter - ((footer.BackwardSize + 1) * 4);
                data.SeekIfPossible(indexOffset, SeekOrigin.Begin);

                // Try to parse the index
                var index = ParseIndex(data);
                if (index.IndexIndicator != 0x00)
                    return null;
                if (index.Records == null)
                    return null;

                // Set the index
                archive.Index = index;

                #endregion

                #region Blocks

                // Seek to the start of the blocks
                data.SeekIfPossible(endOfHeader, SeekOrigin.Begin);

                // Create the block array
                int blockCount = index.Records.Length;
                archive.Blocks = new Block[blockCount];

                // Try to parse the blocks
                for (int i = 0; i < archive.Blocks.Length; i++)
                {
                    // Get the record for this block
                    var record = index.Records[i];

                    // Try to parse the block
                    archive.Blocks[i] = ParseBlock(data, header.Flags, record.UnpaddedSize);
                }

                #endregion

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.Signature = data.ReadBytes(6);
            obj.Flags = (HeaderFlags)data.ReadUInt16LittleEndian();
            obj.Crc32 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="headerFlags">HeaderFlags to for determining the check value</param>
        /// <param name="unpaddedSize">Unpadded data size from the index</param>
        /// <returns>Filled Block on success, null on error</returns>
        public static Block ParseBlock(Stream data, HeaderFlags headerFlags, ulong unpaddedSize)
        {
            // Cache the current offset
            long currentOffset = data.Position;

            // Determine the size of the check field
            int checkSize = 0;
            if (headerFlags == HeaderFlags.Crc32)
                checkSize = 4;
            else if (headerFlags == HeaderFlags.Crc64)
                checkSize = 8;
            else if (headerFlags == HeaderFlags.Sha256)
                checkSize = 32;

            var obj = new Block();

            obj.HeaderSize = data.ReadByteValue();
            int realHeaderSize = (obj.HeaderSize + 1) * 4;
            obj.Flags = (BlockFlags)data.ReadByteValue();

#if NET20 || NET35
            if ((obj.Flags & BlockFlags.CompressedSize) != 0)
#else
            if (obj.Flags.HasFlag(BlockFlags.CompressedSize))
#endif
                obj.CompressedSize = ParseVariableLength(data);

#if NET20 || NET35
            if ((obj.Flags & BlockFlags.UncompressedSize) != 0)
#else
            if (obj.Flags.HasFlag(BlockFlags.UncompressedSize))
#endif
                obj.UncompressedSize = ParseVariableLength(data);

            // Determine the number of filters to read
            int filterCount = ((byte)obj.Flags & 0x03) + 1;

            // Try to parse the filters
            obj.FilterFlags = new FilterFlag[filterCount];
            for (int i = 0; i < obj.FilterFlags.Length; i++)
            {
                obj.FilterFlags[i] = ParseFilterFlag(data);
            }

            // Parse the padding as needed, adjusting for CRC size
            int paddingLength = realHeaderSize - (int)(data.Position - currentOffset) - 4;
            if (paddingLength >= 0)
                obj.HeaderPadding = data.ReadBytes(paddingLength);

            obj.Crc32 = data.ReadUInt32LittleEndian();

            // Determine the compressed size
            ulong compressedSize = obj.CompressedSize != 0
                ? obj.CompressedSize
                : unpaddedSize - (ulong)(realHeaderSize + checkSize);

            // TODO: How to handle large blocks?
            if ((int)compressedSize > 0)
                obj.CompressedData = data.ReadBytes((int)compressedSize);

            // Parse the padding as needed
            paddingLength = 4 - (int)(unpaddedSize % 4);
            if (paddingLength >= 0)
                obj.BlockPadding = data.ReadBytes(paddingLength);

            // Read the Check as needed
            obj.Check = data.ReadBytes(checkSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FilterFlag
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FilterFlag on success, null on error</returns>
        public static FilterFlag ParseFilterFlag(Stream data)
        {
            var obj = new FilterFlag();

            obj.FilterID = ParseVariableLength(data);
            obj.SizeOfProperties = ParseVariableLength(data);
            obj.Properties = data.ReadBytes((int)obj.SizeOfProperties);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Index
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Index on success, null on error</returns>
        public static Data.Models.XZ.Index ParseIndex(Stream data)
        {
            // Cache the current offset
            long currentOffset = data.Position;

            var obj = new Data.Models.XZ.Index();

            obj.IndexIndicator = data.ReadByteValue();
            obj.NumberOfRecords = ParseVariableLength(data);

            obj.Records = new Record[obj.NumberOfRecords];
            for (int i = 0; i < obj.Records.Length; i++)
            {
                obj.Records[i] = ParseRecord(data);
            }

            // Parse the padding as needed
            int paddingLength = 4 - (int)(data.Position - currentOffset) % 4;
            if (paddingLength >= 0)
                obj.Padding = data.ReadBytes(paddingLength);

            obj.Crc32 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Record on success, null on error</returns>
        public static Record ParseRecord(Stream data)
        {
            var obj = new Record();

            obj.UnpaddedSize = ParseVariableLength(data);
            obj.UncompressedSize = ParseVariableLength(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Footer on success, null on error</returns>
        public static Footer ParseFooter(Stream data)
        {
            var obj = new Footer();

            obj.Crc32 = data.ReadUInt32LittleEndian();
            obj.BackwardSize = data.ReadUInt32LittleEndian();
            obj.Flags = (HeaderFlags)data.ReadUInt16LittleEndian();
            obj.Signature = data.ReadBytes(2);

            return obj;
        }

        /// <summary>
        /// Parse a variable-length number from the stream
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Decoded variable-length value</returns>
        private static ulong ParseVariableLength(Stream data)
        {
            // Cache the current offset
            long currentOffset = data.Position;

            // Read up to 9 bytes for decoding
            int byteCount = (int)Math.Min(data.Length - data.Position, 9);
            byte[] encoded = data.ReadBytes(byteCount);

            // Attempt to decode the value
            ulong output = encoded.DecodeVariableLength(byteCount, out int length);

            // Seek the actual length processed and return
            data.SeekIfPossible(currentOffset + length, SeekOrigin.Begin);
            return output;
        }
    }
}
