using System;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.N3DS;

namespace SabreTools.Serialization.Wrappers
{
    public partial class N3DS : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("3DS Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
            Print(builder, Model.CardInfoHeader);
            Print(builder, Model.DevelopmentCardInfoHeader);
            Print(builder, Model.Partitions);
            Print(builder, Model.ExtendedHeaders);
            Print(builder, Model.ExeFSHeaders);
            Print(builder, Model.RomFSHeaders);
        }

        private static void Print(StringBuilder builder, NCSDHeader header)
        {
            builder.AppendLine("  NCSD Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.RSA2048Signature, "  RSA-2048 SHA-256 signature");
            builder.AppendLine(header.MagicNumber, "  Magic number");
            builder.AppendLine(header.ImageSizeInMediaUnits, "  Image size in media units");
            builder.AppendLine(header.MediaId, "  Media ID");
            builder.AppendLine($"  Partitions filesystem type: {header.PartitionsFSType} (0x{header.PartitionsFSType:X})");
            builder.AppendLine(header.PartitionsCryptType, "  Partitions crypt type");
            builder.AppendLine();

            builder.AppendLine("    Partition table:");
            builder.AppendLine("    -------------------------");
            if (header.PartitionsTable.Length == 0)
            {
                builder.AppendLine("    No partition table entries");
            }
            else
            {
                for (int i = 0; i < header.PartitionsTable.Length; i++)
                {
                    var entry = header.PartitionsTable[i];

                    builder.AppendLine($"    Partition table entry {i}");
                    builder.AppendLine(entry.Offset, "      Offset");
                    builder.AppendLine(entry.Length, "      Length");
                }
            }

            builder.AppendLine();

            // If we have a cart image
            if (header.PartitionsFSType == FilesystemType.Normal || header.PartitionsFSType == FilesystemType.None)
            {
                builder.AppendLine(header.ExheaderHash, "  Exheader SHA-256 hash");
                builder.AppendLine(header.AdditionalHeaderSize, "  Additional header size");
                builder.AppendLine(header.SectorZeroOffset, "  Sector zero offset");
                builder.AppendLine(header.PartitionFlags, "  Partition flags");
                builder.AppendLine();

                builder.AppendLine("    Partition ID table:");
                builder.AppendLine("    -------------------------");
                if (header.PartitionIdTable == null || header.PartitionIdTable.Length == 0)
                {
                    builder.AppendLine("    No partition ID table entries");
                }
                else
                {
                    for (int i = 0; i < header.PartitionIdTable.Length; i++)
                    {
                        builder.AppendLine(header.PartitionIdTable[i], $"    Partition {i} ID");
                    }
                }

                builder.AppendLine();

                builder.AppendLine(header.Reserved1, "  Reserved 1");
                builder.AppendLine(header.Reserved2, "  Reserved 2");
                builder.AppendLine(header.FirmUpdateByte1, "  Firmware update byte 1");
                builder.AppendLine(header.FirmUpdateByte2, "  Firmware update byte 2");
            }

            // If we have a firmware image
            else if (header.PartitionsFSType == FilesystemType.FIRM)
            {
                builder.AppendLine(header.Unknown, "  Unknown");
                builder.AppendLine(header.EncryptedMBR, "  Encrypted MBR");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, CardInfoHeader header)
        {
            builder.AppendLine("  Card Info Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.WritableAddressMediaUnits, "  Writable address in media units");
            builder.AppendLine(header.CardInfoBitmask, "  Card info bitmask");
            builder.AppendLine(header.Reserved1, "  Reserved 1");
            builder.AppendLine(header.FilledSize, "  Filled size of cartridge");
            builder.AppendLine(header.Reserved2, "  Reserved 2");
            builder.AppendLine(header.TitleVersion, "  Title version");
            builder.AppendLine(header.CardRevision, "  Card revision");
            builder.AppendLine(header.Reserved3, "  Reserved 3");
            builder.AppendLine(header.CVerTitleID, "  Title ID of CVer in included update partition");
            builder.AppendLine(header.CVerVersionNumber, "  Version number of CVer in included update partition");
            builder.AppendLine(header.Reserved4, "  Reserved 4");
            builder.AppendLine();

            Print(builder, header.InitialData);
        }

        private static void Print(StringBuilder builder, DevelopmentCardInfoHeader header)
        {
            builder.AppendLine("  Development Card Info Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.CardDeviceReserved1, "  Card device reserved 1");
            builder.AppendLine(header.TitleKey, "  Title key");
            builder.AppendLine(header.CardDeviceReserved2, "  Card device reserved 2");
            builder.AppendLine();

            builder.AppendLine("  Test Data:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.TestData.Signature, "  Signature");
            builder.AppendLine(header.TestData.AscendingByteSequence, "  Ascending byte sequence");
            builder.AppendLine(header.TestData.DescendingByteSequence, "  Descending byte sequence");
            builder.AppendLine(header.TestData.Filled00, "  Filled with 00");
            builder.AppendLine(header.TestData.FilledFF, "  Filled with FF");
            builder.AppendLine(header.TestData.Filled0F, "  Filled with 0F");
            builder.AppendLine(header.TestData.FilledF0, "  Filled with F0");
            builder.AppendLine(header.TestData.Filled55, "  Filled with 55");
            builder.AppendLine(header.TestData.FilledAA, "  Filled with AA");
            builder.AppendLine(header.TestData.FinalByte, "  Final byte");

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, InitialData id)
        {
            builder.AppendLine("    Initial Data Information:");
            builder.AppendLine("    -------------------------");
            builder.AppendLine(id.CardSeedKeyY, "    Card seed KeyY");
            builder.AppendLine(id.EncryptedCardSeed, "    Encrypted card seed");
            builder.AppendLine(id.CardSeedAESMAC, "    Card seed AES-MAC");
            builder.AppendLine(id.CardSeedNonce, "    Card seed nonce");
            builder.AppendLine(id.Reserved, "    Reserved");
            builder.AppendLine();

            PrintBackup(builder, id.BackupHeader);
        }

        private static void PrintBackup(StringBuilder builder, NCCHHeader header)
        {
            builder.AppendLine("      Backup NCCH Header Information:");
            builder.AppendLine("      -------------------------");
            if (header.MagicID == string.Empty)
            {
                builder.AppendLine("      Empty backup header, no data can be parsed");
                builder.AppendLine();
                return;
            }
            else if (header.MagicID != Constants.NCCHMagicNumber)
            {
                builder.AppendLine("      Unrecognized backup header, no data can be parsed");
                builder.AppendLine();
                return;
            }

            // Backup header omits RSA signature
            builder.AppendLine(header.MagicID, "      Magic ID");
            builder.AppendLine(header.ContentSizeInMediaUnits, "      Content size in media units");
            builder.AppendLine(header.PartitionId, "      Partition ID");
            builder.AppendLine(header.MakerCode, "      Maker code");
            builder.AppendLine(header.Version, "      Version");
            builder.AppendLine(header.VerificationHash, "      Verification hash");
            builder.AppendLine(header.ProgramId, "      Program ID");
            builder.AppendLine(header.Reserved1, "      Reserved 1");
            builder.AppendLine(header.LogoRegionHash, "      Logo region SHA-256 hash");
            builder.AppendLine(header.ProductCode, "      Product code");
            builder.AppendLine(header.ExtendedHeaderHash, "      Extended header SHA-256 hash");
            builder.AppendLine(header.ExtendedHeaderSizeInBytes, "      Extended header size in bytes");
            builder.AppendLine(header.Reserved2, "      Reserved 2");

            builder.AppendLine("      Flags:");
            builder.AppendLine(header.Flags.Reserved0, "        Reserved 0");
            builder.AppendLine(header.Flags.Reserved1, "        Reserved 1");
            builder.AppendLine(header.Flags.Reserved2, "        Reserved 2");
            builder.AppendLine($"        Crypto method: {header.Flags.CryptoMethod} (0x{header.Flags.CryptoMethod:X})");
            builder.AppendLine($"        Content platform: {header.Flags.ContentPlatform} (0x{header.Flags.ContentPlatform:X})");
            builder.AppendLine($"        Content type: {header.Flags.MediaPlatformIndex} (0x{header.Flags.MediaPlatformIndex:X})");
            builder.AppendLine(header.Flags.ContentUnitSize, "        Content unit size");
            builder.AppendLine($"        Bitmasks: {header.Flags.BitMasks} (0x{header.Flags.BitMasks:X})");

            builder.AppendLine(header.PlainRegionOffsetInMediaUnits, "      Plain region offset, in media units");
            builder.AppendLine(header.PlainRegionSizeInMediaUnits, "      Plain region size, in media units");
            builder.AppendLine(header.LogoRegionOffsetInMediaUnits, "      Logo region offset, in media units");
            builder.AppendLine(header.LogoRegionSizeInMediaUnits, "      Logo region size, in media units");
            builder.AppendLine(header.ExeFSOffsetInMediaUnits, "      ExeFS offset, in media units");
            builder.AppendLine(header.ExeFSSizeInMediaUnits, "      ExeFS size, in media units");
            builder.AppendLine(header.ExeFSHashRegionSizeInMediaUnits, "      ExeFS hash region size, in media units");
            builder.AppendLine(header.Reserved3, "      Reserved 3");
            builder.AppendLine(header.RomFSOffsetInMediaUnits, "      RomFS offset, in media units");
            builder.AppendLine(header.RomFSSizeInMediaUnits, "      RomFS size, in media units");
            builder.AppendLine(header.RomFSHashRegionSizeInMediaUnits, "      RomFS hash region size, in media units");
            builder.AppendLine(header.Reserved4, "      Reserved 4");
            builder.AppendLine(header.ExeFSSuperblockHash, "      ExeFS superblock SHA-256 hash");
            builder.AppendLine(header.RomFSSuperblockHash, "      RomFS superblock SHA-256 hash");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, NCCHHeader[] entries)
        {
            builder.AppendLine("  NCCH Partition Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
            {
                builder.AppendLine("  No NCCH partition headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  NCCH Partition Header {i}");
                if (entry.MagicID == string.Empty)
                {
                    builder.AppendLine("    Empty partition, no data can be parsed");
                    continue;
                }

                if (entry.MagicID != Constants.NCCHMagicNumber)
                {
                    builder.AppendLine("    Unrecognized partition data, no data can be parsed");
                    continue;
                }

                builder.AppendLine(entry.RSA2048Signature, "    RSA-2048 SHA-256 signature");
                builder.AppendLine(entry.MagicID, "    Magic ID");
                builder.AppendLine(entry.ContentSizeInMediaUnits, "    Content size in media units");
                builder.AppendLine(entry.PartitionId, "    Partition ID");
                builder.AppendLine(entry.MakerCode, "    Maker code");
                builder.AppendLine(entry.Version, "    Version");
                builder.AppendLine(entry.VerificationHash, "    Verification hash");
                builder.AppendLine(entry.ProgramId, "    Program ID");
                builder.AppendLine(entry.Reserved1, "    Reserved 1");
                builder.AppendLine(entry.LogoRegionHash, "    Logo region SHA-256 hash");
                builder.AppendLine(entry.ProductCode, "    Product code");
                builder.AppendLine(entry.ExtendedHeaderHash, "    Extended header SHA-256 hash");
                builder.AppendLine(entry.ExtendedHeaderSizeInBytes, "    Extended header size in bytes");
                builder.AppendLine(entry.Reserved2, "    Reserved 2");
                builder.AppendLine("    Flags:");
                if (entry.Flags == null)
                {
                    builder.AppendLine("      [NULL]");
                }
                else
                {
                    builder.AppendLine(entry.Flags.Reserved0, "      Reserved 0");
                    builder.AppendLine(entry.Flags.Reserved1, "      Reserved 1");
                    builder.AppendLine(entry.Flags.Reserved2, "      Reserved 2");
                    builder.AppendLine($"      Crypto method: {entry.Flags.CryptoMethod} (0x{entry.Flags.CryptoMethod:X})");
                    builder.AppendLine($"      Content platform: {entry.Flags.ContentPlatform} (0x{entry.Flags.ContentPlatform:X})");
                    builder.AppendLine($"      Content type: {entry.Flags.MediaPlatformIndex} (0x{entry.Flags.MediaPlatformIndex:X})");
                    builder.AppendLine(entry.Flags.ContentUnitSize, "      Content unit size");
                    builder.AppendLine($"      Bitmasks: {entry.Flags.BitMasks} (0x{entry.Flags.BitMasks:X})");
                }

                builder.AppendLine(entry.PlainRegionOffsetInMediaUnits, "    Plain region offset, in media units");
                builder.AppendLine(entry.PlainRegionSizeInMediaUnits, "    Plain region size, in media units");
                builder.AppendLine(entry.LogoRegionOffsetInMediaUnits, "    Logo region offset, in media units");
                builder.AppendLine(entry.LogoRegionSizeInMediaUnits, "    Logo region size, in media units");
                builder.AppendLine(entry.ExeFSOffsetInMediaUnits, "    ExeFS offset, in media units");
                builder.AppendLine(entry.ExeFSSizeInMediaUnits, "    ExeFS size, in media units");
                builder.AppendLine(entry.ExeFSHashRegionSizeInMediaUnits, "    ExeFS hash region size, in media units");
                builder.AppendLine(entry.Reserved3, "    Reserved 3");
                builder.AppendLine(entry.RomFSOffsetInMediaUnits, "    RomFS offset, in media units");
                builder.AppendLine(entry.RomFSSizeInMediaUnits, "    RomFS size, in media units");
                builder.AppendLine(entry.RomFSHashRegionSizeInMediaUnits, "    RomFS hash region size, in media units");
                builder.AppendLine(entry.Reserved4, "    Reserved 4");
                builder.AppendLine(entry.ExeFSSuperblockHash, "    ExeFS superblock SHA-256 hash");
                builder.AppendLine(entry.RomFSSuperblockHash, "    RomFS superblock SHA-256 hash");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, NCCHExtendedHeader[] entries)
        {
            builder.AppendLine("  NCCH Extended Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
            {
                builder.AppendLine("  No NCCH extended headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  NCCH Extended Header {i}");

                builder.AppendLine("    System control info:");
                builder.AppendLine(entry.SCI.ApplicationTitle, "      Application title");
                builder.AppendLine(entry.SCI.Reserved1, "      Reserved 1");
                builder.AppendLine(entry.SCI.Flag, "      Flag");
                builder.AppendLine(entry.SCI.RemasterVersion, "      Remaster version");

                builder.AppendLine("      Text code set info:");
                builder.AppendLine(entry.SCI.TextCodeSetInfo.Address, "        Address");
                builder.AppendLine(entry.SCI.TextCodeSetInfo.PhysicalRegionSizeInPages, "        Physical region size (in page-multiples)");
                builder.AppendLine(entry.SCI.TextCodeSetInfo.SizeInBytes, "        Size (in bytes)");

                builder.AppendLine(entry.SCI.StackSize, "      Stack size");

                builder.AppendLine("      Read-only code set info:");
                builder.AppendLine(entry.SCI.ReadOnlyCodeSetInfo.Address, "        Address");
                builder.AppendLine(entry.SCI.ReadOnlyCodeSetInfo.PhysicalRegionSizeInPages, "        Physical region size (in page-multiples)");
                builder.AppendLine(entry.SCI.ReadOnlyCodeSetInfo.SizeInBytes, "        Size (in bytes)");

                builder.AppendLine(entry.SCI.Reserved2, "      Reserved 2");

                builder.AppendLine("      Data code set info:");
                builder.AppendLine(entry.SCI.DataCodeSetInfo.Address, "        Address");
                builder.AppendLine(entry.SCI.DataCodeSetInfo.PhysicalRegionSizeInPages, "        Physical region size (in page-multiples)");
                builder.AppendLine(entry.SCI.DataCodeSetInfo.SizeInBytes, "        Size (in bytes)");

                builder.AppendLine(entry.SCI.BSSSize, "      BSS size");
                builder.AppendLine(entry.SCI.DependencyModuleList, "      Dependency module list");

                builder.AppendLine("      System info:");
                builder.AppendLine(entry.SCI.SystemInfo.SaveDataSize, "        SaveData size");
                builder.AppendLine(entry.SCI.SystemInfo.JumpID, "        Jump ID");
                builder.AppendLine(entry.SCI.SystemInfo.Reserved, "        Reserved");

                builder.AppendLine("    Access control info:");
                builder.AppendLine("      ARM11 local system capabilities:");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ProgramID, "        Program ID");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.CoreVersion, "        Core version");
                builder.AppendLine($"        Flag 1: {entry.ACI.ARM11LocalSystemCapabilities.Flag1} (0x{entry.ACI.ARM11LocalSystemCapabilities.Flag1:X})");
                builder.AppendLine($"        Flag 2: {entry.ACI.ARM11LocalSystemCapabilities.Flag2} (0x{entry.ACI.ARM11LocalSystemCapabilities.Flag2:X})");
                builder.AppendLine($"        Flag 0: {entry.ACI.ARM11LocalSystemCapabilities.Flag0} (0x{entry.ACI.ARM11LocalSystemCapabilities.Flag0:X})");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.Priority, "        Priority");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ResourceLimitDescriptors, "        Resource limit descriptors");

                builder.AppendLine("        Storage info:");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID, "          Extdata ID");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs, "          System savedata IDs");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs, "          Storage accessible unique IDs");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo, "          File system access info");
                builder.AppendLine($"          Other attributes: {entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes} (0x{entry.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes:X})");

                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ServiceAccessControl, "        Service access control");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl, "        Extended service access control");
                builder.AppendLine(entry.ACI.ARM11LocalSystemCapabilities.Reserved, "        Reserved");
                builder.AppendLine($"        Resource limit cateogry: {entry.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory} (0x{entry.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory:X})");

