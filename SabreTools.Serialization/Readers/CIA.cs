using System.IO;
using System.Text;
using SabreTools.Data.Models.N3DS;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class CIA : BaseBinaryDeserializer<Data.Models.N3DS.CIA>
    {
        /// <inheritdoc/>
        public override Data.Models.N3DS.CIA? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new CIA archive to fill
                var cia = new Data.Models.N3DS.CIA();

                #region CIA Header

                // Try to parse the header
                var header = ParseCIAHeader(data);
                if (header.CertificateChainSize > data.Length)
                    return null;
                if (header.TicketSize > data.Length)
                    return null;
                if (header.TMDFileSize > data.Length)
                    return null;
                if (header.MetaSize > data.Length)
                    return null;
                if ((long)header.ContentSize > data.Length)
                    return null;

                // Set the CIA archive header
                cia.Header = header;

                #endregion

                // Align to 64-byte boundary, if needed
                if (!data.AlignToBoundary(64))
                    return null;

                #region Certificate Chain

                // Create the certificate chain
                cia.CertificateChain = new Certificate[3];

                // Try to parse the certificates
                for (int i = 0; i < 3; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    cia.CertificateChain[i] = certificate;
                }

                #endregion

                // Align to 64-byte boundary, if needed
                if (!data.AlignToBoundary(64))
                    return null;

                #region Ticket

                // Try to parse the ticket
                var ticket = ParseTicket(data);
                if (ticket == null)
                    return null;

                // Set the ticket
                cia.Ticket = ticket;

                #endregion

                // Align to 64-byte boundary, if needed
                if (!data.AlignToBoundary(64))
                    return null;

                #region Title Metadata

                // Try to parse the title metadata
                var titleMetadata = ParseTitleMetadata(data);
                if (titleMetadata == null)
                    return null;

                // Set the title metadata
                cia.TMDFileData = titleMetadata;

                #endregion

                // Align to 64-byte boundary, if needed
                if (!data.AlignToBoundary(64))
                    return null;

                #region Content File Data

                // Create the partition table
                cia.Partitions = new NCCHHeader[8];

                // Iterate and build the partitions
                for (int i = 0; i < 8; i++)
                {
                    cia.Partitions[i] = N3DS.ParseNCCHHeader(data);
                }

                #endregion

                // Align to 64-byte boundary, if needed
                if (!data.AlignToBoundary(64))
                    return null;

                #region Meta Data

                // If we have a meta data
                if (header.MetaSize > 0)
                    cia.MetaData = ParseMetaData(data);

                #endregion

                return cia;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Certificate
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Certificate on success, null on error</returns>
        public static Certificate? ParseCertificate(Stream data)
        {
            var obj = new Certificate();

            obj.SignatureType = (SignatureType)data.ReadUInt32LittleEndian();
            switch (obj.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                case SignatureType.RSA_4096_SHA256:
                    obj.SignatureSize = 0x200;
                    obj.PaddingSize = 0x3C;
                    break;

                case SignatureType.RSA_2048_SHA1:
                case SignatureType.RSA_2048_SHA256:
                    obj.SignatureSize = 0x100;
                    obj.PaddingSize = 0x3C;
                    break;

                case SignatureType.ECDSA_SHA1:
                case SignatureType.ECDSA_SHA256:
                    obj.SignatureSize = 0x3C;
                    obj.PaddingSize = 0x40;
                    break;

                default:
                    return null;
            }

            obj.Signature = data.ReadBytes(obj.SignatureSize);
            obj.Padding = data.ReadBytes(obj.PaddingSize);
            byte[] issuer = data.ReadBytes(0x40);
            obj.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            obj.KeyType = (PublicKeyType)data.ReadUInt32LittleEndian();
            byte[] name = data.ReadBytes(0x40);
            obj.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            obj.ExpirationTime = data.ReadUInt32LittleEndian();

            switch (obj.KeyType)
            {
                case PublicKeyType.RSA_4096:
                    obj.RSAModulus = data.ReadBytes(0x200);
                    obj.RSAPublicExponent = data.ReadUInt32LittleEndian();
                    obj.RSAPadding = data.ReadBytes(0x34);
                    break;
                case PublicKeyType.RSA_2048:
                    obj.RSAModulus = data.ReadBytes(0x100);
                    obj.RSAPublicExponent = data.ReadUInt32LittleEndian();
                    obj.RSAPadding = data.ReadBytes(0x34);
                    break;
                case PublicKeyType.EllipticCurve:
                    obj.ECCPublicKey = data.ReadBytes(0x3C);
                    obj.ECCPadding = data.ReadBytes(0x3C);
                    break;
                default:
                    return null;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a CIAHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CIAHeader on success, null on error</returns>
        public static CIAHeader ParseCIAHeader(Stream data)
        {
            var obj = new CIAHeader();

            obj.HeaderSize = data.ReadUInt32LittleEndian();
            obj.Type = data.ReadUInt16LittleEndian();
            obj.Version = data.ReadUInt16LittleEndian();
            obj.CertificateChainSize = data.ReadUInt32LittleEndian();
            obj.TicketSize = data.ReadUInt32LittleEndian();
            obj.TMDFileSize = data.ReadUInt32LittleEndian();
            obj.MetaSize = data.ReadUInt32LittleEndian();
            obj.ContentSize = data.ReadUInt64LittleEndian();
            obj.ContentIndex = data.ReadBytes(0x2000);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ContentChunkRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ContentChunkRecord on success, null on error</returns>
        public static ContentChunkRecord ParseContentChunkRecord(Stream data)
        {
            var obj = new ContentChunkRecord();

            obj.ContentId = data.ReadUInt32LittleEndian();
            obj.ContentIndex = (ContentIndex)data.ReadUInt16LittleEndian();
            obj.ContentType = (TMDContentType)data.ReadUInt16LittleEndian();
            obj.ContentSize = data.ReadUInt64LittleEndian();
            obj.SHA256Hash = data.ReadBytes(0x20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ContentInfoRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ContentInfoRecord on success, null on error</returns>
        public static ContentInfoRecord ParseContentInfoRecord(Stream data)
        {
            var obj = new ContentInfoRecord();

            obj.ContentIndexOffset = data.ReadUInt16LittleEndian();
            obj.ContentCommandCount = data.ReadUInt16LittleEndian();
            obj.UnhashedContentRecordsSHA256Hash = data.ReadBytes(0x20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a MetaData
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled MetaData on success, null on error</returns>
        public static MetaData ParseMetaData(Stream data)
        {
            var obj = new MetaData();

            obj.TitleIDDependencyList = data.ReadBytes(0x180);
            obj.Reserved1 = data.ReadBytes(0x180);
            obj.CoreVersion = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadBytes(0xFC);
            obj.IconData = data.ReadBytes(0x36C0);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Ticket
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="fromCdn">Indicates if the ticket is from CDN</param>
        /// <returns>Filled Ticket on success, null on error</returns>
        public static Ticket? ParseTicket(Stream data, bool fromCdn = false)
        {
            var obj = new Ticket();

            obj.SignatureType = (SignatureType)data.ReadUInt32LittleEndian();
            switch (obj.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                case SignatureType.RSA_4096_SHA256:
                    obj.SignatureSize = 0x200;
                    obj.PaddingSize = 0x3C;
                    break;

                case SignatureType.RSA_2048_SHA1:
                case SignatureType.RSA_2048_SHA256:
                    obj.SignatureSize = 0x100;
                    obj.PaddingSize = 0x3C;
                    break;

                case SignatureType.ECDSA_SHA1:
                case SignatureType.ECDSA_SHA256:
                    obj.SignatureSize = 0x3C;
                    obj.PaddingSize = 0x40;
                    break;

                default:
                    return null;
            }

            obj.Signature = data.ReadBytes(obj.SignatureSize);
            obj.Padding = data.ReadBytes(obj.PaddingSize);
            byte[] issuer = data.ReadBytes(0x40);
            obj.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            obj.ECCPublicKey = data.ReadBytes(0x3C);
            obj.Version = data.ReadByteValue();
            obj.CaCrlVersion = data.ReadByteValue();
            obj.SignerCrlVersion = data.ReadByteValue();
            obj.TitleKey = data.ReadBytes(0x10);
            obj.Reserved1 = data.ReadByteValue();
            obj.TicketID = data.ReadUInt64LittleEndian();
            obj.ConsoleID = data.ReadUInt32LittleEndian();
            obj.TitleID = data.ReadUInt64LittleEndian();
            obj.Reserved2 = data.ReadBytes(2);
            obj.TicketTitleVersion = data.ReadUInt16LittleEndian();
            obj.Reserved3 = data.ReadBytes(8);
            obj.LicenseType = data.ReadByteValue();
            obj.CommonKeyYIndex = data.ReadByteValue();
            obj.Reserved4 = data.ReadBytes(0x2A);
            obj.eShopAccountID = data.ReadUInt32LittleEndian();
            obj.Reserved5 = data.ReadByteValue();
            obj.Audit = data.ReadByteValue();
            obj.Reserved6 = data.ReadBytes(0x42);
            obj.Limits = new uint[0x10];
            for (int i = 0; i < obj.Limits.Length; i++)
            {
                obj.Limits[i] = data.ReadUInt32LittleEndian();
            }

            // Seek to the content index size
            data.Seek(4, SeekOrigin.Current);

            // Read the size (big-endian)
            obj.ContentIndexSize = data.ReadUInt32BigEndian();

            // Seek back to the start of the content index
            data.Seek(-8, SeekOrigin.Current);

            obj.ContentIndex = data.ReadBytes((int)obj.ContentIndexSize);

            // Certificates only exist in standalone CETK files
            if (fromCdn)
            {
                obj.CertificateChain = new Certificate[2];
                for (int i = 0; i < 2; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    obj.CertificateChain[i] = certificate;
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a title metadata
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="fromCdn">Indicates if the ticket is from CDN</param>
        /// <returns>Filled title metadata on success, null on error</returns>
        public static TitleMetadata? ParseTitleMetadata(Stream data, bool fromCdn = false)
        {
            var obj = new TitleMetadata();

            obj.SignatureType = (SignatureType)data.ReadUInt32LittleEndian();
            switch (obj.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                case SignatureType.RSA_4096_SHA256:
                    obj.SignatureSize = 0x200;
                    obj.PaddingSize = 0x3C;
                    break;

                case SignatureType.RSA_2048_SHA1:
                case SignatureType.RSA_2048_SHA256:
                    obj.SignatureSize = 0x100;
                    obj.PaddingSize = 0x3C;
                    break;

                case SignatureType.ECDSA_SHA1:
                case SignatureType.ECDSA_SHA256:
                    obj.SignatureSize = 0x3C;
                    obj.PaddingSize = 0x40;
                    break;

                default:
                    return null;
            }

            obj.Signature = data.ReadBytes(obj.SignatureSize);
            obj.Padding1 = data.ReadBytes(obj.PaddingSize);
            byte[] issuer = data.ReadBytes(0x40);
            obj.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            obj.Version = data.ReadByteValue();
            obj.CaCrlVersion = data.ReadByteValue();
            obj.SignerCrlVersion = data.ReadByteValue();
            obj.Reserved1 = data.ReadByteValue();
            obj.SystemVersion = data.ReadUInt64LittleEndian();
            obj.TitleID = data.ReadUInt64LittleEndian();
            obj.TitleType = data.ReadUInt32LittleEndian();
            obj.GroupID = data.ReadUInt16LittleEndian();
            obj.SaveDataSize = data.ReadUInt32LittleEndian();
            obj.SRLPrivateSaveDataSize = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadBytes(4);
            obj.SRLFlag = data.ReadByteValue();
            obj.Reserved3 = data.ReadBytes(0x31);
            obj.AccessRights = data.ReadUInt32LittleEndian();
            obj.TitleVersion = data.ReadUInt16LittleEndian();
            obj.ContentCount = data.ReadUInt16BigEndian();
            obj.BootContent = data.ReadUInt16LittleEndian();
            obj.Padding2 = data.ReadBytes(2);
            obj.SHA256HashContentInfoRecords = data.ReadBytes(0x20);
            obj.ContentInfoRecords = new ContentInfoRecord[64];
            for (int i = 0; i < 64; i++)
            {
                var contentInfoRecord = ParseContentInfoRecord(data);
                obj.ContentInfoRecords[i] = contentInfoRecord;
            }
            obj.ContentChunkRecords = new ContentChunkRecord[obj.ContentCount];
            for (int i = 0; i < obj.ContentCount; i++)
            {
                var contentChunkRecord = ParseContentChunkRecord(data);
                obj.ContentChunkRecords[i] = contentChunkRecord;
            }

            // Certificates only exist in standalone TMD files
            if (fromCdn)
            {
                obj.CertificateChain = new Certificate[2];
                for (int i = 0; i < 2; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    obj.CertificateChain[i] = certificate;
                }
            }

            return obj;
        }
    }
}
