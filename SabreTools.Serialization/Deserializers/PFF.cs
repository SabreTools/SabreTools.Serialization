using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.PFF;
using static SabreTools.Models.PFF.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PFF : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new archive to fill
            var archive = new Archive();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the archive header
            archive.Header = header;

            #endregion

            #region Segments

            // Get the segments
            long offset = header.FileListOffset;
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the segments
            data.Seek(offset, SeekOrigin.Begin);

            // Create the segments array
            archive.Segments = new Segment[header.NumberOfFiles];

            // Read all segments in turn
            for (int i = 0; i < header.NumberOfFiles; i++)
            {
                var file = ParseSegment(data, header.FileSegmentSize);
                if (file == null)
                    continue;

                archive.Segments[i] = file;
            }

            #endregion

            #region Footer

            // Get the footer offset
            offset = header.FileListOffset + (header.FileSegmentSize * header.NumberOfFiles);
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the footer
            data.Seek(offset, SeekOrigin.Begin);

            // Try to parse the footer
            var footer = data.ReadType<Footer>();
            if (footer == null)
                return null;

            // Set the archive footer
            archive.Footer = footer;

            #endregion

            return archive;
        }

        /// <summary>
        /// Parse a Stream into a header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            var header = data.ReadType<Header>();
            return header?.Signature switch
            {
                Version0SignatureString when header.FileSegmentSize != Version0HSegmentSize => null,
                Version0SignatureString => header,

                Version2SignatureString when header.FileSegmentSize != Version2SegmentSize => null,
                Version2SignatureString => header,

                Version3SignatureString when header.FileSegmentSize != Version2SegmentSize
                                    && header.FileSegmentSize != Version3SegmentSize => null,
                Version3SignatureString => header,

                Version4SignatureString when header.FileSegmentSize != Version4SegmentSize => null,
                Version4SignatureString => header,

                _ => null,
            };
        }

        /// <summary>
        /// Parse a Stream into a file entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="segmentSize">PFF segment size</param>
        /// <returns>Filled file entry on success, null on error</returns>
        private static Segment ParseSegment(Stream data, uint segmentSize)
        {
            var segment = new Segment();

            segment.Deleted = data.ReadUInt32();
            segment.FileLocation = data.ReadUInt32();
            segment.FileSize = data.ReadUInt32();
            segment.PackedDate = data.ReadUInt32();
            byte[] fileName = data.ReadBytes(0x10);
            segment.FileName = Encoding.ASCII.GetString(fileName).TrimEnd('\0');
            if (segmentSize > Version2SegmentSize)
                segment.ModifiedDate = data.ReadUInt32();
            if (segmentSize > Version3SegmentSize)
                segment.CompressionLevel = data.ReadUInt32();

            return segment;
        }
    }
}