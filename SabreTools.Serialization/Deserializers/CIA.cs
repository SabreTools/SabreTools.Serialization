using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.N3DS;

namespace SabreTools.Serialization.Deserializers
{
    public class CIA : BaseBinaryDeserializer<Models.N3DS.CIA>
    {
        /// <inheritdoc/>
        public override Models.N3DS.CIA? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new CIA archive to fill
            var cia = new Models.N3DS.CIA();

            #region CIA Header

            // Try to parse the header
            var header = ParseCIAHeader(data);
            if (header == null)
                return null;

            // Set the CIA archive header
            cia.Header = header;

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

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
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Ticket

            // Try to parse the ticket
            var ticket = ParseTicket(data);
            if (ticket == null)
                return null;

            // Set the ticket
            cia.Ticket = ticket;

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Title Metadata

            // Try to parse the title metadata
            var titleMetadata = ParseTitleMetadata(data);
            if (titleMetadata == null)
                return null;

            // Set the title metadata
            cia.TMDFileData = titleMetadata;

            #endregion

            // Align to 64-byte boundary, if needed
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

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
            while (data.Position < data.Length - 1 && data.Position % 64 != 0)
            {
                _ = data.ReadByteValue();
            }

            #region Meta Data

            // If we have a meta data
            if (header.MetaSize > 0)
            {
                // Try to parse the meta
                var meta = ParseMetaData(data);
                if (meta == null)
                    return null;

                // Set the meta
                cia.MetaData = meta;
            }

            #endregion

            return cia;
        }

        /// <summary>
        /// Parse a Stream into a CIA header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CIA header on success, null on error</returns>
        public static CIAHeader? ParseCIAHeader(Stream data)
        {
            return data.ReadType<CIAHeader>();
        }

        /// <summary>
        /// Parse a Stream into a certificate
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled certificate on success, null on error</returns>
        public static Certificate? ParseCertificate(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Certificate certificate = new Certificate();

            certificate.SignatureType = (SignatureType)data.ReadUInt32();
            switch (certificate.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                    certificate.SignatureSize = 0x200;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA1:
                    certificate.SignatureSize = 0x100;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA1:
                    certificate.SignatureSize = 0x3C;
                    certificate.PaddingSize = 0x40;
                    break;
                case SignatureType.RSA_4096_SHA256:
                    certificate.SignatureSize = 0x200;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA256:
                    certificate.SignatureSize = 0x100;
                    certificate.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA256:
                    certificate.SignatureSize = 0x3C;
                    certificate.PaddingSize = 0x40;
                    break;
                default:
                    return null;
            }

            certificate.Signature = data.ReadBytes(certificate.SignatureSize);
            certificate.Padding = data.ReadBytes(certificate.PaddingSize);
            byte[]? issuer = data.ReadBytes(0x40);
            if (issuer != null)
                certificate.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            certificate.KeyType = (PublicKeyType)data.ReadUInt32();
            byte[]? name = data.ReadBytes(0x40);
            if (name != null)
                certificate.Name = Encoding.ASCII.GetString(name).TrimEnd('\0');
            certificate.ExpirationTime = data.ReadUInt32();

            switch (certificate.KeyType)
            {
                case PublicKeyType.RSA_4096:
                    certificate.RSAModulus = data.ReadBytes(0x200);
                    certificate.RSAPublicExponent = data.ReadUInt32();
                    certificate.RSAPadding = data.ReadBytes(0x34);
                    break;
                case PublicKeyType.RSA_2048:
                    certificate.RSAModulus = data.ReadBytes(0x100);
                    certificate.RSAPublicExponent = data.ReadUInt32();
                    certificate.RSAPadding = data.ReadBytes(0x34);
                    break;
                case PublicKeyType.EllipticCurve:
                    certificate.ECCPublicKey = data.ReadBytes(0x3C);
                    certificate.ECCPadding = data.ReadBytes(0x3C);
                    break;
                default:
                    return null;
            }

            return certificate;
        }

