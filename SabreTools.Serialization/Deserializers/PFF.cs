using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Models.PFF;
using static SabreTools.Serialization.Models.PFF.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PFF : BaseBinaryDeserializer<Archive>
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
                if (header.Signature == Version0SignatureString)
                {
                    if (header.FileSegmentSize != Version0HSegmentSize)
                        return null;
                }
                else if (header.Signature == Version2SignatureString)
                {
                    if (header.FileSegmentSize != Version2SegmentSize)
                        return null;
                }
                else if (header.Signature == Version3SignatureString)
                {
                    if (header.FileSegmentSize != Version2SegmentSize
                        && header.FileSegmentSize != Version3SegmentSize)
                    {
                        return null;
                    }
                }
                else if (header.Signature == Version4SignatureString)
                {
                    if (header.FileSegmentSize != Version4SegmentSize)
                        return null;
                }
                else
                {
                    return null;
                }

                // Set the archive header
                archive.Header = header;

                #endregion

                #region Segments

                // Get the segments
                long offset = initialOffset + header.FileListOffset;
                if (offset < initialOffset || offset >= data.Length)
                    return null;

                // Seek to the segments
                data.Seek(offset, SeekOrigin.Begin);

                // Create the segments array
                archive.Segments = new Segment[header.NumberOfFiles];

                // Read all segments in turn
                for (int i = 0; i < header.NumberOfFiles; i++)
                {
                    archive.Segments[i] = ParseSegment(data, header.FileSegmentSize);
                }

                #endregion

                #region Footer

                // Get the footer offset
                offset = initialOffset + header.FileListOffset + (header.FileSegmentSize * header.NumberOfFiles);
                if (offset < initialOffset || offset >= data.Length)
                    return null;

                // Seek to the footer
                data.Seek(offset, SeekOrigin.Begin);

                // Set the archive footer
                archive.Footer = ParseFooter(data);

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
        /// Parse a Stream into a Footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Footer on success, null on error</returns>
        public static Footer ParseFooter(Stream data)
        {
            var obj = new Footer();

            obj.SystemIP = data.ReadUInt32LittleEndian();
            obj.Reserved = data.ReadUInt32LittleEndian();
            byte[] kingTag = data.ReadBytes(4);
            obj.KingTag = Encoding.ASCII.GetString(kingTag);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.HeaderSize = data.ReadUInt32LittleEndian();
            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.NumberOfFiles = data.ReadUInt32LittleEndian();
            obj.FileSegmentSize = data.ReadUInt32LittleEndian();
            obj.FileListOffset = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Segment
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="segmentSize">PFF segment size</param>
        /// <returns>Filled Segment on success, null on error</returns>
        public static Segment ParseSegment(Stream data, uint segmentSize)
        {
            var obj = new Segment();

            obj.Deleted = data.ReadUInt32LittleEndian();
            obj.FileLocation = data.ReadUInt32LittleEndian();
            obj.FileSize = data.ReadUInt32LittleEndian();
            obj.PackedDate = data.ReadUInt32LittleEndian();
            byte[] fileName = data.ReadBytes(0x10);
            obj.FileName = Encoding.ASCII.GetString(fileName).TrimEnd('\0');
            if (segmentSize > Version2SegmentSize)
                obj.ModifiedDate = data.ReadUInt32LittleEndian();
            if (segmentSize > Version3SegmentSize)
                obj.CompressionLevel = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
