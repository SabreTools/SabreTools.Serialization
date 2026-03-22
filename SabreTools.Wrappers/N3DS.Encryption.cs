using System;
using System.IO;
using SabreTools.Data.Models.N3DS;
using SabreTools.IO.Encryption;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.N3DS.Constants;

namespace SabreTools.Wrappers
{
    public partial class N3DS
    {
        #region Common

        /// <summary>
        /// Get the initial value for the ExeFS counter
        /// </summary>
        public byte[] ExeFSIV(int index)
        {
            if (Partitions is null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header is null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. ExefsCounter];
        }

        /// <summary>
        /// Get the initial value for the plain counter
        /// </summary>
        public byte[] PlainIV(int index)
        {
            if (Partitions is null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header is null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. PlainCounter];
        }

        /// <summary>
        /// Get the initial value for the RomFS counter
        /// </summary>
        public byte[] RomFSIV(int index)
        {
            if (Partitions is null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header is null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. RomfsCounter];
        }

        /// <summary>
        /// Get KeyX value for a crypto method and development status combination
        /// </summary>
        private static byte[] GetKeyXForCryptoMethod(EncryptionSettings settings, CryptoMethod method)
        {
            switch (method)
            {
                case CryptoMethod.Original:
                    Console.WriteLine("Encryption Method: Key 0x2C");
                    return settings.Development ? settings.DevKeyX0x2C : settings.KeyX0x2C;

                case CryptoMethod.Seven:
                    Console.WriteLine("Encryption Method: Key 0x25");
                    return settings.Development ? settings.DevKeyX0x25 : settings.KeyX0x25;

                case CryptoMethod.NineThree:
                    Console.WriteLine("Encryption Method: Key 0x18");
                    return settings.Development ? settings.DevKeyX0x18 : settings.KeyX0x18;

                case CryptoMethod.NineSix:
                    Console.WriteLine("Encryption Method: Key 0x1B");
                    return settings.Development ? settings.DevKeyX0x1B : settings.KeyX0x1B;

                // This should never happen
                default:
                    Console.WriteLine("Encryption Method: UNSUPPORTED");
                    return [];
            }
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt all partitions in the partition table of an NCSD header
        /// </summary>
        /// <param name="force">Indicates if the operation should be forced</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        /// <param name="development">Indicates if development images are expected</param>
        /// <param name="aesHardwareConstant">AES Hardware Constant</param>
        /// <param name="keyX0x18">KeyX 0x18 (New 3DS 9.3)</param>
        /// <param name="devKeyX0x18">Dev KeyX 0x18 (New 3DS 9.3)</param>
        /// <param name="keyX0x1B">KeyX 0x1B (New 3DS 9.6)</param>
        /// <param name="devKeyX0x1B">Dev KeyX 0x1B New 3DS 9.6)</param>
        /// <param name="keyX0x25">KeyX 0x25 (> 7.x)</param>
        /// <param name="devKeyX0x25">Dev KeyX 0x25 (> 7.x)</param>
        /// <param name="keyX0x2C">KeyX 0x2C (< 6.x)</param>
        /// <param name="devKeyX0x2C">Dev KeyX 0x2C (< 6.x)</param>
        public void DecryptAllPartitions(bool force,
            Stream reader,
            Stream writer,
            bool development = false,
            byte[]? aesHardwareConstant = null,
            byte[]? keyX0x18 = null,
            byte[]? devKeyX0x18 = null,
            byte[]? keyX0x1B = null,
            byte[]? devKeyX0x1B = null,
            byte[]? keyX0x25 = null,
            byte[]? devKeyX0x25 = null,
            byte[]? keyX0x2C = null,
            byte[]? devKeyX0x2C = null)
        {
            // Check the partitions table
            if (PartitionsTable is null || Partitions is null)
            {
                Console.WriteLine("Invalid partitions table!");
                return;
            }

            // Create a new set of encryption settings
            var settings = new EncryptionSettings
            {
                Development = development,
                AESHardwareConstant = aesHardwareConstant ?? [],
                KeyX0x18 = keyX0x18 ?? [],
                DevKeyX0x18 = devKeyX0x18 ?? [],
                KeyX0x1B = keyX0x1B ?? [],
                DevKeyX0x1B = devKeyX0x1B ?? [],
                KeyX0x25 = keyX0x25 ?? [],
                DevKeyX0x25 = devKeyX0x25 ?? [],
                KeyX0x2C = keyX0x2C ?? [],
                DevKeyX0x2C = devKeyX0x2C ?? [],
            };

            // Iterate over all 8 NCCH partitions
            for (int p = 0; p < 8; p++)
            {
                var partition = Partitions[p];
                if (partition is null || partition.MagicID != NCCHMagicNumber)
                {
                    Console.WriteLine($"Partition {p} Not found... Skipping...");
                    continue;
                }

                // Check the partition has data
                var partitionEntry = PartitionsTable[p];
                if (partitionEntry is null || partitionEntry.Length == 0)
                {
                    Console.WriteLine($"Partition {p} No data... Skipping...");
                    continue;
                }

                // Decrypt the partition, if possible
                if (ShouldDecryptPartition(p, force))
                    DecryptPartition(p, settings, reader, writer);
            }
        }

        /// <summary>
        /// Determine if the current partition should be decrypted
        /// </summary>s
        private bool ShouldDecryptPartition(int index, bool force)
        {
            // If we're forcing the operation, tell the user
            if (force)
            {
                Console.WriteLine($"Partition {index} is not verified due to force flag being set.");
                return true;
            }
            // If we're not forcing the operation, check if the 'NoCrypto' bit is set
            else if (PossiblyDecrypted(index))
            {
                Console.WriteLine($"Partition {index}: Already Decrypted?...");
                return false;
            }

            // By default, it passes
            return true;
        }

        /// <summary>
        /// Decrypt a single partition
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="settings">Encryption settings</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private void DecryptPartition(int index, EncryptionSettings settings, Stream reader, Stream writer)
        {
            // Determine the keys needed for this partition
            PartitionKeys? keys = GetDecryptionKeys(index, settings);
            if (keys == null)
            {
                Console.WriteLine($"Partition {index} could not generate keys. Skipping...");
                return;
            }

            // Decrypt the parts of the partition
            DecryptExtendedHeader(index, keys, reader, writer);
            DecryptExeFS(index, keys, reader, writer);
            DecryptRomFS(index, keys, reader, writer);

            // Update the flags
            UpdateDecryptCryptoAndMasks(index, writer);
        }

        /// <summary>
        /// Determine the set of keys to be used for decryption
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="settings">Encryption settings</param>
        private PartitionKeys? GetDecryptionKeys(int index, EncryptionSettings settings)
        {
            // Get the partition
            var partition = Partitions?[index];
            if (partition?.Flags is null)
                return null;

            // Get partition-specific values
            byte[]? signature = partition.RSA2048Signature;
            BitMasks masks = GetBitMasks(index);
            CryptoMethod method = GetCryptoMethod(index);

            // Get the partition keys
#if NET20 || NET35
            bool fixedCryptoKey = (masks & BitMasks.FixedCryptoKey) > 0;
#else
            bool fixedCryptoKey = masks.HasFlag(BitMasks.FixedCryptoKey);
#endif
            byte[] keyX = GetKeyXForCryptoMethod(settings, method);
            byte[] keyX0x2C = settings.Development ? settings.DevKeyX0x2C : settings.KeyX0x2C;
            return new PartitionKeys(signature, fixedCryptoKey, settings.AESHardwareConstant, keyX, keyX0x2C);
        }

        /// <summary>
        /// Decrypt the extended header, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool DecryptExtendedHeader(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Get required offsets
            uint partitionOffset = GetPartitionOffset(index);
            if (partitionOffset == 0 || partitionOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} No Data... Skipping...");
                return false;
            }