                builder.AppendLine("      ARM11 kernel capabilities:");
                builder.AppendLine(entry.ACI.ARM11KernelCapabilities.Descriptors, "        Descriptors");
                builder.AppendLine(entry.ACI.ARM11KernelCapabilities.Reserved, "        Reserved");

                builder.AppendLine("      ARM9 access control:");
                var descriptors = Array.ConvertAll(entry.ACI.ARM9AccessControl.Descriptors, d => d.ToString());
                string descriptorsStr = string.Join(", ", descriptors);
                builder.AppendLine(descriptorsStr, "        Descriptors");
                builder.AppendLine(entry.ACI.ARM9AccessControl.DescriptorVersion, "        Descriptor version");

                builder.AppendLine(entry.AccessDescSignature, "    AccessDec signature (RSA-2048-SHA256)");
                builder.AppendLine(entry.NCCHHDRPublicKey, "    NCCH HDR RSA-2048 public key");

                builder.AppendLine("    Access control info (for limitations of first ACI):");
                builder.AppendLine("      ARM11 local system capabilities:");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ProgramID, "        Program ID");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.CoreVersion, "        Core version");
                builder.AppendLine($"        Flag 1: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1:X})");
                builder.AppendLine($"        Flag 2: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2:X})");
                builder.AppendLine($"        Flag 0: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0:X})");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.Priority, "        Priority");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitDescriptors, "        Resource limit descriptors");

                builder.AppendLine("        Storage info:");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID, "          Extdata ID");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs, "          System savedata IDs");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs, "          Storage accessible unique IDs");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo, "          File system access info");
                builder.AppendLine($"          Other attributes: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes:X})");

                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ServiceAccessControl, "        Service access control");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl, "        Extended service access control");
                builder.AppendLine(entry.ACIForLimitations.ARM11LocalSystemCapabilities.Reserved, "        Reserved");
                builder.AppendLine($"        Resource limit cateogry: {entry.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory} (0x{entry.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory:X})");

                builder.AppendLine("      ARM11 kernel capabilities:");
                builder.AppendLine(entry.ACIForLimitations.ARM11KernelCapabilities.Descriptors, "        Descriptors");
                builder.AppendLine(entry.ACIForLimitations.ARM11KernelCapabilities.Reserved, "        Reserved");

                builder.AppendLine("      ARM9 access control:");
                descriptors = Array.ConvertAll(entry.ACIForLimitations.ARM9AccessControl.Descriptors, d => d.ToString());
                descriptorsStr = string.Join(", ", descriptors);
                builder.AppendLine(descriptorsStr, "        Descriptors");
                builder.AppendLine(entry.ACIForLimitations.ARM9AccessControl.DescriptorVersion, "        Descriptor version");
            }
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ExeFSHeader[] entries)
        {
            builder.AppendLine("  ExeFS Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
            {
                builder.AppendLine("  No ExeFS headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  ExeFS Header {i}");
                Print(builder, entry.FileHeaders);
                builder.AppendLine(entry.Reserved, "    Reserved");

                builder.AppendLine("    File hashes:");
                if (entry.FileHashes.Length == 0)
                {
                    builder.AppendLine("    No file hashes");
                }
                else
                {
                    for (int j = 0; j < entry.FileHashes.Length; j++)
                    {
                        var fileHash = entry.FileHashes[j];

                        builder.AppendLine($"    File Hash {j}");
                        builder.AppendLine(fileHash, "      SHA-256");
                    }
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ExeFSFileHeader[] entries)
        {
            builder.AppendLine("    File headers:");
            if (entries.Length == 0)
            {
                builder.AppendLine("    No file headers");
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"    File Header {i}");
                builder.AppendLine(entry.FileName, "      File name");
                builder.AppendLine(entry.FileOffset, "      File offset");
                builder.AppendLine(entry.FileSize, "      File size");
            }
        }

        private static void Print(StringBuilder builder, RomFSHeader[] entries)
        {
            builder.AppendLine("  RomFS Header Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
            {
                builder.AppendLine("  No RomFS headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  RomFS Header {i}");
                builder.AppendLine(entry.MagicString, "    Magic string");
                builder.AppendLine(entry.MagicNumber, "    Magic number");
                builder.AppendLine(entry.MasterHashSize, "    Master hash size");
                builder.AppendLine(entry.Level1LogicalOffset, "    Level 1 logical offset");
                builder.AppendLine(entry.Level1HashdataSize, "    Level 1 hashdata size");
                builder.AppendLine(entry.Level1BlockSizeLog2, "    Level 1 block size");
                builder.AppendLine(entry.Reserved1, "    Reserved 1");
                builder.AppendLine(entry.Level2LogicalOffset, "    Level 2 logical offset");
                builder.AppendLine(entry.Level2HashdataSize, "    Level 2 hashdata size");
                builder.AppendLine(entry.Level2BlockSizeLog2, "    Level 2 block size");
                builder.AppendLine(entry.Reserved2, "    Reserved 2");
                builder.AppendLine(entry.Level3LogicalOffset, "    Level 3 logical offset");
                builder.AppendLine(entry.Level3HashdataSize, "    Level 3 hashdata size");
                builder.AppendLine(entry.Level3BlockSizeLog2, "    Level 3 block size");
                builder.AppendLine(entry.Reserved3, "    Reserved 3");
                builder.AppendLine(entry.Reserved4, "    Reserved 4");
                builder.AppendLine(entry.OptionalInfoSize, "    Optional info size");
            }

            builder.AppendLine();
        }
    }
}