        /// <summary>
        /// Parse a Stream into a ticket
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="fromCdn">Indicates if the ticket is from CDN</param>
        /// <returns>Filled ticket on success, null on error</returns>
        public static Ticket? ParseTicket(Stream data, bool fromCdn = false)
        {
            // TODO: Use marshalling here instead of building
            Ticket ticket = new Ticket();

            ticket.SignatureType = (SignatureType)data.ReadUInt32();
            switch (ticket.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                    ticket.SignatureSize = 0x200;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA1:
                    ticket.SignatureSize = 0x100;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA1:
                    ticket.SignatureSize = 0x3C;
                    ticket.PaddingSize = 0x40;
                    break;
                case SignatureType.RSA_4096_SHA256:
                    ticket.SignatureSize = 0x200;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA256:
                    ticket.SignatureSize = 0x100;
                    ticket.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA256:
                    ticket.SignatureSize = 0x3C;
                    ticket.PaddingSize = 0x40;
                    break;
                default:
                    return null;
            }

            ticket.Signature = data.ReadBytes(ticket.SignatureSize);
            ticket.Padding = data.ReadBytes(ticket.PaddingSize);
            byte[]? issuer = data.ReadBytes(0x40);
            if (issuer != null)
                ticket.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            ticket.ECCPublicKey = data.ReadBytes(0x3C);
            ticket.Version = data.ReadByteValue();
            ticket.CaCrlVersion = data.ReadByteValue();
            ticket.SignerCrlVersion = data.ReadByteValue();
            ticket.TitleKey = data.ReadBytes(0x10);
            ticket.Reserved1 = data.ReadByteValue();
            ticket.TicketID = data.ReadUInt64();
            ticket.ConsoleID = data.ReadUInt32();
            ticket.TitleID = data.ReadUInt64();
            ticket.Reserved2 = data.ReadBytes(2);
            ticket.TicketTitleVersion = data.ReadUInt16();
            ticket.Reserved3 = data.ReadBytes(8);
            ticket.LicenseType = data.ReadByteValue();
            ticket.CommonKeyYIndex = data.ReadByteValue();
            ticket.Reserved4 = data.ReadBytes(0x2A);
            ticket.eShopAccountID = data.ReadUInt32();
            ticket.Reserved5 = data.ReadByteValue();
            ticket.Audit = data.ReadByteValue();
            ticket.Reserved6 = data.ReadBytes(0x42);
            ticket.Limits = new uint[0x10];
            for (int i = 0; i < ticket.Limits.Length; i++)
            {
                ticket.Limits[i] = data.ReadUInt32();
            }

            // Seek to the content index size
            data.Seek(4, SeekOrigin.Current);

            // Read the size (big-endian)
            byte[]? contentIndexSize = data.ReadBytes(4);
            if (contentIndexSize != null)
            {
                Array.Reverse(contentIndexSize);
                ticket.ContentIndexSize = BitConverter.ToUInt32(contentIndexSize, 0);
            }

            // Seek back to the start of the content index
            data.Seek(-8, SeekOrigin.Current);

            ticket.ContentIndex = data.ReadBytes((int)ticket.ContentIndexSize);

            // Certificates only exist in standalone CETK files
            if (fromCdn)
            {
                ticket.CertificateChain = new Certificate[2];
                for (int i = 0; i < 2; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    ticket.CertificateChain[i] = certificate;
                }
            }

            return ticket;
        }

