using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Models.N3DS;
using static SabreTools.Serialization.Models.N3DS.Constants;

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

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new cart image to fill
                var cart = new Cart();

                #region NCSD Header

                // Try to parse the header
                var header = ParseNCSDHeader(data);
                if (header.MagicNumber != NCSDMagicNumber)
                    return null;

                // Set the cart image header
                cart.Header = header;

                #endregion

                #region Card Info Header

                // Set the card info header
                cart.CardInfoHeader = ParseCardInfoHeader(data);

                #endregion

                #region Development Card Info Header

                // Set the development card info header
                cart.DevelopmentCardInfoHeader = ParseDevelopmentCardInfoHeader(data);

                #endregion

                // Cache the media unit size for further use
                long mediaUnitSize = 0;
                if (header.PartitionFlags != null)
                    mediaUnitSize = (uint)(0x200 * Math.Pow(2, header.PartitionFlags[(int)NCSDFlags.MediaUnitSize]));

                #region Partitions

                // Create the tables
                cart.Partitions = new NCCHHeader[8];
                cart.ExtendedHeaders = new NCCHExtendedHeader[8];
                cart.ExeFSHeaders = new ExeFSHeader[8];
                cart.RomFSHeaders = new RomFSHeader[8];

                // Iterate and build the partitions
                for (int i = 0; i < 8; i++)
                {
                    // Find the offset to the partition
                    long partitionOffset = initialOffset + cart.Header.PartitionsTable?[i]?.Offset ?? 0;
                    partitionOffset *= mediaUnitSize;
                    if (partitionOffset <= initialOffset)
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
                        cart.ExtendedHeaders[i] = ParseNCCHExtendedHeader(data);

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

                        var romFsHeader = ParseRomFSHeader(data);
                        if (romFsHeader.MagicString != RomFSMagicNumber)
                            continue;
                        if (romFsHeader.MagicNumber != RomFSSecondMagicNumber)
                            continue;

                        cart.RomFSHeaders[i] = romFsHeader;
                    }
                }

                #endregion

                return cart;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a AccessControlInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled AccessControlInfo on success, null on error</returns>
        public static AccessControlInfo ParseAccessControlInfo(Stream data)
        {
            var obj = new AccessControlInfo();

            obj.ARM11LocalSystemCapabilities = ParseARM11LocalSystemCapabilities(data);
            obj.ARM11KernelCapabilities = ParseARM11KernelCapabilities(data);
            obj.ARM9AccessControl = ParseARM9AccessControl(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ARM9AccessControl
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ARM9AccessControl on success, null on error</returns>
        public static ARM9AccessControl ParseARM9AccessControl(Stream data)
        {
            var obj = new ARM9AccessControl();

            obj.Descriptors = new ARM9AccessControlDescriptors[15];
            for (int i = 0; i < 15; i++)
            {
                obj.Descriptors[i] = (ARM9AccessControlDescriptors)data.ReadByteValue();
            }
            obj.DescriptorVersion = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ARM11KernelCapabilities
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ARM11KernelCapabilities on success, null on error</returns>
        public static ARM11KernelCapabilities ParseARM11KernelCapabilities(Stream data)
        {
            var obj = new ARM11KernelCapabilities();

            obj.Descriptors = new uint[28];
            for (int i = 0; i < 28; i++)
            {
                obj.Descriptors[i] = data.ReadUInt32LittleEndian();
            }
            obj.Reserved = data.ReadBytes(0x10);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ARM11LocalSystemCapabilities
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ARM11LocalSystemCapabilities on success, null on error</returns>
        public static ARM11LocalSystemCapabilities ParseARM11LocalSystemCapabilities(Stream data)
        {
            var obj = new ARM11LocalSystemCapabilities();

            obj.ProgramID = data.ReadUInt64LittleEndian();
            obj.CoreVersion = data.ReadUInt32LittleEndian();
            obj.Flag1 = (ARM11LSCFlag1)data.ReadByteValue();
            obj.Flag2 = (ARM11LSCFlag2)data.ReadByteValue();
            obj.Flag0 = (ARM11LSCFlag0)data.ReadByteValue();
            obj.Priority = data.ReadByteValue();
            obj.ResourceLimitDescriptors = new ushort[16];
            for (int i = 0; i < 16; i++)
            {
                obj.ResourceLimitDescriptors[i] = data.ReadUInt16LittleEndian();
            }
            obj.StorageInfo = ParseStorageInfo(data);
            obj.ServiceAccessControl = new ulong[32];
            for (int i = 0; i < 32; i++)
            {
                obj.ServiceAccessControl[i] = data.ReadUInt64LittleEndian();
            }
            obj.ExtendedServiceAccessControl = new ulong[2];
            for (int i = 0; i < 2; i++)
            {
                obj.ExtendedServiceAccessControl[i] = data.ReadUInt64LittleEndian();
            }
            obj.Reserved = data.ReadBytes(0x0F);
            obj.ResourceLimitCategory = (ResourceLimitCategory)data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a CardInfoHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CardInfoHeader on success, null on error</returns>
        public static CardInfoHeader ParseCardInfoHeader(Stream data)
        {
            var obj = new CardInfoHeader();

            obj.WritableAddressMediaUnits = data.ReadUInt32LittleEndian();
            obj.CardInfoBitmask = data.ReadUInt32LittleEndian();
            obj.Reserved1 = data.ReadBytes(0xF8);
            obj.FilledSize = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadBytes(0x0C);
            obj.TitleVersion = data.ReadUInt16LittleEndian();
            obj.CardRevision = data.ReadUInt16LittleEndian();
            obj.Reserved3 = data.ReadBytes(0x0C);
            obj.CVerTitleID = data.ReadBytes(0x08);
            obj.CVerVersionNumber = data.ReadUInt16LittleEndian();
            obj.Reserved4 = data.ReadBytes(0xCD6);
            obj.InitialData = ParseInitialData(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a CodeSetInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CodeSetInfo on success, null on error</returns>
        public static CodeSetInfo ParseCodeSetInfo(Stream data)
        {
            var obj = new CodeSetInfo();

            obj.Address = data.ReadUInt32LittleEndian();
            obj.PhysicalRegionSizeInPages = data.ReadUInt32LittleEndian();
            obj.SizeInBytes = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DevelopmentCardInfoHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DevelopmentCardInfoHeader on success, null on error</returns>
        public static DevelopmentCardInfoHeader ParseDevelopmentCardInfoHeader(Stream data)
        {
            var obj = new DevelopmentCardInfoHeader();

            obj.CardDeviceReserved1 = data.ReadBytes(0x200);
            obj.TitleKey = data.ReadBytes(0x10);
            obj.CardDeviceReserved2 = data.ReadBytes(0x1BF0);
            obj.TestData = ParseTestData(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExeFSFileHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFSFileHeader on success, null on error</returns>
        public static ExeFSFileHeader ParseExeFSFileHeader(Stream data)
        {
            var obj = new ExeFSFileHeader();

            byte[] fileName = data.ReadBytes(8);
            obj.FileName = Encoding.ASCII.GetString(fileName).TrimEnd('\0');
            obj.FileOffset = data.ReadUInt32LittleEndian();
            obj.FileSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ExeFSHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExeFSHeader on success, null on error</returns>
        public static ExeFSHeader ParseExeFSHeader(Stream data)
        {
            var obj = new ExeFSHeader();

            obj.FileHeaders = new ExeFSFileHeader[10];
            for (int i = 0; i < 10; i++)
            {
                obj.FileHeaders[i] = ParseExeFSFileHeader(data);
            }
            obj.Reserved = data.ReadBytes(0x20);
            obj.FileHashes = new byte[10][];
            for (int i = 0; i < 10; i++)
            {
                obj.FileHashes[i] = data.ReadBytes(0x20);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into initial data
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled initial data on success, null on error</returns>
        public static InitialData? ParseInitialData(Stream data)
        {
            var obj = new InitialData();

            obj.CardSeedKeyY = data.ReadBytes(0x10);
            obj.EncryptedCardSeed = data.ReadBytes(0x10);
            obj.CardSeedAESMAC = data.ReadBytes(0x10);
            obj.CardSeedNonce = data.ReadBytes(0x0C);
            obj.Reserved = data.ReadBytes(0xC4);
            obj.BackupHeader = ParseNCCHHeader(data, skipSignature: true);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a NCCHExtendedHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCHExtendedHeader on success, null on error</returns>
        public static NCCHExtendedHeader ParseNCCHExtendedHeader(Stream data)
        {
            var obj = new NCCHExtendedHeader();

            obj.SCI = ParseSystemControlInfo(data);
            obj.ACI = ParseAccessControlInfo(data);
            obj.AccessDescSignature = data.ReadBytes(0x100);
            obj.NCCHHDRPublicKey = data.ReadBytes(0x100);
            obj.ACIForLimitations = ParseAccessControlInfo(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an NCCHHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="skipSignature">Indicates if the signature should be skipped</param>
        /// <returns>Filled NCCHHeader on success, null on error</returns>
        public static NCCHHeader ParseNCCHHeader(Stream data, bool skipSignature = false)
        {
            var obj = new NCCHHeader();

            if (!skipSignature)
                obj.RSA2048Signature = data.ReadBytes(0x100);

            byte[] magicId = data.ReadBytes(4);
            obj.MagicID = Encoding.ASCII.GetString(magicId).TrimEnd('\0');
            obj.ContentSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.PartitionId = data.ReadUInt64LittleEndian();
            obj.MakerCode = data.ReadUInt16LittleEndian();
            obj.Version = data.ReadUInt16LittleEndian();
            obj.VerificationHash = data.ReadUInt32LittleEndian();
            obj.ProgramId = data.ReadBytes(8);
            obj.Reserved1 = data.ReadBytes(0x10);
            obj.LogoRegionHash = data.ReadBytes(0x20);
            byte[] productCode = data.ReadBytes(0x10);
            obj.ProductCode = Encoding.ASCII.GetString(productCode).TrimEnd('\0');
            obj.ExtendedHeaderHash = data.ReadBytes(0x20);
            obj.ExtendedHeaderSizeInBytes = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadUInt32LittleEndian();
            obj.Flags = ParseNCCHHeaderFlags(data);
            obj.PlainRegionOffsetInMediaUnits = data.ReadUInt32LittleEndian();
            obj.PlainRegionSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.LogoRegionOffsetInMediaUnits = data.ReadUInt32LittleEndian();
            obj.LogoRegionSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.ExeFSOffsetInMediaUnits = data.ReadUInt32LittleEndian();
            obj.ExeFSSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.ExeFSHashRegionSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.Reserved3 = data.ReadUInt32LittleEndian();
            obj.RomFSOffsetInMediaUnits = data.ReadUInt32LittleEndian();
            obj.RomFSSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.RomFSHashRegionSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.Reserved4 = data.ReadUInt32LittleEndian();
            obj.ExeFSSuperblockHash = data.ReadBytes(0x20);
            obj.RomFSSuperblockHash = data.ReadBytes(0x20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an NCCHHeaderFlags
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCHHeaderFlags on success, null on error</returns>
        public static NCCHHeaderFlags ParseNCCHHeaderFlags(Stream data)
        {
            var obj = new NCCHHeaderFlags();

            obj.Reserved0 = data.ReadByteValue();
            obj.Reserved1 = data.ReadByteValue();
            obj.Reserved2 = data.ReadByteValue();
            obj.CryptoMethod = (CryptoMethod)data.ReadByteValue();
            obj.ContentPlatform = (ContentPlatform)data.ReadByteValue();
            obj.MediaPlatformIndex = (ContentType)data.ReadByteValue();
            obj.ContentUnitSize = data.ReadByteValue();
            obj.BitMasks = (BitMasks)data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an NCSDHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCSDHeader on success, null on error</returns>
        public static NCSDHeader ParseNCSDHeader(Stream data)
        {
            var obj = new NCSDHeader();

            obj.RSA2048Signature = data.ReadBytes(0x100);
            byte[] magicNumber = data.ReadBytes(4);
            obj.MagicNumber = Encoding.ASCII.GetString(magicNumber).TrimEnd('\0');
            obj.ImageSizeInMediaUnits = data.ReadUInt32LittleEndian();
            obj.MediaId = data.ReadBytes(8);
            obj.PartitionsFSType = (FilesystemType)data.ReadUInt64LittleEndian();
            obj.PartitionsCryptType = data.ReadBytes(8);

            obj.PartitionsTable = new PartitionTableEntry[8];
            for (int i = 0; i < 8; i++)
            {
                obj.PartitionsTable[i] = ParsePartitionTableEntry(data);
            }

            if (obj.PartitionsFSType == FilesystemType.Normal || obj.PartitionsFSType == FilesystemType.None)
            {
                obj.ExheaderHash = data.ReadBytes(0x20);
                obj.AdditionalHeaderSize = data.ReadUInt32LittleEndian();
                obj.SectorZeroOffset = data.ReadUInt32LittleEndian();
                obj.PartitionFlags = data.ReadBytes(8);

                obj.PartitionIdTable = new ulong[8];
                for (int i = 0; i < 8; i++)
                {
                    obj.PartitionIdTable[i] = data.ReadUInt64LittleEndian();
                }

                obj.Reserved1 = data.ReadBytes(0x20);
                obj.Reserved2 = data.ReadBytes(0x0E);
                obj.FirmUpdateByte1 = data.ReadByteValue();
                obj.FirmUpdateByte2 = data.ReadByteValue();
            }
            else if (obj.PartitionsFSType == FilesystemType.FIRM)
            {
                obj.Unknown = data.ReadBytes(0x5E);
                obj.EncryptedMBR = data.ReadBytes(0x42);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an PartitionTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled PartitionTableEntry on success, null on error</returns>
        public static PartitionTableEntry ParsePartitionTableEntry(Stream data)
        {
            var obj = new PartitionTableEntry();

            obj.Offset = data.ReadUInt32LittleEndian();
            obj.Length = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an RomFSHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled RomFSHeader on success, null on error</returns>
        public static RomFSHeader ParseRomFSHeader(Stream data)
        {
            var obj = new RomFSHeader();

            byte[] magicString = data.ReadBytes(4);
            obj.MagicString = Encoding.ASCII.GetString(magicString);
            obj.MagicNumber = data.ReadUInt32LittleEndian();
            obj.MasterHashSize = data.ReadUInt32LittleEndian();
            obj.Level1LogicalOffset = data.ReadUInt64LittleEndian();
            obj.Level1HashdataSize = data.ReadUInt64LittleEndian();
            obj.Level1BlockSizeLog2 = data.ReadUInt32LittleEndian();
            obj.Reserved1 = data.ReadUInt32LittleEndian();
            obj.Level2LogicalOffset = data.ReadUInt64LittleEndian();
            obj.Level2HashdataSize = data.ReadUInt64LittleEndian();
            obj.Level2BlockSizeLog2 = data.ReadUInt32LittleEndian();
            obj.Reserved2 = data.ReadUInt32LittleEndian();
            obj.Level3LogicalOffset = data.ReadUInt64LittleEndian();
            obj.Level3HashdataSize = data.ReadUInt64LittleEndian();
            obj.Level3BlockSizeLog2 = data.ReadUInt32LittleEndian();
            obj.Reserved3 = data.ReadUInt32LittleEndian();
            obj.Reserved4 = data.ReadUInt32LittleEndian();
            obj.OptionalInfoSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an StorageInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled StorageInfo on success, null on error</returns>
        public static StorageInfo ParseStorageInfo(Stream data)
        {
            var obj = new StorageInfo();

            obj.ExtdataID = data.ReadUInt64LittleEndian();
            obj.SystemSavedataIDs = data.ReadBytes(8);
            obj.StorageAccessibleUniqueIDs = data.ReadBytes(8);
            obj.FileSystemAccessInfo = data.ReadBytes(7);
            obj.OtherAttributes = (StorageInfoOtherAttributes)data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an SystemControlInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SystemControlInfo on success, null on error</returns>
        public static SystemControlInfo ParseSystemControlInfo(Stream data)
        {
            var obj = new SystemControlInfo();

            byte[] applicationTitle = data.ReadBytes(8);
            obj.ApplicationTitle = Encoding.ASCII.GetString(applicationTitle).TrimEnd('\0');
            obj.Reserved1 = data.ReadBytes(5);
            obj.Flag = data.ReadByteValue();
            obj.RemasterVersion = data.ReadUInt16LittleEndian();
            obj.TextCodeSetInfo = ParseCodeSetInfo(data);
            obj.StackSize = data.ReadUInt32LittleEndian();
            obj.ReadOnlyCodeSetInfo = ParseCodeSetInfo(data);
            obj.Reserved2 = data.ReadUInt32LittleEndian();
            obj.DataCodeSetInfo = ParseCodeSetInfo(data);
            obj.BSSSize = data.ReadUInt32LittleEndian();
            obj.DependencyModuleList = new ulong[48];
            for (int i = 0; i < 48; i++)
            {
                obj.DependencyModuleList[i] = data.ReadUInt64LittleEndian();
            }
            obj.SystemInfo = ParseSystemInfo(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an SystemInfo
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SystemInfo on success, null on error</returns>
        public static SystemInfo ParseSystemInfo(Stream data)
        {
            var obj = new SystemInfo();

            obj.SaveDataSize = data.ReadUInt64LittleEndian();
            obj.JumpID = data.ReadUInt64LittleEndian();
            obj.Reserved = data.ReadBytes(0x30);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an TestData
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled TestData on success, null on error</returns>
        public static TestData ParseTestData(Stream data)
        {
            var obj = new TestData();

            obj.Signature = data.ReadBytes(8);
            obj.AscendingByteSequence = data.ReadBytes(0x1F8);
            obj.DescendingByteSequence = data.ReadBytes(0x200);
            obj.Filled00 = data.ReadBytes(0x200);
            obj.FilledFF = data.ReadBytes(0x200);
            obj.Filled0F = data.ReadBytes(0x200);
            obj.FilledF0 = data.ReadBytes(0x200);
            obj.Filled55 = data.ReadBytes(0x200);
            obj.FilledAA = data.ReadBytes(0x1FF);
            obj.FinalByte = data.ReadByteValue();

            return obj;
        }
    }
}
