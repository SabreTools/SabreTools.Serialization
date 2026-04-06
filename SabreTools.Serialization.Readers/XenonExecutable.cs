using System.IO;
using SabreTools.Data.Models.XenonExecutable;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class XenonExecutable : BaseBinaryReader<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new executable to fill
                var xex = new Executable();

                #region ParseHeader

                // Parse the file header
                var header = ParseHeader(data, initialOffset);
                if (header is null)
                    return null;

                // Set the XEX header
                xex.Header = header;

                #endregion

                #region ParseCertificate

                // Get the certificate address
                long certificateOffset = initialOffset + header.CertificateOffset;
                if (certificateOffset >= initialOffset && certificateOffset < data.Length)
                {
                    // Seek to the certificate
                    data.SeekIfPossible(certificateOffset, SeekOrigin.Begin);

                    // Parse the certificate
                    var certificate = ParseCertificate(data);
                    if (certificate is null)
                        return null;

                    // Set the XEX certificate
                    xex.Certificate = certificate;
                }

                #endregion

                return xex;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an XEX Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XEX Header on success, null on error</returns>
        public static Header? ParseHeader(Stream data, long initialOffset)
        {
            var obj = new Header();

            var magicNumber = data.ReadBytes(4);
            if (!magicNumber.EqualsExactly(Constants.MagicBytes))
                return null;

            obj.MagicNumber = magicNumber;

            obj.ModuleFlags = data.ReadUInt32BigEndian();
            obj.PEDataOffset = data.ReadUInt32BigEndian();
            obj.Reserved = data.ReadUInt32BigEndian();
            obj.CertificateOffset = data.ReadUInt32BigEndian();
            obj.OptionalHeaderCount = data.ReadUInt32BigEndian();

            // Ensure optional headers fit within stream
            if (data.Position + (8 * obj.OptionalHeaderCount) > data.Length)
                return null;

            var optionalHeaders = new OptionalHeader[obj.OptionalHeaderCount];
            for (int i = 0; i < obj.OptionalHeaderCount; i++)
            {
                var optionalHeader = new OptionalHeader();
                optionalHeader.HeaderID = data.ReadUInt32BigEndian();
                optionalHeader.HeaderOffset = data.ReadUInt32BigEndian();

                // If HeaderID LSB is 0x00 or 0x01, HeaderOffset is the data itself
                if ((optionalHeader.HeaderID & 0xFE) == 0x00)
                {
                    optionalHeaders[i] = optionalHeader;
                    continue;
                }

                uint optionalHeaderLength = optionalHeader.HeaderID & 0xFF;
                if (optionalHeaderLength == 0xFF)
                    optionalHeaderLength = 4;
                else
                    optionalHeaderLength *= 4;

                // Ignore invalid offset
                if (optionalHeader.HeaderOffset < initialOffset || optionalHeader.HeaderOffset + optionalHeaderLength > data.Length)
                {
                    optionalHeaders[i] = optionalHeader;
                    continue;
                }

                // Read the optional header data
                long currentPosition = data.Position;
                data.SeekIfPossible(optionalHeader.HeaderOffset, SeekOrigin.Begin);

                // Deal with variable length optional header data
                if ((optionalHeader.HeaderID & 0xFF) == 0xFF)
                {
                    var length = data.ReadUInt32BigEndian();
                    optionalHeaderLength = length - 4;

                    // Ignore invalid length
                    if (optionalHeaderLength <= 0 || data.Position + optionalHeaderLength > data.Length)
                    {
                        data.SeekIfPossible(currentPosition, SeekOrigin.Begin);
                        optionalHeaders[i] = optionalHeader;
                        continue;
                    }
                }

                // Save the optional header data in model
                optionalHeader.HeaderData = data.ReadBytes((int)optionalHeaderLength);
                optionalHeaders[i] = optionalHeader;

                // Return to position in header
                data.SeekIfPossible(currentPosition, SeekOrigin.Begin);
            }

            obj.OptionalHeaders = optionalHeaders;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an XEX Certificate
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled XEX Certificate on success, null on error</returns>
        public static Certificate? ParseCertificate(Stream data)
        {
            var obj = new Certificate();

            // Ensure certificate fits within stream
            if (data.Position + 388 > data.Length)
                return null;

            obj.Length = data.ReadUInt32BigEndian();
            obj.ImageSize = data.ReadUInt32BigEndian();
            obj.Signature = data.ReadBytes(256);
            obj.BaseFileLoadAddress = data.ReadUInt32BigEndian();
            obj.ImageFlags = data.ReadUInt32BigEndian();
            obj.ImageBaseAddress = data.ReadUInt32BigEndian();
            obj.UnknownHash1 = data.ReadBytes(20);
            obj.Unknown0128 = data.ReadUInt32BigEndian();
            obj.UnknownHash2 = data.ReadBytes(20);
            obj.MediaID = data.ReadBytes(16);
            obj.XEXFileKey = data.ReadBytes(16);
            obj.Unknown0160 = data.ReadUInt32BigEndian();
            obj.UnknownHash3 = data.ReadBytes(20);
            obj.RegionFlags = data.ReadUInt32BigEndian();
            obj.AllowedMediaTypeFlags = data.ReadUInt32BigEndian();
            obj.TableCount = data.ReadUInt32BigEndian();

            // Ensure table fits within stream
            if (data.Position + (24 * obj.TableCount) > data.Length)
                return obj;

            var table = new TableEntry[obj.TableCount];
            for (int i = 0; i < obj.TableCount; i++)
            {
                var row = new TableEntry();
                row.ID = data.ReadUInt32BigEndian();
                row.Data = data.ReadBytes(20);
                table[i] = row;
            }

            obj.Table = table;

            return obj;
        }
    }
}