        /// <summary>
        /// Parse a Stream into a title metadata
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="fromCdn">Indicates if the ticket is from CDN</param>
        /// <returns>Filled title metadata on success, null on error</returns>
        public static TitleMetadata? ParseTitleMetadata(Stream data, bool fromCdn = false)
        {
            // TODO: Use marshalling here instead of building
            var titleMetadata = new TitleMetadata();

            titleMetadata.SignatureType = (SignatureType)data.ReadUInt32();
            switch (titleMetadata.SignatureType)
            {
                case SignatureType.RSA_4096_SHA1:
                    titleMetadata.SignatureSize = 0x200;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA1:
                    titleMetadata.SignatureSize = 0x100;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA1:
                    titleMetadata.SignatureSize = 0x3C;
                    titleMetadata.PaddingSize = 0x40;
                    break;
                case SignatureType.RSA_4096_SHA256:
                    titleMetadata.SignatureSize = 0x200;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.RSA_2048_SHA256:
                    titleMetadata.SignatureSize = 0x100;
                    titleMetadata.PaddingSize = 0x3C;
                    break;
                case SignatureType.ECDSA_SHA256:
                    titleMetadata.SignatureSize = 0x3C;
                    titleMetadata.PaddingSize = 0x40;
                    break;
                default:
                    return null;
            }

            titleMetadata.Signature = data.ReadBytes(titleMetadata.SignatureSize);
            titleMetadata.Padding1 = data.ReadBytes(titleMetadata.PaddingSize);
            byte[]? issuer = data.ReadBytes(0x40);
            if (issuer != null)
                titleMetadata.Issuer = Encoding.ASCII.GetString(issuer).TrimEnd('\0');
            titleMetadata.Version = data.ReadByteValue();
            titleMetadata.CaCrlVersion = data.ReadByteValue();
            titleMetadata.SignerCrlVersion = data.ReadByteValue();
            titleMetadata.Reserved1 = data.ReadByteValue();
            titleMetadata.SystemVersion = data.ReadUInt64();
            titleMetadata.TitleID = data.ReadUInt64();
            titleMetadata.TitleType = data.ReadUInt32();
            titleMetadata.GroupID = data.ReadUInt16();
            titleMetadata.SaveDataSize = data.ReadUInt32();
            titleMetadata.SRLPrivateSaveDataSize = data.ReadUInt32();
            titleMetadata.Reserved2 = data.ReadBytes(4);
            titleMetadata.SRLFlag = data.ReadByteValue();
            titleMetadata.Reserved3 = data.ReadBytes(0x31);
            titleMetadata.AccessRights = data.ReadUInt32();
            titleMetadata.TitleVersion = data.ReadUInt16();

            // Read the content count (big-endian)
            byte[]? contentCount = data.ReadBytes(2);
            if (contentCount != null)
            {
                Array.Reverse(contentCount);
                titleMetadata.ContentCount = BitConverter.ToUInt16(contentCount, 0);
            }

            titleMetadata.BootContent = data.ReadUInt16();
            titleMetadata.Padding2 = data.ReadBytes(2);
            titleMetadata.SHA256HashContentInfoRecords = data.ReadBytes(0x20);
            titleMetadata.ContentInfoRecords = new ContentInfoRecord[64];
            for (int i = 0; i < 64; i++)
            {
                var contentInfoRecord = ParseContentInfoRecord(data);
                if (contentInfoRecord == null)
                    return null;

                titleMetadata.ContentInfoRecords[i] = ParseContentInfoRecord(data);
            }
            titleMetadata.ContentChunkRecords = new ContentChunkRecord[titleMetadata.ContentCount];
            for (int i = 0; i < titleMetadata.ContentCount; i++)
            {
                var contentChunkRecord = ParseContentChunkRecord(data);
                if (contentChunkRecord == null)
                    return null;

                titleMetadata.ContentChunkRecords[i] = ParseContentChunkRecord(data);
            }

            // Certificates only exist in standalone TMD files
            if (fromCdn)
            {
                titleMetadata.CertificateChain = new Certificate[2];
                for (int i = 0; i < 2; i++)
                {
                    var certificate = ParseCertificate(data);
                    if (certificate == null)
                        return null;

                    titleMetadata.CertificateChain[i] = certificate;
                }
            }

            return titleMetadata;
        }

        /// <summary>
        /// Parse a Stream into a content info record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled content info record on success, null on error</returns>
        public static ContentInfoRecord? ParseContentInfoRecord(Stream data)
        {
            return data.ReadType<ContentInfoRecord>();
        }

        /// <summary>
        /// Parse a Stream into a content chunk record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled content chunk record on success, null on error</returns>
        public static ContentChunkRecord? ParseContentChunkRecord(Stream data)
        {
            return data.ReadType<ContentChunkRecord>();
        }

        /// <summary>
        /// Parse a Stream into a meta data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled meta data on success, null on error</returns>
        public static MetaData? ParseMetaData(Stream data)
        {
            return data.ReadType<MetaData>();
        }
    }
}