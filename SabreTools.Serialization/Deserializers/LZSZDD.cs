using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.LZ;
using static SabreTools.Models.LZ.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class LZSZDD : BaseBinaryDeserializer<SZDDFile>
    {
        /// <inheritdoc/>
        public override SZDDFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                int initialOffset = (int)data.Position;

                // Create a new file to fill
                var file = new SZDDFile();

                #region File Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header == null)
                    return null;

                // Set the header
                file.Header = header;

                #endregion

                return file;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled header on success, null on error</returns>
        private static SZDDHeader? ParseHeader(Stream data)
        {
            var header = new SZDDHeader();

            header.Magic = data.ReadBytes(8);
            if (Encoding.ASCII.GetString(header.Magic) != Encoding.ASCII.GetString(SZDDSignatureBytes))
                return null;

            header.CompressionType = (ExpandCompressionType)data.ReadByteValue();
            if (header.CompressionType != ExpandCompressionType.A)
                return null;

            header.LastChar = (char)data.ReadByteValue();
            header.RealLength = data.ReadUInt32LittleEndian();

            return header;
        }
    }
}