            uint extHeaderSize = GetExtendedHeaderSize(index);
            if (extHeaderSize == 0)
            {
                Console.WriteLine($"Partition {index} No Extended Header... Skipping...");
                return false;
            }

            // Seek to the extended header
            reader.Seek(partitionOffset + 0x200, SeekOrigin.Begin);
            writer.Seek(partitionOffset + 0x200, SeekOrigin.Begin);

            Console.WriteLine($"Partition {index}: Decrypting - ExHeader");

            // Create the Plain AES cipher for this partition
            var cipher = AESCTR.CreateDecryptionCipher(keys.NormalKey2C, PlainIV(index));

            // Process the extended header
            AESCTR.PerformOperation(CXTExtendedDataHeaderLength, cipher, reader, writer, null);

#if NET6_0_OR_GREATER
            // In .NET 6.0, this operation is not picked up by the reader, so we have to force it to reload its buffer
            reader.Seek(0, SeekOrigin.Begin);
#endif
            writer.Flush();
            return true;
        }

        /// <summary>
        /// Decrypt the ExeFS, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool DecryptExeFS(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Validate the ExeFS
            uint exeFsHeaderOffset = GetExeFSOffset(index);
            if (exeFsHeaderOffset == 0 || exeFsHeaderOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return false;
            }

