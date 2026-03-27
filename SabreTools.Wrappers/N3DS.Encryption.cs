using System;
using System.IO;
using SabreTools.Data.Models.N3DS;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;
using SabreTools.Security.Cryptography;
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
        private static byte[] GetKeyXForCryptoMethod(N3DSEncryptionSettings settings, CryptoMethod method)
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
            var settings = new N3DSEncryptionSettings
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
        private void DecryptPartition(int index, N3DSEncryptionSettings settings, Stream reader, Stream writer)
        {
            // Determine the keys needed for this partition
            N3DSPartitionKeys? keys = GetDecryptionKeys(index, settings);
            if (keys is null)
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
        private N3DSPartitionKeys? GetDecryptionKeys(int index, N3DSEncryptionSettings settings)
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
            return new N3DSPartitionKeys(signature, fixedCryptoKey, settings.AESHardwareConstant, keyX, keyX0x2C);
        }

        /// <summary>
        /// Decrypt the extended header, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool DecryptExtendedHeader(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private bool DecryptExeFS(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private void DecryptExeFSFilenameTable(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private void DecryptExeFSFileEntries(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private bool DecryptRomFS(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
            var settings = new N3DSEncryptionSettings
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
        private void EncryptPartition(int index, N3DSEncryptionSettings settings, Stream reader, Stream writer)
        {
            // Determine the keys needed for this partition
            N3DSPartitionKeys? keys = GetEncryptionKeys(index, settings);
            if (keys is null)
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
        private N3DSPartitionKeys? GetEncryptionKeys(int index, N3DSEncryptionSettings settings)
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
            return new N3DSPartitionKeys(signature, fixedCryptoKey, settings.AESHardwareConstant, keyX, keyX0x2C);
        }

        /// <summary>
        /// Encrypt the extended header, if it exists
        /// </summary>
        /// <param name="index">Index of the partition</param>
        /// <param name="keys">Keys for the partition</param>
        /// <param name="reader">Stream representing the input</param>
        /// <param name="writer">Stream representing the output</param>
        private bool EncryptExtendedHeader(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private bool EncryptExeFS(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private void EncryptExeFSFilenameTable(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private void EncryptExeFSFileEntries(int index, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
        private bool EncryptRomFS(int index, N3DSEncryptionSettings settings, N3DSPartitionKeys keys, Stream reader, Stream writer)
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
    }
}
