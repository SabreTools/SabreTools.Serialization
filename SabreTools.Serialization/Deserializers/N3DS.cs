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
            if (data == null || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

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
            var developmentCardInfoHeader = data.ReadType<DevelopmentCardInfoHeader>();
            if (developmentCardInfoHeader == null)
                return null;

            // Set the development card info header
            cart.DevelopmentCardInfoHeader = developmentCardInfoHeader;

            #endregion

            // Cache the media unit size for further use
            long mediaUnitSize = 0;
            if (header.PartitionFlags != null)
                mediaUnitSize = (uint)(0x200 * Math.Pow(2, header.PartitionFlags[(int)NCSDFlags.MediaUnitSize]));

            #region Partitions

            // Create the tables
            cart.Partitions = new NCCHHeader[8];
            cart.ExtendedHeaders = new NCCHExtendedHeader?[8];
            cart.ExeFSHeaders = new ExeFSHeader?[8];
            cart.RomFSHeaders = new RomFSHeader?[8];

            // Iterate and build the partitions
            for (int i = 0; i < 8; i++)
            {
                // Find the offset to the partition
                long partitionOffset = cart.Header.PartitionsTable?[i]?.Offset ?? 0;
                partitionOffset *= mediaUnitSize;
                if (partitionOffset == 0)
                    continue;

                // Seek to the start of the partition
                data.Seek(partitionOffset, SeekOrigin.Begin);

                // Handle the normal header
                var partition = ParseNCCHHeader(data);
                if (partition == null || partition.MagicID != NCCHMagicNumber)
                    continue;

                // Set the normal header
                cart.Partitions[i] = partition;

                // Handle the extended header, if it exists
                if (partition.ExtendedHeaderSizeInBytes > 0)
                {
                    var extendedHeader = data.ReadType<NCCHExtendedHeader>();
                    if (extendedHeader != null)
                        cart.ExtendedHeaders[i] = extendedHeader;
                }

                // Handle the ExeFS, if it exists
                if (partition.ExeFSSizeInMediaUnits > 0)
                {
                    long offset = partition.ExeFSOffsetInMediaUnits * mediaUnitSize;
                    data.Seek(partitionOffset + offset, SeekOrigin.Begin);

                    var exeFsHeader = ParseExeFSHeader(data);
                    if (exeFsHeader == null)
                        return null;

                    cart.ExeFSHeaders[i] = exeFsHeader;
                }

                // Handle the RomFS, if it exists
                if (partition.RomFSSizeInMediaUnits > 0)
                {
                    long offset = partition.RomFSOffsetInMediaUnits * mediaUnitSize;
                    data.Seek(partitionOffset + offset, SeekOrigin.Begin);

                    var romFsHeader = data.ReadType<RomFSHeader>();
                    if (romFsHeader?.MagicString != RomFSMagicNumber)
                        continue;
                    if (romFsHeader?.MagicNumber != RomFSSecondMagicNumber)
                        continue;

                    cart.RomFSHeaders[i] = romFsHeader;
                }
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
            var header = new NCSDHeader();

            header.RSA2048Signature = data.ReadBytes(0x100);
            byte[] magicNumber = data.ReadBytes(4);
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
                var partitionTableEntry = data.ReadType<PartitionTableEntry>();
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
        /// Parse a Stream into a card info header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled card info header on success, null on error</returns>
        public static CardInfoHeader? ParseCardInfoHeader(Stream data)
        {
            var header = new CardInfoHeader();

            header.WritableAddressMediaUnits = data.ReadUInt32();
            header.CardInfoBitmask = data.ReadUInt32();
            header.Reserved1 = data.ReadBytes(0xF8);
            header.FilledSize = data.ReadUInt32();
            header.Reserved2 = data.ReadBytes(0x0C);
            header.TitleVersion = data.ReadUInt16();
            header.CardRevision = data.ReadUInt16();
            header.Reserved3 = data.ReadBytes(0x0C);
            header.CVerTitleID = data.ReadBytes(0x08);
            header.CVerVersionNumber = data.ReadUInt16();
            header.Reserved4 = data.ReadBytes(0xCD6);
            header.InitialData = ParseInitialData(data);

            return header;
        }

        /// <summary>
        /// Parse a Stream into initial data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled initial data on success, null on error</returns>
        public static InitialData? ParseInitialData(Stream data)
        {
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
            var header = new NCCHHeader();

            if (!skipSignature)
                header.RSA2048Signature = data.ReadBytes(0x100);

            byte[] magicId = data.ReadBytes(4);
            header.MagicID = Encoding.ASCII.GetString(magicId).TrimEnd('\0');
            header.ContentSizeInMediaUnits = data.ReadUInt32();
            header.PartitionId = data.ReadUInt64();
            header.MakerCode = data.ReadUInt16();
            header.Version = data.ReadUInt16();
            header.VerificationHash = data.ReadUInt32();
            header.ProgramId = data.ReadBytes(8);
            header.Reserved1 = data.ReadBytes(0x10);
            header.LogoRegionHash = data.ReadBytes(0x20);
            byte[] productCode = data.ReadBytes(0x10);
            header.ProductCode = Encoding.ASCII.GetString(productCode).TrimEnd('\0');
            header.ExtendedHeaderHash = data.ReadBytes(0x20);
            header.ExtendedHeaderSizeInBytes = data.ReadUInt32();
            header.Reserved2 = data.ReadUInt32();
            header.Flags = data.ReadType<NCCHHeaderFlags>();
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
        /// Parse a Stream into an ExeFS header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFS header on success, null on error</returns>
        public static ExeFSHeader? ParseExeFSHeader(Stream data)
        {
            var exeFSHeader = new ExeFSHeader();

            exeFSHeader.FileHeaders = new ExeFSFileHeader[10];
            for (int i = 0; i < 10; i++)
            {
                var exeFsFileHeader = data.ReadType<ExeFSFileHeader>();
                if (exeFsFileHeader == null)
                    return null;

                exeFsFileHeader.FileName = exeFsFileHeader.FileName?.TrimEnd('\0');
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
    }
}