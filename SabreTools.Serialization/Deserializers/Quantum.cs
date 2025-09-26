using System.IO;
using System.Text;
using SabreTools.Data.Models.Quantum;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.Quantum.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class Quantum : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new archive to fill
                var archive = new Archive();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Signature != SignatureString)
                    return null;

                // Set the archive header
                archive.Header = header;

                #endregion

                #region File List

                // If we have any files
                var fileDescriptors = new FileDescriptor[header.FileCount];

                // Read all entries in turn
                for (int i = 0; i < header.FileCount; i++)
                {
                    fileDescriptors[i] = ParseFileDescriptor(data, header.MinorVersion);
                }

                // Set the file list
                archive.FileList = fileDescriptors;

                #endregion

                // Cache the compressed data offset
                archive.CompressedDataOffset = data.Position;

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a FileDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="minorVersion">Minor version of the archive</param>
        /// <returns>Filled FileDescriptor on success, null on error</returns>
        public static FileDescriptor ParseFileDescriptor(Stream data, byte minorVersion)
        {
            var obj = new FileDescriptor();

            obj.FileNameSize = ReadVariableLength(data);
            if (obj.FileNameSize > 0)
            {
                byte[] fileName = data.ReadBytes(obj.FileNameSize);
                obj.FileName = Encoding.ASCII.GetString(fileName);
            }

            obj.CommentFieldSize = ReadVariableLength(data);
            if (obj.CommentFieldSize > 0)
            {
                byte[] commentField = data.ReadBytes(obj.CommentFieldSize);
                obj.CommentField = Encoding.ASCII.GetString(commentField);
            }

            obj.ExpandedFileSize = data.ReadUInt32LittleEndian();
            obj.FileTime = data.ReadUInt16LittleEndian();
            obj.FileDate = data.ReadUInt16LittleEndian();

            // Hack for unknown format data
            if (minorVersion == 22)
                obj.Unknown = data.ReadUInt16LittleEndian();

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

            byte[] signature = data.ReadBytes(2);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.MajorVersion = data.ReadByteValue();
            obj.MinorVersion = data.ReadByteValue();
            obj.FileCount = data.ReadUInt16LittleEndian();
            obj.TableSize = data.ReadByteValue();
            obj.CompressionFlags = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a variable-length size prefix
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Variable-length size prefix</returns>
        /// <remarks>
        /// Strings are prefixed with their length. If the length is less than 128
        /// then it is stored directly in one byte. If it is greater than 127 then
        /// the high bit of the first byte is set to 1 and the remaining fifteen bits
        /// contain the actual length in big-endian format.
        /// </remarks>
        private static int ReadVariableLength(Stream data)
        {
            byte b0 = data.ReadByteValue();
            if (b0 < 0x7F)
                return b0;

            b0 &= 0x7F;
            byte b1 = data.ReadByteValue();
            return (b0 << 8) | b1;
        }
    }
}
