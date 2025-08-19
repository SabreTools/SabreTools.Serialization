using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.MoPaQ;
using static SabreTools.Models.MoPaQ.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class MoPaQ : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new archive to fill
                var archive = new Archive();

                #region User Data

                // Check for User Data
                uint possibleSignature = data.ReadUInt32LittleEndian();
                data.Seek(-4, SeekOrigin.Current);
                if (possibleSignature == UserDataSignatureUInt32)
                {
                    // Deserialize the user data, returning null if invalid
                    var userData = ParseUserData(data);
                    if (userData?.Signature != UserDataSignatureString)
                        return null;

                    // Set the user data
                    archive.UserData = userData;

                    // Set the starting position according to the header offset
                    data.Seek(initialOffset + archive.UserData.HeaderOffset, SeekOrigin.Begin);
                }

                #endregion

                #region Archive Header

                // Check for the Header
                possibleSignature = data.ReadUInt32LittleEndian();
                data.Seek(-4, SeekOrigin.Current);
                if (possibleSignature == ArchiveHeaderSignatureUInt32)
                {
                    // Try to parse the archive header
                    var archiveHeader = ParseArchiveHeader(data);
                    if (archiveHeader.Signature != ArchiveHeaderSignatureString)
                        return null;

                    // Set the archive header
                    archive.ArchiveHeader = archiveHeader;
                }
                else
                {
                    return null;
                }

                #endregion

                #region Hash Table

                // TODO: The hash table has to be be decrypted before reading

                // Version 1
                if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format1)
                {
                    // If we have a hash table
                    long hashTableOffset = initialOffset + archive.ArchiveHeader.HashTablePosition;
                    if (hashTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(hashTableOffset, SeekOrigin.Begin);

                        // Find the ending offset based on size
                        long hashTableEnd = hashTableOffset + archive.ArchiveHeader.HashTableSize;

                        // Read in the hash table
                        var hashTable = new List<HashEntry>();

                        while (data.Position < hashTableEnd)
                        {
                            var hashEntry = ParseHashEntry(data);
                            hashTable.Add(hashEntry);
                        }

                        archive.HashTable = [.. hashTable];
                    }
                }

                // Version 2 and 3
                else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format2
                    || archive.ArchiveHeader.FormatVersion == FormatVersion.Format3)
                {
                    // If we have a hash table
                    long hashTableOffset = initialOffset + (((uint)archive.ArchiveHeader.HashTablePositionHi << 23) | archive.ArchiveHeader.HashTablePosition);
                    if (hashTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(hashTableOffset, SeekOrigin.Begin);

                        // Find the ending offset based on size
                        long hashTableEnd = hashTableOffset + archive.ArchiveHeader.HashTableSize;

                        // Read in the hash table
                        var hashTable = new List<HashEntry>();

                        while (data.Position < hashTableEnd)
                        {
                            var hashEntry = ParseHashEntry(data);
                            hashTable.Add(hashEntry);
                        }

                        archive.HashTable = [.. hashTable];
                    }
                }

                // Version 4
                else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format4)
                {
                    // If we have a hash table
                    long hashTableOffset = initialOffset + (((uint)archive.ArchiveHeader.HashTablePositionHi << 23) | archive.ArchiveHeader.HashTablePosition);
                    if (hashTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(hashTableOffset, SeekOrigin.Begin);

                        // Find the ending offset based on size
                        long hashTableEnd = hashTableOffset + (long)archive.ArchiveHeader.HashTableSizeLong;

                        // Read in the hash table
                        var hashTable = new List<HashEntry>();

                        while (data.Position < hashTableEnd)
                        {
                            var hashEntry = ParseHashEntry(data);
                            hashTable.Add(hashEntry);
                        }

                        archive.HashTable = [.. hashTable];
                    }
                }

                #endregion

                #region Block Table

                // Version 1
                if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format1)
                {
                    // If we have a block table
                    long blockTableOffset = initialOffset + archive.ArchiveHeader.BlockTablePosition;
                    if (blockTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(blockTableOffset, SeekOrigin.Begin);

                        // Find the ending offset based on size
                        long blockTableEnd = blockTableOffset + archive.ArchiveHeader.BlockTableSize;

                        // Read in the block table
                        var blockTable = new List<BlockEntry>();

                        while (data.Position < blockTableEnd)
                        {
                            var blockEntry = ParseBlockEntry(data);
                            blockTable.Add(blockEntry);
                        }

                        archive.BlockTable = [.. blockTable];
                    }
                }

                // Version 2 and 3
                else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format2
                    || archive.ArchiveHeader.FormatVersion == FormatVersion.Format3)
                {
                    // If we have a block table
                    long blockTableOffset = initialOffset + (((uint)archive.ArchiveHeader.BlockTablePositionHi << 23) | archive.ArchiveHeader.BlockTablePosition);
                    if (blockTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(blockTableOffset, SeekOrigin.Begin);

                        // Find the ending offset based on size
                        long blockTableEnd = blockTableOffset + archive.ArchiveHeader.BlockTableSize;

                        // Read in the block table
                        var blockTable = new List<BlockEntry>();

                        while (data.Position < blockTableEnd)
                        {
                            var blockEntry = ParseBlockEntry(data);
                            blockTable.Add(blockEntry);
                        }

                        archive.BlockTable = [.. blockTable];
                    }
                }

                // Version 4
                else if (archive.ArchiveHeader.FormatVersion == FormatVersion.Format4)
                {
                    // If we have a block table
                    long blockTableOffset = initialOffset + (((uint)archive.ArchiveHeader.BlockTablePositionHi << 23) | archive.ArchiveHeader.BlockTablePosition);
                    if (blockTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(blockTableOffset, SeekOrigin.Begin);

                        // Find the ending offset based on size
                        long blockTableEnd = blockTableOffset + (long)archive.ArchiveHeader.BlockTableSizeLong;

                        // Read in the block table
                        var blockTable = new List<BlockEntry>();

                        while (data.Position < blockTableEnd)
                        {
                            var blockEntry = ParseBlockEntry(data);
                            blockTable.Add(blockEntry);
                        }

                        archive.BlockTable = [.. blockTable];
                    }
                }

                #endregion

                #region Hi-Block Table

                // Version 2, 3, and 4
                if (archive.ArchiveHeader.FormatVersion >= FormatVersion.Format2)
                {
                    // If we have a hi-block table
                    long hiBlockTableOffset = initialOffset + (long)archive.ArchiveHeader.HiBlockTablePosition;
                    if (hiBlockTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(hiBlockTableOffset, SeekOrigin.Begin);

                        // Read in the hi-block table
                        var hiBlockTable = new List<short>();

                        for (int i = 0; i < (archive.BlockTable?.Length ?? 0); i++)
                        {
                            short hiBlockEntry = data.ReadInt16LittleEndian();
                            hiBlockTable.Add(hiBlockEntry);
                        }

                        archive.HiBlockTable = [.. hiBlockTable];
                    }
                }

                #endregion

                #region BET Table

                // Version 3 and 4
                if (archive.ArchiveHeader.FormatVersion >= FormatVersion.Format3)
                {
                    // If we have a BET table
                    long betTableOffset = initialOffset + (long)archive.ArchiveHeader.BetTablePosition;
                    if (betTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(betTableOffset, SeekOrigin.Begin);

                        // Read in the BET table
                        var betTable = ParseBetTable(data);
                        if (betTable.Signature != BetTableSignatureString)
                            return null;

                        archive.BetTable = betTable;
                    }
                }

                #endregion

                #region HET Table

                // Version 3 and 4
                if (archive.ArchiveHeader.FormatVersion >= FormatVersion.Format3)
                {
                    // If we have a HET table
                    long hetTableOffset = initialOffset + (long)archive.ArchiveHeader.HetTablePosition;
                    if (hetTableOffset > initialOffset)
                    {
                        // Seek to the offset
                        data.Seek(hetTableOffset, SeekOrigin.Begin);

                        // Read in the HET table
                        var hetTable = ParseHetTable(data);
                        if (hetTable.Signature != HetTableSignatureString)
                            return null;

                        archive.HetTable = hetTable;
                    }
                }

                #endregion

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an ArchiveHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ArchiveHeader on success, null on error</returns>
        public static ArchiveHeader ParseArchiveHeader(Stream data)
        {
            var obj = new ArchiveHeader();

            // V1 - Common
            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);

            obj.HeaderSize = data.ReadUInt32LittleEndian();
            obj.ArchiveSize = data.ReadUInt32LittleEndian();
            obj.FormatVersion = (FormatVersion)data.ReadUInt16LittleEndian();
            obj.BlockSize = data.ReadUInt16LittleEndian();
            obj.HashTablePosition = data.ReadUInt32LittleEndian();
            obj.BlockTablePosition = data.ReadUInt32LittleEndian();
            obj.HashTableSize = data.ReadUInt32LittleEndian();
            obj.BlockTableSize = data.ReadUInt32LittleEndian();

            // V2
            if (obj.FormatVersion >= FormatVersion.Format2)
            {
                obj.HiBlockTablePosition = data.ReadUInt64LittleEndian();
                obj.HashTablePositionHi = data.ReadUInt16LittleEndian();
                obj.BlockTablePositionHi = data.ReadUInt16LittleEndian();
            }

            // V3
            if (obj.FormatVersion >= FormatVersion.Format3)
            {
                obj.ArchiveSizeLong = data.ReadUInt64LittleEndian();
                obj.BetTablePosition = data.ReadUInt64LittleEndian();
                obj.HetTablePosition = data.ReadUInt64LittleEndian();
            }

            // V4
            if (obj.FormatVersion >= FormatVersion.Format4)
            {
                obj.HashTableSizeLong = data.ReadUInt64LittleEndian();
                obj.BlockTableSizeLong = data.ReadUInt64LittleEndian();
                obj.HiBlockTableSize = data.ReadUInt64LittleEndian();
                obj.HetTableSize = data.ReadUInt64LittleEndian();
                obj.BetTablesize = data.ReadUInt64LittleEndian();
                obj.RawChunkSize = data.ReadUInt32LittleEndian();

                obj.BlockTableMD5 = data.ReadBytes(0x10);
                obj.HashTableMD5 = data.ReadBytes(0x10);
                obj.HiBlockTableMD5 = data.ReadBytes(0x10);
                obj.BetTableMD5 = data.ReadBytes(0x10);
                obj.HetTableMD5 = data.ReadBytes(0x10);
                obj.HetTableMD5 = data.ReadBytes(0x10);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a BetTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BetTable on success, null on error</returns>
        public static BetTable ParseBetTable(Stream data)
        {
            var obj = new BetTable();

            // Common Headers
            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.Version = data.ReadUInt32LittleEndian();
            obj.DataSize = data.ReadUInt32LittleEndian();

            // BET-Specific
            obj.TableSize = data.ReadUInt32LittleEndian();
            obj.FileCount = data.ReadUInt32LittleEndian();
            obj.Unknown = data.ReadUInt32LittleEndian();
            obj.TableEntrySize = data.ReadUInt32LittleEndian();

            obj.FilePositionBitIndex = data.ReadUInt32LittleEndian();
            obj.FileSizeBitIndex = data.ReadUInt32LittleEndian();
            obj.CompressedSizeBitIndex = data.ReadUInt32LittleEndian();
            obj.FlagIndexBitIndex = data.ReadUInt32LittleEndian();
            obj.UnknownBitIndex = data.ReadUInt32LittleEndian();

            obj.FilePositionBitCount = data.ReadUInt32LittleEndian();
            obj.FileSizeBitCount = data.ReadUInt32LittleEndian();
            obj.CompressedSizeBitCount = data.ReadUInt32LittleEndian();
            obj.FlagIndexBitCount = data.ReadUInt32LittleEndian();
            obj.UnknownBitCount = data.ReadUInt32LittleEndian();

            obj.TotalBetHashSize = data.ReadUInt32LittleEndian();
            obj.BetHashSizeExtra = data.ReadUInt32LittleEndian();
            obj.BetHashSize = data.ReadUInt32LittleEndian();
            obj.BetHashArraySize = data.ReadUInt32LittleEndian();
            obj.FlagCount = data.ReadUInt32LittleEndian();

            obj.FlagsArray = new uint[obj.FlagCount];
            byte[] flagsArray = data.ReadBytes((int)obj.FlagCount * 4);
            Buffer.BlockCopy(flagsArray, 0, obj.FlagsArray, 0, (int)obj.FlagCount * 4);

            // TODO: Populate the file table
            // TODO: Populate the hash table

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an BlockEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BlockEntry on success, null on error</returns>
        public static BlockEntry ParseBlockEntry(Stream data)
        {
            var obj = new BlockEntry();

            obj.FilePosition = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();
            obj.Flags = (FileFlags)data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an HashEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HashEntry on success, null on error</returns>
        public static HashEntry ParseHashEntry(Stream data)
        {
            var obj = new HashEntry();

            obj.NameHashPartA = data.ReadUInt32LittleEndian();
            obj.NameHashPartB = data.ReadUInt32LittleEndian();
            obj.Locale = (Locale)data.ReadInt16LittleEndian();
            obj.Platform = data.ReadUInt16LittleEndian();
            obj.BlockIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HetTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HetTable on success, null on error</returns>
        public static HetTable ParseHetTable(Stream data)
        {
            var obj = new HetTable();

            // Common Headers
            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.Version = data.ReadUInt32LittleEndian();
            obj.DataSize = data.ReadUInt32LittleEndian();

            // HET-Specific
            obj.TableSize = data.ReadUInt32LittleEndian();
            obj.MaxFileCount = data.ReadUInt32LittleEndian();
            obj.HashTableSize = data.ReadUInt32LittleEndian();
            obj.TotalIndexSize = data.ReadUInt32LittleEndian();
            obj.IndexSizeExtra = data.ReadUInt32LittleEndian();
            obj.IndexSize = data.ReadUInt32LittleEndian();
            obj.BlockTableSize = data.ReadUInt32LittleEndian();
            obj.HashTable = data.ReadBytes((int)obj.HashTableSize);

            // TODO: Populate the file indexes array
            obj.FileIndexes = new byte[(int)obj.HashTableSize][];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a UserData
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled UserData on success, null on error</returns>
        public static UserData ParseUserData(Stream data)
        {
            var obj = new UserData();

            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.UserDataSize = data.ReadUInt32LittleEndian();
            obj.HeaderOffset = data.ReadUInt32LittleEndian();
            obj.UserDataHeaderSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        #region Helpers

        /// <summary>
        /// Buffer for encryption and decryption
        /// </summary>
        private readonly uint[] _stormBuffer = new uint[STORM_BUFFER_SIZE];

        /// <summary>
        /// Prepare the encryption table
        /// </summary>
        private void PrepareCryptTable()
        {
            uint seed = 0x00100001;
            for (uint index1 = 0; index1 < 0x100; index1++)
            {
                for (uint index2 = index1, i = 0; i < 5; i++, index2 += 0x100)
                {
                    seed = (seed * 125 + 3) % 0x2AAAAB;
                    uint temp1 = (seed & 0xFFFF) << 0x10;

                    seed = (seed * 125 + 3) % 0x2AAAAB;
                    uint temp2 = (seed & 0xFFFF);

                    _stormBuffer[index2] = (temp1 | temp2);
                }
            }
        }

        /// <summary>
        /// Decrypt a single block of data
        /// </summary>
        private unsafe byte[] DecryptBlock(byte[] block, uint length, uint key)
        {
            uint seed = 0xEEEEEEEE;

            uint[] castBlock = new uint[length / 4];
            Buffer.BlockCopy(block, 0, castBlock, 0, (int)length);
            int castBlockPtr = 0;

            // Round to uints
            length >>= 2;

            while (length-- > 0)
            {
                seed += _stormBuffer[MPQ_HASH_KEY2_MIX + (key & 0xFF)];
                uint ch = castBlock[castBlockPtr] ^ (key + seed);

                key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
                seed = ch + seed + (seed << 5) + 3;
                castBlock[castBlockPtr++] = ch;
            }

            Buffer.BlockCopy(castBlock, 0, block, 0, (int)length);
            return block;
        }

        #endregion
    }
}