            uint exeFsSize = GetExeFSSize(index);
            if (exeFsSize == 0)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return false;
            }

            // Decrypt the filename table
            DecryptExeFSFilenameTable(index, keys, reader, writer);

            // For all but the original crypto method, process each of the files in the table
            if (GetCryptoMethod(index) != CryptoMethod.Original)
                DecryptExeFSFileEntries(index, keys, reader, writer);

            // Get the ExeFS files offset
            uint exeFsFilesOffset = exeFsHeaderOffset + MediaUnitSize;

            // Seek to the ExeFS
            reader.Seek(exeFsFilesOffset, SeekOrigin.Begin);
            writer.Seek(exeFsFilesOffset, SeekOrigin.Begin);

            // Create the ExeFS AES cipher for this partition
            uint ctroffsetE = MediaUnitSize / 0x10;
            byte[] exefsIVWithOffset = ExeFSIV(index).Add(ctroffsetE);
            var cipher = AESCTR.CreateDecryptionCipher(keys.NormalKey2C, exefsIVWithOffset);

            // Setup and perform the decryption
            exeFsSize -= MediaUnitSize;
            AESCTR.PerformOperation(exeFsSize,
                cipher,
                reader,
                writer,
                s => Console.WriteLine($"\rPartition {index} ExeFS: Decrypting - {s}"));

            return true;
        }

        /// <summary>
        /// Decrypt the ExeFS Filename Table
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private void DecryptExeFSFilenameTable(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Get ExeFS offset
            uint exeFsOffset = GetExeFSOffset(index);
            if (exeFsOffset == 0 || exeFsOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return;
            }

            // Seek to the ExeFS header
            reader.Seek(exeFsOffset, SeekOrigin.Begin);
            writer.Seek(exeFsOffset, SeekOrigin.Begin);

            Console.WriteLine($"Partition {index} ExeFS: Decrypting - ExeFS Filename Table");

            // Create the ExeFS AES cipher for this partition
            var cipher = AESCTR.CreateDecryptionCipher(keys.NormalKey2C, ExeFSIV(index));

            // Process the filename table
            byte[] readBytes = reader.ReadBytes((int)MediaUnitSize);
            byte[] processedBytes = cipher.ProcessBytes(readBytes);
            writer.Write(processedBytes);

#if NET6_0_OR_GREATER
            // In .NET 6.0, this operation is not picked up by the reader, so we have to force it to reload its buffer
            reader.Seek(0, SeekOrigin.Begin);
#endif
            writer.Flush();
        }

        /// <summary>
        /// Decrypt the ExeFS file entries
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private void DecryptExeFSFileEntries(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            if (ExeFSHeaders is null || index < 0 || index > ExeFSHeaders.Length)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return;
            }

            // Reread the decrypted ExeFS header
            uint exeFsHeaderOffset = GetExeFSOffset(index);
            reader.Seek(exeFsHeaderOffset, SeekOrigin.Begin);
            ExeFSHeaders[index] = Serialization.Readers.N3DS.ParseExeFSHeader(reader);

            // Get the ExeFS header
            var exeFsHeader = ExeFSHeaders[index];
            if (exeFsHeader?.FileHeaders is null)
            {
                Console.WriteLine($"Partition {index} ExeFS header does not exist. Skipping...");
                return;
            }

            // Get the ExeFS files offset
            uint exeFsFilesOffset = exeFsHeaderOffset + MediaUnitSize;

            // Loop through and process all headers
            for (int i = 0; i < exeFsHeader.FileHeaders.Length; i++)
            {
                // Only attempt to process code binary files
                if (!IsCodeBinary(index, i))
                    continue;

                // Get the file header
                var fileHeader = exeFsHeader.FileHeaders[i];
                if (fileHeader is null)
                    continue;

                // Create the ExeFS AES ciphers for this partition
                uint ctroffset = (fileHeader.FileOffset + MediaUnitSize) / 0x10;
                byte[] exefsIVWithOffsetForHeader = ExeFSIV(index).Add(ctroffset);
                var firstCipher = AESCTR.CreateDecryptionCipher(keys.NormalKey, exefsIVWithOffsetForHeader);
                var secondCipher = AESCTR.CreateEncryptionCipher(keys.NormalKey2C, exefsIVWithOffsetForHeader);

                // Seek to the file entry
                reader.Seek(exeFsFilesOffset + fileHeader.FileOffset, SeekOrigin.Begin);
                writer.Seek(exeFsFilesOffset + fileHeader.FileOffset, SeekOrigin.Begin);

                // Setup and perform the encryption
                AESCTR.PerformOperation(fileHeader.FileSize,
                    firstCipher,
                    secondCipher,
                    reader,
                    writer,
                    s => Console.WriteLine($"\rPartition {index} ExeFS: Decrypting - {fileHeader.FileName}...{s}"));
            }
        }

        /// <summary>
        /// Decrypt the RomFS, if it exists
        /// </summary>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="index">Index of the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool DecryptRomFS(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Validate the RomFS
            uint romFsOffset = GetRomFSOffset(index);
            if (romFsOffset == 0 || romFsOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} RomFS: No Data... Skipping...");
                return false;
            }

            uint romFsSize = GetRomFSSize(index);
            if (romFsSize == 0)
            {
                Console.WriteLine($"Partition {index} RomFS: No Data... Skipping...");
                return false;
            }

            // Seek to the RomFS
            reader.Seek(romFsOffset, SeekOrigin.Begin);
            writer.Seek(romFsOffset, SeekOrigin.Begin);

            // Create the RomFS AES cipher for this partition
            var cipher = AESCTR.CreateDecryptionCipher(keys.NormalKey, RomFSIV(index));

            // Setup and perform the decryption
            AESCTR.PerformOperation(romFsSize,
                cipher,
                reader,
                writer,
                s => Console.WriteLine($"\rPartition {index} RomFS: Decrypting - {s}"));

            return true;
        }

        /// <summary>
        /// Update the CryptoMethod and BitMasks for the decrypted partition
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="writer">Stream representing the output</param>
        private void UpdateDecryptCryptoAndMasks(int index, Stream writer)
        {
            // Get required offsets
            uint partitionOffset = GetPartitionOffset(index);

            // Seek to the CryptoMethod location
            writer.Seek(partitionOffset + 0x18B, SeekOrigin.Begin);

            // Write the new CryptoMethod
            writer.Write((byte)CryptoMethod.Original);
            writer.Flush();

            // Seek to the BitMasks location
            writer.Seek(partitionOffset + 0x18F, SeekOrigin.Begin);

            // Write the new BitMasks flag
            BitMasks flag = GetBitMasks(index);
            flag &= (BitMasks)((byte)(BitMasks.FixedCryptoKey | BitMasks.NewKeyYGenerator) ^ 0xFF);
            flag |= BitMasks.NoCrypto;
            writer.Write((byte)flag);
            writer.Flush();
        }

        #endregion

        #region Encrypt

        /// <summary>
        /// Encrypt all partitions in the partition table of an NCSD header
        /// </summary>
        /// <param name="force">Indicates if the operation should be forced</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        /// <param name="development">Indicates if development images are expected</param>
        /// <param name="aesHardwareConstant">AES Hardware Constant</param>
        /// <param name="keyX0x18">KeyX 0x18 (New 3DS 9.3)</param>
        /// <param name="devKeyX0x18">Dev KeyX 0x18 (New 3DS 9.3)</param>
        /// <param name="keyX0x1B">KeyX 0x1B (New 3DS 9.6)</param>
        /// <param name="devKeyX0x1B">Dev KeyX 0x1B New 3DS 9.6)</param>
        /// <param name="keyX0x25">KeyX 0x25 (> 7.x)</param>
        /// <param name="devKeyX0x25">Dev KeyX 0x25 (> 7.x)</param>
        /// <param name="keyX0x2C">KeyX 0x2C (< 6.x)</param>
        /// <param name="devKeyX0x2C">Dev KeyX 0x2C (< 6.x)</param>
        public void EncryptAllPartitions(bool force,
            Stream reader,
            Stream writer,
            bool development = false,
            byte[]? aesHardwareConstant = null,
            byte[]? keyX0x18 = null,
            byte[]? devKeyX0x18 = null,
            byte[]? keyX0x1B = null,
            byte[]? devKeyX0x1B = null,
            byte[]? keyX0x25 = null,
            byte[]? devKeyX0x25 = null,
            byte[]? keyX0x2C = null,
            byte[]? devKeyX0x2C = null)
        {
            // Check the partitions table
            if (PartitionsTable is null || Partitions is null)
            {
                Console.WriteLine("Invalid partitions table!");
                return;
            }

            // Create a new set of encryption settings
            var settings = new EncryptionSettings
            {
                Development = development,
                AESHardwareConstant = aesHardwareConstant ?? [],
                KeyX0x18 = keyX0x18 ?? [],
                DevKeyX0x18 = devKeyX0x18 ?? [],
                KeyX0x1B = keyX0x1B ?? [],
                DevKeyX0x1B = devKeyX0x1B ?? [],
                KeyX0x25 = keyX0x25 ?? [],
                DevKeyX0x25 = devKeyX0x25 ?? [],
                KeyX0x2C = keyX0x2C ?? [],
                DevKeyX0x2C = devKeyX0x2C ?? [],
            };

            // Iterate over all 8 NCCH partitions
            for (int p = 0; p < 8; p++)
            {
                // Check the partition exists
                var partition = Partitions[p];
                if (partition is null || partition.MagicID != NCCHMagicNumber)
                {
                    Console.WriteLine($"Partition {p} Not found... Skipping...");
                    continue;
                }

                // Check the partition has data
                var partitionEntry = PartitionsTable[p];
                if (partitionEntry is null || partitionEntry.Length == 0)
                {
                    Console.WriteLine($"Partition {p} No data... Skipping...");
                    continue;
                }

                // Encrypt the partition, if possible
                if (ShouldEncryptPartition(p, force))
                    EncryptPartition(p, settings, reader, writer);
            }
        }

        /// <summary>
        /// Determine if the current partition should be encrypted
        /// </summary>
        private bool ShouldEncryptPartition(int index, bool force)
        {
            // If we're forcing the operation, tell the user
            if (force)
            {
                Console.WriteLine($"Partition {index} is not verified due to force flag being set.");
                return true;
            }
            // If we're not forcing the operation, check if the 'NoCrypto' bit is set
            else if (!PossiblyDecrypted(index))
            {
                Console.WriteLine($"Partition {index}: Already Encrypted?...");
                return false;
            }

            // By default, it passes
            return true;
        }

        /// <summary>
        /// Encrypt a single partition
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="settings">Encryption settings</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private void EncryptPartition(int index, EncryptionSettings settings, Stream reader, Stream writer)
        {
            // Determine the keys needed for this partition
            PartitionKeys? keys = GetEncryptionKeys(index, settings);
            if (keys == null)
            {
                Console.WriteLine($"Partition {index} could not generate keys. Skipping...");
                return;
            }

            // Encrypt the parts of the partition
            EncryptExtendedHeader(index, keys, reader, writer);
            EncryptExeFS(index, keys, reader, writer);
            EncryptRomFS(index, settings, keys, reader, writer);

            // Update the flags
            UpdateEncryptCryptoAndMasks(index, writer);
        }

        /// <summary>
        /// Determine the set of keys to be used for encryption
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="settings">Encryption settings</param>
        private PartitionKeys? GetEncryptionKeys(int index, EncryptionSettings settings)
        {
            // Get the partition
            var partition = Partitions?[index];
            if (partition is null)
                return null;

            // Get the backup header
            var backupHeader = BackupHeader;
            if (backupHeader?.Flags is null)
                return null;

            // Get partition-specific values
            byte[]? signature = partition.RSA2048Signature;
            BitMasks masks = backupHeader.Flags.BitMasks;
            CryptoMethod method = backupHeader.Flags.CryptoMethod;

            // Get the partition keys
#if NET20 || NET35
            bool fixedCryptoKey = (masks & BitMasks.FixedCryptoKey) > 0;
#else
            bool fixedCryptoKey = masks.HasFlag(BitMasks.FixedCryptoKey);
#endif
            byte[] keyX = GetKeyXForCryptoMethod(settings, method);
            byte[] keyX0x2C = settings.Development ? settings.DevKeyX0x2C : settings.KeyX0x2C;
            return new PartitionKeys(signature, fixedCryptoKey, settings.AESHardwareConstant, keyX, keyX0x2C);
        }

        /// <summary>
        /// Encrypt the extended header, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool EncryptExtendedHeader(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Get required offsets
            uint partitionOffset = GetPartitionOffset(index);
            if (partitionOffset == 0 || partitionOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} No Data... Skipping...");
                return false;
            }

            uint extHeaderSize = GetExtendedHeaderSize(index);
            if (extHeaderSize == 0)
            {
                Console.WriteLine($"Partition {index} No Extended Header... Skipping...");
                return false;
            }

            // Seek to the extended header
            reader.Seek(partitionOffset + 0x200, SeekOrigin.Begin);
            writer.Seek(partitionOffset + 0x200, SeekOrigin.Begin);

            Console.WriteLine($"Partition {index}: Encrypting - ExHeader");

            // Create the Plain AES cipher for this partition
            var cipher = AESCTR.CreateEncryptionCipher(keys.NormalKey2C, PlainIV(index));

            // Process the extended header
            AESCTR.PerformOperation(CXTExtendedDataHeaderLength, cipher, reader, writer, null);

