using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.N3DS;
using static SabreTools.Models.N3DS.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class N3DS : BaseBinaryDeserializer<Cart>
    {
        /// <inheritdoc/>
        public override Cart? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cart image to fill
            var cart = new Cart();

            #region NCSD Header

            // Try to parse the header
            var header = ParseNCSDHeader(data);
            if (header == null)
                return null;

            // Set the cart image header
            cart.Header = header;

            #endregion

            #region Card Info Header

            // Try to parse the card info header
            var cardInfoHeader = ParseCardInfoHeader(data);
            if (cardInfoHeader == null)
                return null;

            // Set the card info header
            cart.CardInfoHeader = cardInfoHeader;

            #endregion

            #region Development Card Info Header

            // Try to parse the development card info header
            var developmentCardInfoHeader = ParseDevelopmentCardInfoHeader(data);
            if (developmentCardInfoHeader == null)
                return null;

            // Set the development card info header
            cart.DevelopmentCardInfoHeader = developmentCardInfoHeader;

            #endregion

            #region Partitions

            // Create the partition table
            cart.Partitions = new NCCHHeader[8];

            // Iterate and build the partitions
            for (int i = 0; i < 8; i++)
            {
                cart.Partitions[i] = ParseNCCHHeader(data);
            }

            #endregion

            // Cache the media unit size for further use
            long mediaUnitSize = 0;
            if (header.PartitionFlags != null)
                mediaUnitSize = (uint)(0x200 * Math.Pow(2, header.PartitionFlags[(int)NCSDFlags.MediaUnitSize]));

            #region Extended Headers

            // Create the extended header table
            cart.ExtendedHeaders = new NCCHExtendedHeader[8];

            // Iterate and build the extended headers
            for (int i = 0; i < 8; i++)
            {
                // If we have an encrypted or invalid partition
                if (cart.Partitions[i]!.MagicID != NCCHMagicNumber)
                    continue;

                // If we have no partitions table
                if (cart.Header!.PartitionsTable == null)
                    continue;

                // Get the extended header offset
                long offset = (cart.Header.PartitionsTable[i]!.Offset * mediaUnitSize) + 0x200;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the extended header
                data.Seek(offset, SeekOrigin.Begin);

                // Parse the extended header
                var extendedHeader = ParseNCCHExtendedHeader(data);
                if (extendedHeader != null)
                    cart.ExtendedHeaders[i] = extendedHeader;
            }

            #endregion

            #region ExeFS Headers

            // Create the ExeFS header table
            cart.ExeFSHeaders = new ExeFSHeader[8];

            // Iterate and build the ExeFS headers
            for (int i = 0; i < 8; i++)
            {
                // If we have an encrypted or invalid partition
                if (cart.Partitions[i]!.MagicID != NCCHMagicNumber)
                    continue;

                // If we have no partitions table
                if (cart.Header!.PartitionsTable == null)
                    continue;

                // Get the ExeFS header offset
                long offset = (cart.Header.PartitionsTable[i]!.Offset + cart.Partitions[i]!.ExeFSOffsetInMediaUnits) * mediaUnitSize;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the ExeFS header
                data.Seek(offset, SeekOrigin.Begin);

                // Parse the ExeFS header
                var exeFsHeader = ParseExeFSHeader(data);
                if (exeFsHeader == null)
                    return null;

                cart.ExeFSHeaders[i] = exeFsHeader;
            }

            #endregion

            #region RomFS Headers

            // Create the RomFS header table
            cart.RomFSHeaders = new RomFSHeader[8];

            // Iterate and build the RomFS headers
            for (int i = 0; i < 8; i++)
            {
                // If we have an encrypted or invalid partition
                if (cart.Partitions[i]!.MagicID != NCCHMagicNumber)
                    continue;

                // If we have no partitions table
                if (cart.Header!.PartitionsTable == null)
                    continue;

                // Get the RomFS header offset
                long offset = (cart.Header.PartitionsTable[i]!.Offset + cart.Partitions[i]!.RomFSOffsetInMediaUnits) * mediaUnitSize;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the RomFS header
                data.Seek(offset, SeekOrigin.Begin);

                // Parse the RomFS header
                var romFsHeader = ParseRomFSHeader(data);
                if (romFsHeader != null)
                    cart.RomFSHeaders[i] = romFsHeader;
            }

            #endregion

            return cart;
        }

        /// <summary>
        /// Parse a Stream into an NCSD header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCSD header on success, null on error</returns>
        public static NCSDHeader? ParseNCSDHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var header = new NCSDHeader();

            header.RSA2048Signature = data.ReadBytes(0x100);
            byte[]? magicNumber = data.ReadBytes(4);
            if (magicNumber == null)
                return null;

            header.MagicNumber = Encoding.ASCII.GetString(magicNumber).TrimEnd('\0'); ;
            if (header.MagicNumber != NCSDMagicNumber)
                return null;

            header.ImageSizeInMediaUnits = data.ReadUInt32();
            header.MediaId = data.ReadBytes(8);
            header.PartitionsFSType = (FilesystemType)data.ReadUInt64();
            header.PartitionsCryptType = data.ReadBytes(8);

            header.PartitionsTable = new PartitionTableEntry[8];
            for (int i = 0; i < 8; i++)
            {
                var partitionTableEntry = ParsePartitionTableEntry(data);
                if (partitionTableEntry == null)
                    return null;

                header.PartitionsTable[i] = partitionTableEntry;
            }

            if (header.PartitionsFSType == FilesystemType.Normal || header.PartitionsFSType == FilesystemType.None)
            {
                header.ExheaderHash = data.ReadBytes(0x20);
                header.AdditionalHeaderSize = data.ReadUInt32();
                header.SectorZeroOffset = data.ReadUInt32();
                header.PartitionFlags = data.ReadBytes(8);

                header.PartitionIdTable = new ulong[8];
                for (int i = 0; i < 8; i++)
                {
                    header.PartitionIdTable[i] = data.ReadUInt64();
                }

                header.Reserved1 = data.ReadBytes(0x20);
                header.Reserved2 = data.ReadBytes(0x0E);
                header.FirmUpdateByte1 = data.ReadByteValue();
                header.FirmUpdateByte2 = data.ReadByteValue();
            }
            else if (header.PartitionsFSType == FilesystemType.FIRM)
            {
                header.Unknown = data.ReadBytes(0x5E);
                header.EncryptedMBR = data.ReadBytes(0x42);
            }

            return header;
        }

        /// <summary>
        /// Parse a Stream into a partition table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled partition table entry on success, null on error</returns>
        public static PartitionTableEntry? ParsePartitionTableEntry(Stream data)
        {
            return data.ReadType<PartitionTableEntry>();
        }

        /// <summary>
        /// Parse a Stream into a card info header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled card info header on success, null on error</returns>
        public static CardInfoHeader? ParseCardInfoHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var header = new CardInfoHeader();

            header.WritableAddressMediaUnits = data.ReadUInt32();
            header.CardInfoBitmask = data.ReadUInt32();
            header.Reserved3 = data.ReadBytes(0x108);
            header.TitleVersion = data.ReadUInt16();
            header.CardRevision = data.ReadUInt16();
            header.Reserved4 = data.ReadBytes(0xCD6);
            header.InitialData = ParseInitialData(data);

            return header;
        }

        /// <summary>
        /// Parse a Stream into a development card info header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled development card info header on success, null on error</returns>
        public static DevelopmentCardInfoHeader? ParseDevelopmentCardInfoHeader(Stream data)
        {
            return data.ReadType<DevelopmentCardInfoHeader>();
        }

        /// <summary>
        /// Parse a Stream into initial data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled initial data on success, null on error</returns>
        public static InitialData? ParseInitialData(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var id = new InitialData();

            id.CardSeedKeyY = data.ReadBytes(0x10);
            id.EncryptedCardSeed = data.ReadBytes(0x10);
            id.CardSeedAESMAC = data.ReadBytes(0x10);
            id.CardSeedNonce = data.ReadBytes(0x0C);
            id.Reserved = data.ReadBytes(0xC4);
            id.BackupHeader = ParseNCCHHeader(data, skipSignature: true);

            return id;
        }

        /// <summary>
        /// Parse a Stream into an NCCH header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="skipSignature">Indicates if the signature should be skipped</param>
        /// <returns>Filled NCCH header on success, null on error</returns>
        public static NCCHHeader ParseNCCHHeader(Stream data, bool skipSignature = false)
        {
            // TODO: Use marshalling here instead of building
            var header = new NCCHHeader();

            if (!skipSignature)
                header.RSA2048Signature = data.ReadBytes(0x100);

            byte[]? magicId = data.ReadBytes(4);
            if (magicId != null)
                header.MagicID = Encoding.ASCII.GetString(magicId).TrimEnd('\0');
            header.ContentSizeInMediaUnits = data.ReadUInt32();
            header.PartitionId = data.ReadUInt64();
            header.MakerCode = data.ReadUInt16();
            header.Version = data.ReadUInt16();
            header.VerificationHash = data.ReadUInt32();
            header.ProgramId = data.ReadBytes(8);
            header.Reserved1 = data.ReadBytes(0x10);
            header.LogoRegionHash = data.ReadBytes(0x20);
            byte[]? productCode = data.ReadBytes(0x10);
            if (productCode != null)
                header.ProductCode = Encoding.ASCII.GetString(productCode).TrimEnd('\0');
            header.ExtendedHeaderHash = data.ReadBytes(0x20);
            header.ExtendedHeaderSizeInBytes = data.ReadUInt32();
            header.Reserved2 = data.ReadUInt32();
            header.Flags = ParseNCCHHeaderFlags(data);
            header.PlainRegionOffsetInMediaUnits = data.ReadUInt32();
            header.PlainRegionSizeInMediaUnits = data.ReadUInt32();
            header.LogoRegionOffsetInMediaUnits = data.ReadUInt32();
            header.LogoRegionSizeInMediaUnits = data.ReadUInt32();
            header.ExeFSOffsetInMediaUnits = data.ReadUInt32();
            header.ExeFSSizeInMediaUnits = data.ReadUInt32();
            header.ExeFSHashRegionSizeInMediaUnits = data.ReadUInt32();
            header.Reserved3 = data.ReadUInt32();
            header.RomFSOffsetInMediaUnits = data.ReadUInt32();
            header.RomFSSizeInMediaUnits = data.ReadUInt32();
            header.RomFSHashRegionSizeInMediaUnits = data.ReadUInt32();
            header.Reserved4 = data.ReadUInt32();
            header.ExeFSSuperblockHash = data.ReadBytes(0x20);
            header.RomFSSuperblockHash = data.ReadBytes(0x20);

            return header;
        }

        /// <summary>
        /// Parse a Stream into an NCCH header flags
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCH header flags on success, null on error</returns>
        public static NCCHHeaderFlags? ParseNCCHHeaderFlags(Stream data)
        {
            return data.ReadType<NCCHHeaderFlags>();
        }

        /// <summary>
        /// Parse a Stream into an NCCH extended header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCH extended header on success, null on error</returns>
        public static NCCHExtendedHeader? ParseNCCHExtendedHeader(Stream data)
        {
            return data.ReadType<NCCHExtendedHeader>();
        }

        /// <summary>
        /// Parse a Stream into an ExeFS header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFS header on success, null on error</returns>
        public static ExeFSHeader? ParseExeFSHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var exeFSHeader = new ExeFSHeader();

            exeFSHeader.FileHeaders = new ExeFSFileHeader[10];
            for (int i = 0; i < 10; i++)
            {
                var exeFsFileHeader = ParseExeFSFileHeader(data);
                if (exeFsFileHeader == null)
                    return null;

                exeFSHeader.FileHeaders[i] = exeFsFileHeader;
            }
            exeFSHeader.Reserved = data.ReadBytes(0x20);
            exeFSHeader.FileHashes = new byte[10][];
            for (int i = 0; i < 10; i++)
            {
                exeFSHeader.FileHashes[i] = data.ReadBytes(0x20) ?? [];
            }

            return exeFSHeader;
        }

        /// <summary>
        /// Parse a Stream into an ExeFS file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFS file header on success, null on error</returns>
        public static ExeFSFileHeader? ParseExeFSFileHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var exeFSFileHeader = new ExeFSFileHeader();

            byte[]? fileName = data.ReadBytes(8);
            if (fileName != null)
                exeFSFileHeader.FileName = Encoding.ASCII.GetString(fileName).TrimEnd('\0');
            exeFSFileHeader.FileOffset = data.ReadUInt32();
            exeFSFileHeader.FileSize = data.ReadUInt32();

            return exeFSFileHeader;
        }

        /// <summary>
        /// Parse a Stream into an RomFS header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled RomFS header on success, null on error</returns>
        public static RomFSHeader? ParseRomFSHeader(Stream data)
        {
            var romFSHeader = data.ReadType<RomFSHeader>();

            if (romFSHeader == null)
                return null;
            if (romFSHeader.MagicString != RomFSMagicNumber)
                return null;
            if (romFSHeader.MagicNumber != RomFSSecondMagicNumber)
                return null;

            return romFSHeader;
        }
    }
}