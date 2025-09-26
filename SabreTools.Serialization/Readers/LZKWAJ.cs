using System.IO;
using System.Text;
using SabreTools.Data.Models.LZ;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.LZ.Constants;

namespace SabreTools.Serialization.Readers
{
    public class LZKWAJ : BaseBinaryReader<KWAJFile>
    {
        /// <inheritdoc/>
        public override KWAJFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                int initialOffset = (int)data.Position;

                // Create a new file to fill
                var file = new KWAJFile();

                #region File Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header == null)
                    return null;

                // Set the header
                file.Header = header;

                #endregion

                #region Extended Header

                if (header.HeaderFlags != 0)
                {
                    var extensions = new KWAJHeaderExtensions();

#if NET20 || NET35
                    if ((header.HeaderFlags & KWAJHeaderFlags.HasDecompressedLength) != 0)
                        extensions.DecompressedLength = data.ReadUInt32LittleEndian();
                    if ((header.HeaderFlags & KWAJHeaderFlags.HasUnknownFlag) != 0)
                        extensions.UnknownPurpose = data.ReadUInt16LittleEndian();
                    if ((header.HeaderFlags & KWAJHeaderFlags.HasPrefixedData) != 0)
                    {
                        extensions.UnknownDataLength = data.ReadUInt16LittleEndian();
                        extensions.UnknownData = data.ReadBytes((int)extensions.UnknownDataLength);
                    }
                    if ((header.HeaderFlags & KWAJHeaderFlags.HasFileName) != 0)
                        extensions.FileName = data.ReadNullTerminatedAnsiString();
                    if ((header.HeaderFlags & KWAJHeaderFlags.HasFileExtension) != 0)
                        extensions.FileExtension = data.ReadNullTerminatedAnsiString();
                    if ((header.HeaderFlags & KWAJHeaderFlags.HasPrefixedData) != 0)
                    {
                        extensions.ArbitraryTextLength = data.ReadUInt16LittleEndian();
                        extensions.ArbitraryText = data.ReadBytes((int)extensions.ArbitraryTextLength);
                    }
#else
                    if (header.HeaderFlags.HasFlag(KWAJHeaderFlags.HasDecompressedLength))
                        extensions.DecompressedLength = data.ReadUInt32LittleEndian();
                    if (header.HeaderFlags.HasFlag(KWAJHeaderFlags.HasUnknownFlag))
                        extensions.UnknownPurpose = data.ReadUInt16LittleEndian();
                    if (header.HeaderFlags.HasFlag(KWAJHeaderFlags.HasPrefixedData))
                    {
                        extensions.UnknownDataLength = data.ReadUInt16LittleEndian();
                        extensions.UnknownData = data.ReadBytes((int)extensions.UnknownDataLength);
                    }
                    if (header.HeaderFlags.HasFlag(KWAJHeaderFlags.HasFileName))
                        extensions.FileName = data.ReadNullTerminatedAnsiString();
                    if (header.HeaderFlags.HasFlag(KWAJHeaderFlags.HasFileExtension))
                        extensions.FileExtension = data.ReadNullTerminatedAnsiString();
                    if (header.HeaderFlags.HasFlag(KWAJHeaderFlags.HasPrefixedData))
                    {
                        extensions.ArbitraryTextLength = data.ReadUInt16LittleEndian();
                        extensions.ArbitraryText = data.ReadBytes((int)extensions.ArbitraryTextLength);
                    }
#endif

                    file.HeaderExtensions = extensions;
                }

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
        private static KWAJHeader? ParseHeader(Stream data)
        {
            var header = new KWAJHeader();

            header.Magic = data.ReadBytes(8);
            if (Encoding.ASCII.GetString(header.Magic) != Encoding.ASCII.GetString(KWAJSignatureBytes))
                return null;

            header.CompressionType = (KWAJCompressionType)data.ReadUInt16LittleEndian();
            if (header.CompressionType > KWAJCompressionType.MSZIP)
                return null;

            header.DataOffset = data.ReadUInt16LittleEndian();
            header.HeaderFlags = (KWAJHeaderFlags)data.ReadUInt16LittleEndian();

            return header;
        }
    }
}
