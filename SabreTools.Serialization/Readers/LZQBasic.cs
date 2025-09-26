using System.IO;
using System.Text;
using SabreTools.Data.Models.LZ;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.LZ.Constants;

namespace SabreTools.Serialization.Readers
{
    public class LZQBasic : BaseBinaryDeserializer<QBasicFile>
    {
        /// <inheritdoc/>
        public override QBasicFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                int initialOffset = (int)data.Position;

                // Create a new file to fill
                var file = new QBasicFile();

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
        private static QBasicHeader? ParseHeader(Stream data)
        {
            var header = new QBasicHeader();

            header.Magic = data.ReadBytes(8);
            if (Encoding.ASCII.GetString(header.Magic) != Encoding.ASCII.GetString(QBasicSignatureBytes))
                return null;

            header.RealLength = data.ReadUInt32LittleEndian();

            return header;
        }
    }
}