#if NET6_0_OR_GREATER
            // In .NET 6.0, this operation is not picked up by the reader, so we have to force it to reload its buffer
            reader.Seek(0, SeekOrigin.Begin);
#endif
            writer.Flush();
            return true;
        }

        /// <summary>
        /// Encrypt the ExeFS, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool EncryptExeFS(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            if (ExeFSHeaders is null || index < 0 || index > ExeFSHeaders.Length)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return false;
            }

            // Get the ExeFS header
            var exefsHeader = ExeFSHeaders[index];
            if (exefsHeader is null)
            {
                Console.WriteLine($"Partition {index} ExeFS header does not exist. Skipping...");
                return false;
            }

            // For all but the original crypto method, process each of the files in the table
            var backupHeader = BackupHeader;
            if (backupHeader!.Flags!.CryptoMethod != CryptoMethod.Original)
                EncryptExeFSFileEntries(index, keys, reader, writer);

            // Encrypt the filename table
            EncryptExeFSFilenameTable(index, keys, reader, writer);

            // Get the ExeFS files offset
            uint exeFsHeaderOffset = GetExeFSOffset(index);
            uint exeFsFilesOffset = exeFsHeaderOffset + MediaUnitSize;

            // Seek to the ExeFS
            reader.Seek(exeFsFilesOffset, SeekOrigin.Begin);
            writer.Seek(exeFsFilesOffset, SeekOrigin.Begin);

            // Create the ExeFS AES cipher for this partition
            uint ctroffsetE = MediaUnitSize / 0x10;
            byte[] exefsIVWithOffset = ExeFSIV(index).Add(ctroffsetE);
            var cipher = AESCTR.CreateEncryptionCipher(keys.NormalKey2C, exefsIVWithOffset);

            // Setup and perform the encryption
            uint exeFsSize = GetExeFSSize(index) - MediaUnitSize;
            AESCTR.PerformOperation(exeFsSize,
                cipher,
                reader,
                writer,
                s => Console.WriteLine($"\rPartition {index} ExeFS: Encrypting - {s}"));

            return true;
        }

        /// <summary>
        /// Encrypt the ExeFS Filename Table
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private void EncryptExeFSFilenameTable(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Get ExeFS offset
            uint exeFsOffset = GetExeFSOffset(index);
            if (exeFsOffset == 0 || exeFsOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return;
            }

            // Seek to the ExeFS header
            reader.Seek(exeFsOffset, SeekOrigin.Begin);
            writer.Seek(exeFsOffset, SeekOrigin.Begin);

            Console.WriteLine($"Partition {index} ExeFS: Encrypting - ExeFS Filename Table");

            // Create the ExeFS AES cipher for this partition
            var cipher = AESCTR.CreateEncryptionCipher(keys.NormalKey2C, ExeFSIV(index));

            // Process the filename table
            byte[] readBytes = reader.ReadBytes((int)MediaUnitSize);
            byte[] processedBytes = cipher.ProcessBytes(readBytes);
            writer.Write(processedBytes);

#if NET6_0_OR_GREATER
            // In .NET 6.0, this operation is not picked up by the reader, so we have to force it to reload its buffer
            reader.Seek(0, SeekOrigin.Begin);
#endif
            writer.Flush();
        }

        /// <summary>
        /// Encrypt the ExeFS file entries
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private void EncryptExeFSFileEntries(int index, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Get ExeFS offset
            uint exeFsHeaderOffset = GetExeFSOffset(index);
            if (exeFsHeaderOffset == 0 || exeFsHeaderOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} ExeFS: No Data... Skipping...");
                return;
            }

            // Get to the start of the files
            uint exeFsFilesOffset = exeFsHeaderOffset + MediaUnitSize;

            // If the header failed to read, log and return
            var exeFsHeader = ExeFSHeaders?[index];
            if (exeFsHeader?.FileHeaders is null)
            {
                Console.WriteLine($"Partition {index} ExeFS header does not exist. Skipping...");
                return;
            }

            // Loop through and process all headers
            for (int i = 0; i < exeFsHeader.FileHeaders.Length; i++)
            {
                // Only attempt to process code binary files
                if (!IsCodeBinary(index, i))
                    continue;

                // Get the file header
                var fileHeader = exeFsHeader.FileHeaders[i];
                if (fileHeader is null)
                    continue;

                // Create the ExeFS AES ciphers for this partition
                uint ctroffset = (fileHeader.FileOffset + MediaUnitSize) / 0x10;
                byte[] exefsIVWithOffsetForHeader = ExeFSIV(index).Add(ctroffset);
                var firstCipher = AESCTR.CreateEncryptionCipher(keys.NormalKey, exefsIVWithOffsetForHeader);
                var secondCipher = AESCTR.CreateDecryptionCipher(keys.NormalKey2C, exefsIVWithOffsetForHeader);

                // Seek to the file entry
                reader.Seek(exeFsFilesOffset + fileHeader.FileOffset, SeekOrigin.Begin);
                writer.Seek(exeFsFilesOffset + fileHeader.FileOffset, SeekOrigin.Begin);

                // Setup and perform the encryption
                AESCTR.PerformOperation(fileHeader.FileSize,
                    firstCipher,
                    secondCipher,
                    reader,
                    writer,
                    s => Console.WriteLine($"\rPartition {index} ExeFS: Encrypting - {fileHeader.FileName}...{s}"));
            }
        }

        /// <summary>
        /// Encrypt the RomFS, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="settings">Encryption settings</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool EncryptRomFS(int index, EncryptionSettings settings, PartitionKeys keys, Stream reader, Stream writer)
        {
            // Validate the RomFS
            uint romFsOffset = GetRomFSOffset(index);
            if (romFsOffset == 0 || romFsOffset > reader.Length)
            {
                Console.WriteLine($"Partition {index} RomFS: No Data... Skipping...");
                return false;
            }

            uint romFsSize = GetRomFSSize(index);
            if (romFsSize == 0)
            {
                Console.WriteLine($"Partition {index} RomFS: No Data... Skipping...");
                return false;
            }

            // Seek to the RomFS
            reader.Seek(romFsOffset, SeekOrigin.Begin);
            writer.Seek(romFsOffset, SeekOrigin.Begin);

            // Force setting encryption keys for partitions 1 and above
            if (index > 0)
            {
                var backupHeader = BackupHeader;
#if NET20 || NET35
                bool fixedCryptoKey = (backupHeader.Flags.BitMasks & BitMasks.FixedCryptoKey) > 0;
#else
                bool fixedCryptoKey = backupHeader.Flags.BitMasks.HasFlag(BitMasks.FixedCryptoKey);
#endif

                keys.SetRomFSValues(fixedCryptoKey,
                    settings.AESHardwareConstant,
                    settings.Development ? settings.DevKeyX0x2C : settings.KeyX0x2C);
            }

            // Create the RomFS AES cipher for this partition
            var cipher = AESCTR.CreateEncryptionCipher(keys.NormalKey, RomFSIV(index));

            // Setup and perform the decryption
            AESCTR.PerformOperation(romFsSize,
                cipher,
                reader,
                writer,
                s => Console.WriteLine($"\rPartition {index} RomFS: Encrypting - {s}"));

            return true;
        }

        /// <summary>
        /// Update the CryptoMethod and BitMasks for the encrypted partition
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="writer">Stream representing the output</param>
        private void UpdateEncryptCryptoAndMasks(int index, Stream writer)
        {
            // Get required offsets
            uint partitionOffset = GetPartitionOffset(index);

            // Get the backup header
            var backupHeader = BackupHeader;
            if (backupHeader?.Flags is null)
                return;

            // Seek to the CryptoMethod location
            writer.Seek(partitionOffset + 0x18B, SeekOrigin.Begin);

            // Write the new CryptoMethod
            // - For partitions 1 and up, set crypto-method to 0x00
            // - If partition 0, restore crypto-method from backup flags
            byte cryptoMethod = index > 0 ? (byte)CryptoMethod.Original : (byte)backupHeader.Flags.CryptoMethod;
            writer.Write(cryptoMethod);
            writer.Flush();

            // Seek to the BitMasks location
            writer.Seek(partitionOffset + 0x18F, SeekOrigin.Begin);

            // Write the new BitMasks flag
            BitMasks flag = GetBitMasks(index);
            flag &= (BitMasks.FixedCryptoKey | BitMasks.NewKeyYGenerator | BitMasks.NoCrypto) ^ (BitMasks)0xFF;
            flag |= (BitMasks.FixedCryptoKey | BitMasks.NewKeyYGenerator) & backupHeader.Flags.BitMasks;
            writer.Write((byte)flag);
            writer.Flush();
        }

        #endregion

        // TODO: Remove these when IO is updated
        #region Helper Classes

        /// <summary>
        /// 3DS Encryption settings to be passed around during processing
        /// </summary>
        private class EncryptionSettings
        {
            /// <summary>
            /// Indicates if development images are expected
            /// </summary>
            public bool Development { get; set; }

            /// <summary>
            /// AES Hardware Constant
            /// </summary>
            /// TODO: Validate this value on assignment
            public byte[] AESHardwareConstant { get; set; } = [];

            /// <summary>
            /// KeyX 0x18 (New 3DS 9.3)
            /// </summary>
            public byte[] KeyX0x18
            {
                get;
                set
                {
                    if (ValidateKeyX0x18(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// Dev KeyX 0x18 (New 3DS 9.3)
            /// </summary>
            public byte[] DevKeyX0x18
            {
                get;
                set
                {
                    if (ValidateDevKeyX0x18(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// KeyX 0x1B (New 3DS 9.6)
            /// </summary>
            public byte[] KeyX0x1B
            {
                get;
                set
                {
                    if (ValidateKeyX0x1B(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// Dev KeyX 0x1B New 3DS 9.6)
            /// </summary>
            public byte[] DevKeyX0x1B
            {
                get;
                set
                {
                    if (ValidateDevKeyX0x1B(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// KeyX 0x25 (> 7.x)
            /// </summary>
            public byte[] KeyX0x25
            {
                get;
                set
                {
                    if (ValidateKeyX0x25(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// Dev KeyX 0x25 (> 7.x)
            /// </summary>
            public byte[] DevKeyX0x25
            {
                get;
                set
                {
                    if (ValidateDevKeyX0x25(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// KeyX 0x2C (< 6.x)
            /// </summary>
            public byte[] KeyX0x2C
            {
                get;
                set
                {
                    if (ValidateKeyX0x2C(value))
                        field = value;
                }
            } = [];

            /// <summary>
            /// Dev KeyX 0x2C (< 6.x)
            /// </summary>
            public byte[] DevKeyX0x2C
            {
                get;
                set
                {
                    if (ValidateDevKeyX0x2C(value))
                        field = value;
                }
            } = [];

            #region Internal Test Values

            /// <summary>
            /// Initial value for key validation tests
            /// </summary>
            private static readonly byte[] TestIV =
            [
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            ];

            /// <summary>
            /// Pattern to use for key validation tests
            /// </summary>
            private static readonly byte[] TestPattern =
            [
                0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08,
                0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
                0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08,
                0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
                0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08,
                0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
                0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08,
                0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00,
            ];

            /// <summary>
            /// Expected output value for KeyX0x18
            /// </summary>
            private static readonly byte[] ExpectedKeyX0x18 =
            [
                0x06, 0xF1, 0xB2, 0x3B, 0x12, 0xAD, 0x80, 0xC1,
                0x13, 0xC6, 0x18, 0x3D, 0x27, 0xB8, 0xB9, 0x95,
                0x49, 0x73, 0x59, 0x82, 0xEF, 0xFE, 0x16, 0x48,
                0x91, 0x2A, 0x89, 0x55, 0x9A, 0xDC, 0x3C, 0xA0,
                0x84, 0x46, 0x14, 0xE0, 0x16, 0x59, 0x8E, 0x4F,
                0xC2, 0x6C, 0x52, 0xA4, 0x7D, 0xAD, 0x4F, 0x23,
                0xF1, 0xC6, 0x99, 0x44, 0x39, 0xB7, 0x42, 0xF0,
                0x1F, 0xBB, 0x02, 0xF6, 0x0A, 0x8A, 0xC2, 0x9A,
            ];

            /// <summary>
            /// Expected output value for DevKeyX0x18
            /// </summary>
            private static readonly byte[] ExpectedDevKeyX0x18 =
            [
                0x99, 0x6E, 0x3C, 0x54, 0x97, 0x3C, 0xEA, 0xE8,
                0xBA, 0xAE, 0x18, 0x5C, 0x93, 0x27, 0x65, 0x50,
                0xF6, 0x6D, 0x67, 0xD7, 0xEF, 0xBD, 0x7C, 0xCB,
                0x8A, 0xC1, 0x1A, 0x54, 0xFC, 0x3B, 0x8B, 0x3A,
                0x0E, 0xE5, 0xEF, 0x27, 0x4A, 0x73, 0x7E, 0x0A,
                0x2E, 0x2E, 0x9D, 0xAF, 0x6C, 0x03, 0xF2, 0x91,
                0xC4, 0xFA, 0x73, 0xFD, 0x6B, 0xA0, 0x07, 0xD4,
                0x75, 0x5B, 0x6F, 0x2E, 0x8B, 0x68, 0x4C, 0xD1,
            ];

            /// <summary>
            /// Expected output value for KeyX0x1B
            /// </summary>
            private static readonly byte[] ExpectedKeyX0x1B =
            [
                0x0A, 0xE4, 0x79, 0x02, 0x1B, 0xFA, 0x25, 0x4B,
                0x2D, 0x92, 0x4F, 0xA8, 0x41, 0x59, 0xCE, 0x10,
                0x09, 0xE6, 0x08, 0x61, 0x23, 0xC7, 0xD2, 0x30,
                0x84, 0x37, 0xD5, 0x49, 0x42, 0x94, 0xB2, 0x70,
                0x6A, 0xF3, 0x75, 0xB0, 0x1F, 0x4F, 0xA1, 0xCE,
                0x03, 0xA2, 0x6A, 0x19, 0x5D, 0x32, 0x0D, 0xB5,
                0x79, 0xCD, 0xFD, 0xF0, 0xDE, 0x49, 0x26, 0x2D,
                0x29, 0x36, 0x30, 0x69, 0x8B, 0x45, 0xE1, 0xFC,
            ];

            /// <summary>
            /// Expected output value for DevKeyX0x1B
            /// </summary>
            private static readonly byte[] ExpectedDevKeyX0x1B =
            [
                    0x16, 0x4F, 0xD9, 0x58, 0xC9, 0x20, 0xB3, 0xED,
                0xC4, 0xEB, 0x57, 0x39, 0x10, 0xEF, 0xA8, 0xCC,
                0xE5, 0x49, 0xBF, 0x52, 0x10, 0xA9, 0xCC, 0xE1,
                0x65, 0x3B, 0x2D, 0x51, 0x45, 0xFB, 0x60, 0x52,
                0x3E, 0x29, 0xEB, 0xEB, 0x3F, 0xF2, 0x76, 0x08,
                0x00, 0x05, 0x7F, 0x64, 0x29, 0x4A, 0x17, 0x22,
                0x56, 0x7F, 0x49, 0x94, 0x1A, 0x8C, 0x56, 0x35,
                0x38, 0xBE, 0xA4, 0x2E, 0x58, 0xD3, 0x81, 0x8C,
            ];

            /// <summary>
            /// Expected output value for KeyX0x25
            /// </summary>
            private static readonly byte[] ExpectedKeyX0x25 =
            [
                0x37, 0xBC, 0x73, 0xD6, 0xEE, 0x73, 0xE0, 0x94,
                0x42, 0x84, 0x74, 0xE5, 0xD8, 0xFB, 0x5F, 0x65,
                0xF4, 0xCF, 0x2E, 0xC1, 0x43, 0x48, 0x6C, 0xAA,
                0xC8, 0xF9, 0x96, 0xE6, 0x33, 0xDD, 0xE7, 0xBF,
                0xD2, 0x21, 0x89, 0x39, 0x13, 0xD1, 0xEC, 0xCA,
                0x1D, 0x5D, 0x1F, 0x77, 0x95, 0xD2, 0x8B, 0x27,
                0x92, 0x79, 0xC5, 0x1D, 0x72, 0xA7, 0x28, 0x57,
                0x41, 0x0E, 0x46, 0xB8, 0x80, 0x7B, 0x7C, 0x0D,
            ];

            /// <summary>
            /// Expected output value for DevKeyX0x25
            /// </summary>
            private static readonly byte[] ExpectedDevKeyX0x25 =
            [
                0x71, 0x65, 0x30, 0xF2, 0x68, 0xEC, 0x65, 0x0A,
                0x8C, 0x9E, 0xC5, 0x5A, 0xFA, 0x37, 0x8E, 0xDA,
                0x7B, 0x58, 0x3B, 0x66, 0x7C, 0x9D, 0x16, 0xD9,
                0x2D, 0x8F, 0xCF, 0x04, 0x66, 0x7F, 0x27, 0x41,
                0xBF, 0x5F, 0x1E, 0x11, 0x4C, 0xD6, 0xB9, 0x0A,
                0xC5, 0x42, 0xCF, 0x2B, 0x87, 0x6B, 0xD4, 0x72,
                0x4D, 0x9C, 0x29, 0x2E, 0xF8, 0xB0, 0x6F, 0x22,
                0x35, 0x5B, 0x96, 0x83, 0xD1, 0xE4, 0x5E, 0xDB,
            ];

            /// <summary>
            /// Expected output value for KeyX0x2C
            /// </summary>
            private static readonly byte[] ExpectedKeyX0x2C =
            [
                0xAE, 0x44, 0x20, 0xDB, 0xA5, 0x96, 0xDC, 0xF3,
                0xD8, 0x23, 0x9E, 0x3C, 0x44, 0x73, 0x3D, 0xCD,
                0x07, 0xD5, 0xF8, 0xD0, 0xC6, 0xB3, 0x5A, 0x80,
                0xB5, 0x5A, 0x55, 0x30, 0x5D, 0x4A, 0xBE, 0x61,
                0xBF, 0xEF, 0x64, 0x17, 0x28, 0xD6, 0x26, 0x52,
                0x42, 0x4D, 0x8F, 0x1C, 0xBC, 0x63, 0xD3, 0x91,
                0x7D, 0xA6, 0x4F, 0xAF, 0x26, 0x38, 0x60, 0xEE,
                0x79, 0x92, 0x2F, 0xD8, 0xCA, 0x4E, 0xE7, 0xEC,
            ];

            /// <summary>
            /// Expected output value for DevKeyX0x2C
            /// </summary>
            private static readonly byte[] ExpectedDevKeyX0x2C =
            [
                0x5F, 0x73, 0xD5, 0x9A, 0x67, 0xFF, 0x8C, 0x12,
                0x31, 0x58, 0x0B, 0x58, 0x46, 0xFE, 0x05, 0x16,
                0x92, 0xE4, 0x84, 0x06, 0x18, 0x9B, 0x58, 0x91,
                0xE7, 0xF8, 0xCD, 0xA9, 0x95, 0xAC, 0x07, 0xCD,
                0x43, 0x20, 0x7A, 0x8C, 0xCC, 0xAB, 0x48, 0x50,
                0x29, 0x2F, 0x96, 0x73, 0xB0, 0xD9, 0xE5, 0xCB,
                0xE6, 0x9A, 0x0D, 0xF7, 0xD0, 0x1E, 0xC2, 0xEC,
                0xC1, 0xE2, 0x8E, 0xEE, 0x89, 0xB9, 0xB1, 0x97,
            ];

            #endregion

            #region Validation Methods

            /// <summary>
            /// Validate a possible KeyX0x18 value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateKeyX0x18(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedKeyX0x18);
            }

            /// <summary>
            /// Validate a possible DevKeyX0x18 value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateDevKeyX0x18(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedDevKeyX0x18);
            }

            /// <summary>
            /// Validate a possible KeyX0x1B value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateKeyX0x1B(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedKeyX0x1B);
            }

            /// <summary>
            /// Validate a possible DevKeyX0x1B value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateDevKeyX0x1B(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedDevKeyX0x1B);
            }

            /// <summary>
            /// Validate a possible KeyX0x25 value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateKeyX0x25(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedKeyX0x25);
            }

            /// <summary>
            /// Validate a possible DevKeyX0x25 value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateDevKeyX0x25(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedDevKeyX0x25);
            }

            /// <summary>
            /// Validate a possible KeyX0x2C value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateKeyX0x2C(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedKeyX0x2C);
            }

            /// <summary>
            /// Validate a possible DevKeyX0x2C value
            /// </summary>
            /// <param name="key">Key to test</param>
            /// <returns>True if the key was valid, false otherwise</returns>
            public bool ValidateDevKeyX0x2C(byte[]? key)
            {
                // Missing key data is considered invalid
                if (key is null || key.Length == 0)
                    return false;

                // Validate the key data
                var cipher = AESCTR.CreateEncryptionCipher(key, TestIV);
                byte[] actual = cipher.ProcessBytes(TestPattern);
                return actual.EqualsExactly(ExpectedDevKeyX0x2C);
            }

            #endregion
        }

        /// <summary>
        /// Set of all keys associated with a partition
        /// </summary>
        private class PartitionKeys
        {
            /// <summary>
            /// Primary AES-CTR encryption key
            /// </summary>
            /// <remarks>Used for both EXE-FS and ROM-FS</remarks>
            public byte[] NormalKey { get; private set; }

            /// <summary>
            /// Secondary AES-CTR encryption key
            /// </summary>
            /// <remarks>Used for only EXE-FS</remarks>
            public byte[] NormalKey2C { get; }

            /// <summary>
            /// First 16 bytes of the RSA-2048 signature
            /// </summary>
            /// <remarks>Used as an XOR value during key generation</remarks>
            private readonly byte[] KeyY;

            /// <summary>
            /// Create a new set of keys for a given partition
            /// </summary>
            /// <param name="signature">RSA-2048 signature from the partition</param>
            /// <param name="fixedCryptoKey">Flag indicating if the FixedCryptoKey bit mask was set</param>
            /// <param name="hardwareConstant">AES hardware constant to use</param>
            /// <param name="keyX">KeyX value to assign based on crypto method and development status</param>
            /// <param name="keyX0x2C">KeyX2C value to assign based on development status</param>
            public PartitionKeys(byte[]? signature, bool fixedCryptoKey, byte[] hardwareConstant, byte[] keyX, byte[] keyX0x2C)
            {
                // Validate inputs
                if (signature is not null && signature.Length < 16)
                    throw new ArgumentOutOfRangeException(nameof(signature), $"{nameof(signature)} must be at least 16 bytes");

                // Backup headers can't have a KeyY value set
                KeyY = new byte[16];
                if (signature is not null)
                    Array.Copy(signature, KeyY, 16);

                // Special case for zero-key
                if (fixedCryptoKey)
                {
                    Console.WriteLine("Encryption Method: Zero Key");
                    NormalKey = new byte[16];
                    NormalKey2C = new byte[16];
                    return;
                }

                // Set the standard normal key values
                NormalKey = ProcessKey(keyX, KeyY, hardwareConstant);
                NormalKey2C = ProcessKey(keyX0x2C, KeyY, hardwareConstant);
            }

            /// <summary>
            /// Set RomFS values based on the bit masks
            /// </summary>
            /// <param name="fixedCryptoKey">Flag indicating if the FixedCryptoKey bit mask was set</param>
            /// <param name="hardwareConstant">AES hardware constant to use</param>
            /// <param name="keyX0x2C">KeyX2C value to assign based on development status</param>
            public void SetRomFSValues(bool fixedCryptoKey, byte[] hardwareConstant, byte[] keyX0x2C)
            {
                // NormalKey has a constant value for zero-key
                if (fixedCryptoKey)
                {
                    NormalKey = new byte[16];
                    return;
                }

                // Encrypting RomFS for partitions 1 and up always use Key0x2C
                NormalKey = ProcessKey(keyX0x2C, KeyY, hardwareConstant);
            }

            /// <summary>
            /// Process a key with the standard processing steps
            /// </summary>
            private static byte[] ProcessKey(byte[] keyBase, byte[] keyY, byte[] hardwareConstant)
            {
                byte[] processed = keyBase.RotateLeft(2);
                processed = processed.Xor(keyY);
                processed = processed.Add(hardwareConstant);
                return processed.RotateLeft(87);
            }
        }

        #endregion
    }
}
