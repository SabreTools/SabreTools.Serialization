using System;
using System.IO;
using System.Text;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
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

                // Prepare the crypt table
                PrepareCryptTable();

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

                // Try to parse the archive header
                var archiveHeader = ParseArchiveHeader(data);
                if (archiveHeader == null)
                    return null;

                // Set the archive header
                archive.ArchiveHeader = archiveHeader;

                #endregion

                #region Hash Table

                // Set the hash table, if possible
                var hashTable = ParseHashTable(data, initialOffset, archive.ArchiveHeader);
                if (hashTable != null)
                    archive.HashTable = hashTable;

                #endregion

                #region Block Table

                // Set the block table, if possible
                var blockTable = ParseBlockTable(data, initialOffset, archive.ArchiveHeader);
                if (blockTable != null)
                    archive.BlockTable = blockTable;

                #endregion

                #region Hi-Block Table

                // Set the hi-block table, if possible
                var hiBlockTable = ParseHiBlockTable(data, initialOffset, archive.ArchiveHeader);
                if (hiBlockTable != null)
                    archive.HiBlockTable = hiBlockTable;

                #endregion

                #region BET Table

                // Set the BET table, if possible
                var betTable = ParseBetTable(data, initialOffset, archive.ArchiveHeader);
                if (betTable != null)
                    archive.BetTable = betTable;

                #endregion

                #region HET Table

                // Set the HET table, if possible
                var hetTable = ParseHetTable(data, initialOffset, archive.ArchiveHeader);
                if (hetTable != null)
                    archive.HetTable = hetTable;

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
        public static ArchiveHeader? ParseArchiveHeader(Stream data)
        {
            var obj = new ArchiveHeader();

            // V1 - Common
            byte[] signature = data.ReadBytes(4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            if (obj.Signature != ArchiveHeaderSignatureString)
                return null;

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
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <returns>Filled BetTable on success, null on error</returns>
        public static BetTable? ParseBetTable(Stream data, long initialOffset, ArchiveHeader header)
        {
            // Get the BET table offset by version
            long offset = header.FormatVersion switch
            {
                FormatVersion.Format1 => -1,
                FormatVersion.Format2 => -1,
                FormatVersion.Format3 => (long)header.BetTablePosition,
                FormatVersion.Format4 => (long)header.BetTablePosition,
                _ => -1,
            };

            // If the offset is invalid
            if (offset < initialOffset || offset >= data.Length)
                return null;

            // Preprocess the table
            byte[]? tableBytes = LoadTable(data,
                offset,
                header.HashTableMD5,
                (uint)header.HashTableSizeLong,
                92,
                MPQ_KEY_BLOCK_TABLE,
                out _);
            if (tableBytes == null)
                return null;

            // Read in the BET table
            int tableOffset = 0;
            return ParseBetTable(tableBytes, ref tableOffset);
        }

        /// <summary>
        /// Parse a Stream into a BetTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BetTable on success, null on error</returns>
        public static BetTable ParseBetTable(byte[] data, ref int offset)
        {
            var obj = new BetTable();

            // Common Headers
            byte[] signature = data.ReadBytes(ref offset, 4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.Version = data.ReadUInt32LittleEndian(ref offset);
            obj.DataSize = data.ReadUInt32LittleEndian(ref offset);

            // BET-Specific
            obj.TableSize = data.ReadUInt32LittleEndian(ref offset);
            obj.FileCount = data.ReadUInt32LittleEndian(ref offset);
            obj.Unknown = data.ReadUInt32LittleEndian(ref offset);
            obj.TableEntrySize = data.ReadUInt32LittleEndian(ref offset);

            obj.FilePositionBitIndex = data.ReadUInt32LittleEndian(ref offset);
            obj.FileSizeBitIndex = data.ReadUInt32LittleEndian(ref offset);
            obj.CompressedSizeBitIndex = data.ReadUInt32LittleEndian(ref offset);
            obj.FlagIndexBitIndex = data.ReadUInt32LittleEndian(ref offset);
            obj.UnknownBitIndex = data.ReadUInt32LittleEndian(ref offset);

            obj.FilePositionBitCount = data.ReadUInt32LittleEndian(ref offset);
            obj.FileSizeBitCount = data.ReadUInt32LittleEndian(ref offset);
            obj.CompressedSizeBitCount = data.ReadUInt32LittleEndian(ref offset);
            obj.FlagIndexBitCount = data.ReadUInt32LittleEndian(ref offset);
            obj.UnknownBitCount = data.ReadUInt32LittleEndian(ref offset);

            obj.TotalBetHashSize = data.ReadUInt32LittleEndian(ref offset);
            obj.BetHashSizeExtra = data.ReadUInt32LittleEndian(ref offset);
            obj.BetHashSize = data.ReadUInt32LittleEndian(ref offset);
            obj.BetHashArraySize = data.ReadUInt32LittleEndian(ref offset);
            obj.FlagCount = data.ReadUInt32LittleEndian(ref offset);

            obj.FlagsArray = new uint[obj.FlagCount];
            byte[] flagsArray = data.ReadBytes(ref offset, (int)obj.FlagCount * 4);
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
        public static BlockEntry ParseBlockEntry(byte[] data, ref int offset)
        {
            var obj = new BlockEntry();

            obj.FilePosition = data.ReadUInt32LittleEndian(ref offset);
            obj.CompressedSize = data.ReadUInt32LittleEndian(ref offset);
            obj.UncompressedSize = data.ReadUInt32LittleEndian(ref offset);
            obj.Flags = (FileFlags)data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a BlockTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <returns>Filled BlockTable on success, null on error</returns>
        public static BlockEntry[]? ParseBlockTable(Stream data, long initialOffset, ArchiveHeader header)
        {
            // Get the block table offset
            long offset = ((uint)header.BlockTablePositionHi << 23) | header.BlockTablePosition;
            if (offset < initialOffset || offset >= data.Length)
                return null;

            // Get the entry count
            uint entryCount = header.BlockTableSize;

            // Preprocess the table
            byte[]? tableBytes = LoadTable(data,
                offset,
                header.HashTableMD5,
                (uint)header.HashTableSizeLong,
                entryCount * 16,
                MPQ_KEY_BLOCK_TABLE,
                out _);
            if (tableBytes == null)
                return null;

            // Read in the block table
            int tableOffset = 0;
            var blockTable = new BlockEntry[entryCount];
            for (int i = 0; i < blockTable.Length; i++)
            {
                var blockEntry = ParseBlockEntry(tableBytes, ref tableOffset);
                blockTable[i] = blockEntry;
            }

            return blockTable;
        }

        /// <summary>
        /// Parse a Stream into an HashEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HashEntry on success, null on error</returns>
        public static HashEntry ParseHashEntry(byte[] data, ref int offset)
        {
            var obj = new HashEntry();

            obj.NameHashPartA = data.ReadUInt32LittleEndian(ref offset);
            obj.NameHashPartB = data.ReadUInt32LittleEndian(ref offset);
            obj.Locale = (Locale)data.ReadInt16LittleEndian(ref offset);
            obj.Platform = data.ReadUInt16LittleEndian(ref offset);
            obj.BlockIndex = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HashTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <returns>Filled HashTable on success, null on error</returns>
        public static HashEntry[]? ParseHashTable(Stream data, long initialOffset, ArchiveHeader header)
        {
            // Get the hash table offset
            long offset = initialOffset + (((uint)header.HashTablePositionHi << 23) | header.HashTablePosition);
            if (offset < initialOffset || offset >= data.Length)
                return null;

            // Get the entry count
            uint entryCount = header.HashTableSize;

            // Preprocess the table
            byte[]? tableBytes = LoadTable(data,
                offset,
                header.HashTableMD5,
                (uint)header.HashTableSizeLong,
                entryCount * 16,
                MPQ_KEY_HASH_TABLE,
                out _);
            if (tableBytes == null)
                return null;

            // Read in the hash table
            int tableOffset = 0;
            var hashTable = new HashEntry[entryCount];
            for (int i = 0; i < hashTable.Length; i++)
            {
                var hashEntry = ParseHashEntry(tableBytes, ref tableOffset);
                hashTable[i] = hashEntry;
            }

            return hashTable;
        }

        /// <summary>
        /// Parse a Stream into a HetTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <returns>Filled HetTable on success, null on error</returns>
        public static HetTable? ParseHetTable(Stream data, long initialOffset, ArchiveHeader header)
        {
            // Get the HET table offset by version
            long offset = header.FormatVersion switch
            {
                FormatVersion.Format1 => -1,
                FormatVersion.Format2 => -1,
                FormatVersion.Format3 => (long)header.HetTablePosition,
                FormatVersion.Format4 => (long)header.HetTablePosition,
                _ => -1,
            };

            // If the offset is invalid
            if (offset < initialOffset || offset >= data.Length)
                return null;

            // Preprocess the table
            byte[]? tableBytes = LoadTable(data,
                offset,
                header.HashTableMD5,
                (uint)header.HashTableSizeLong,
                44,
                MPQ_KEY_HASH_TABLE,
                out _);
            if (tableBytes == null)
                return null;

            // Read in the HET table
            int tableOffset = 0;
            return ParseHetTable(tableBytes, ref tableOffset);
        }

        /// <summary>
        /// Parse a Stream into a HetTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HetTable on success, null on error</returns>
        public static HetTable ParseHetTable(byte[] data, ref int offset)
        {
            var obj = new HetTable();

            // Common Headers
            byte[] signature = data.ReadBytes(ref offset, 4);
            obj.Signature = Encoding.ASCII.GetString(signature);
            obj.Version = data.ReadUInt32LittleEndian(ref offset);
            obj.DataSize = data.ReadUInt32LittleEndian(ref offset);

            // HET-Specific
            obj.TableSize = data.ReadUInt32LittleEndian(ref offset);
            obj.MaxFileCount = data.ReadUInt32LittleEndian(ref offset);
            obj.HashTableSize = data.ReadUInt32LittleEndian(ref offset);
            obj.TotalIndexSize = data.ReadUInt32LittleEndian(ref offset);
            obj.IndexSizeExtra = data.ReadUInt32LittleEndian(ref offset);
            obj.IndexSize = data.ReadUInt32LittleEndian(ref offset);
            obj.BlockTableSize = data.ReadUInt32LittleEndian(ref offset);
            obj.HashTable = data.ReadBytes(ref offset, (int)obj.HashTableSize);

            // TODO: Populate the file indexes array
            obj.FileIndexes = new byte[(int)obj.HashTableSize][];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HiBlockTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <returns>Filled HiBlockTable on success, null on error</returns>
        /// TODO: Add MD5 validation of contents
        /// TODO: The table has to be be decompressed before reading for V4(?)
        public static short[]? ParseHiBlockTable(Stream data, long initialOffset, ArchiveHeader header)
        {
            // Get the hi-block table offset by version
            long offset = header.FormatVersion switch
            {
                FormatVersion.Format1 => -1,
                FormatVersion.Format2 => initialOffset + (long)header.HiBlockTablePosition,
                FormatVersion.Format3 => initialOffset + (long)header.HiBlockTablePosition,
                FormatVersion.Format4 => initialOffset + (long)header.HiBlockTablePosition,
                _ => -1,
            };

            // If the offset is invalid
            if (offset < initialOffset || offset >= data.Length)
                return null;

            // Get the entry count by version
            ulong entryCount = header.FormatVersion switch
            {
                FormatVersion.Format1 => 0,
                FormatVersion.Format2 => header.HiBlockTableSize >> 1,
                FormatVersion.Format3 => header.HiBlockTableSize >> 1,
                FormatVersion.Format4 => header.HiBlockTableSize >> 1,
                _ => 0,
            };

            // Seek to the offset
            data.Seek(offset, SeekOrigin.Begin);

            // Read in the hi-block table
            var hiBlockTable = new short[entryCount];
            for (int i = 0; i < hiBlockTable.Length; i++)
            {
                var hiBlockEntry = data.ReadInt16LittleEndian();
                hiBlockTable[i] = hiBlockEntry;
            }

            return hiBlockTable;
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
        /// Indicates if the buffer has been built or not
        /// </summary>
        private static bool _cryptPrepared = false;

        /// <summary>
        /// Buffer for encryption and decryption
        /// </summary>
        private static readonly uint[] _stormBuffer = new uint[STORM_BUFFER_SIZE];

        /// <summary>
        /// Prepare the encryption table
        /// </summary>
        private static void PrepareCryptTable()
        {
            if (_cryptPrepared)
                return;

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

            _cryptPrepared = true;
        }

        /// <summary>
        /// Load a table block by optionally decompressing and
        /// decrypting before returning the data.
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="offset">Data offset to parse</param>
        /// <param name="expectedHash">Optional MD5 hash for validation</param>
        /// <param name="compressedSize">Size of the table in the file</param>
        /// <param name="tableSize">Expected size of the table</param>
        /// <param name="key">Encryption key to use</param>
        /// <param name="realTableSize">Output represening the real table size</param>
        /// <returns>Byte array representing the processed table</returns>
        private static byte[]? LoadTable(Stream data,
            long offset,
            byte[]? expectedHash,
            uint compressedSize,
            uint tableSize,
            uint key,
            out long realTableSize)
        {
            byte[]? tableData;
            byte[]? readBytes;
            long bytesToRead = tableSize;

            // Allocate the MPQ table
            tableData = readBytes = new byte[tableSize];

            // Check if the MPQ table is compressed
            if (compressedSize != 0 && compressedSize < tableSize)
            {
                // Allocate temporary buffer for holding compressed data
                readBytes = new byte[compressedSize];
                bytesToRead = compressedSize;
            }

            // Get the file offset from which we will read the table
            // Note: According to Storm.dll from Warcraft III (version 2002),
            // if the hash table position is 0xFFFFFFFF, no SetFilePointer call is done
            // and the table is loaded from the current file offset
            if (offset == 0xFFFFFFFF)
                offset = data.Position;

            // Is the sector table within the file?
            if (offset >= data.Length)
            {
                realTableSize = 0;
                return null;
            }

            // The hash table and block table can go beyond EOF.
            // Storm.dll reads as much as possible, then fills the missing part with zeros.
            // Abused by Spazzler map protector which sets hash table size to 0x00100000
            // Abused by NP_Protect in MPQs v4 as well
            if ((offset + bytesToRead) > data.Length)
                bytesToRead = (uint)(data.Length - offset);

            // Give the caller information that the table was cut
            realTableSize = bytesToRead;

            // If everything succeeded, read the raw table from the MPQ
            data.Seek(offset, SeekOrigin.Begin);
            _ = data.Read(readBytes, 0, (int)bytesToRead);

            // Verify the MD5 of the table, if present
            byte[]? actualHash = HashTool.GetByteArrayHashArray(readBytes, HashType.MD5);
            if (expectedHash != null && actualHash != null && !actualHash.EqualsExactly(expectedHash))
            {
                Console.WriteLine("Table is corrupt!");
                return null;
            }

            // First of all, decrypt the table
            if (key != 0)
                tableData = DecryptBlock(readBytes, bytesToRead, key);

            // If the table is compressed, decompress it
            if (compressedSize != 0 && compressedSize < tableSize)
            {
                Console.WriteLine("Table is compressed, it will not read properly!");
                return null;

                // TODO: Handle decompression
                // int cbOutBuffer = (int)tableSize;
                // int cbInBuffer = (int)compressedSize;

                // if (!SCompDecompress2(readBytes, &cbOutBuffer, tableData, cbInBuffer))
                //     errorCode = SErrGetLastError();

                // tableData = readBytes;
            }

            // Return the MPQ table
            return tableData;
        }

        /// <summary>
        /// Decrypt a single block of data
        /// </summary>
        private static unsafe byte[] DecryptBlock(byte[] block, long length, uint key)
        {
            uint seed = 0xEEEEEEEE;

            uint[] castBlock = new uint[length >> 2];
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

            Buffer.BlockCopy(castBlock, 0, block, 0, block.Length >> 2);
            return block;
        }

        #endregion
    }
}
