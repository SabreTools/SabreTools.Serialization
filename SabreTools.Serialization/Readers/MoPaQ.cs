using System;
using System.IO;
using System.Text;
using SabreTools.Data.Models.MoPaQ;
using SabreTools.IO.Encryption;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.MoPaQ.Constants;

namespace SabreTools.Serialization.Readers
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
                var decrypter = new MoPaQDecrypter();

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
                var hashTable = ParseHashTable(data, initialOffset, archive.ArchiveHeader, decrypter);
                if (hashTable != null)
                    archive.HashTable = hashTable;

                #endregion

                #region Block Table

                // Set the block table, if possible
                var blockTable = ParseBlockTable(data, initialOffset, archive.ArchiveHeader, decrypter);
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
                var betTable = ParseBetTable(data, initialOffset, archive.ArchiveHeader, decrypter);
                if (betTable != null)
                    archive.BetTable = betTable;

                #endregion

                #region HET Table

                // Set the HET table, if possible
                var hetTable = ParseHetTable(data, initialOffset, archive.ArchiveHeader, decrypter);
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
                obj.BetTableSize = data.ReadUInt64LittleEndian();
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
        /// <param name="decrypter">Decrypter for processing table data</param>
        /// <returns>Filled BetTable on success, null on error</returns>
        public static BetTable? ParseBetTable(Stream data, long initialOffset, ArchiveHeader header, MoPaQDecrypter decrypter)
        {
            // Get the BET table offset
            long offset = initialOffset + (long)header.BetTablePosition;
            if (offset <= initialOffset || offset >= data.Length)
                return null;

            // Preprocess the table
            byte[]? tableBytes = decrypter.LoadTable(data,
                offset,
                header.BetTableMD5,
                (uint)header.BetTableSize,
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
        /// Parse a Stream into a BlockTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <param name="decrypter">Decrypter for processing table data</param>
        /// <returns>Filled BlockTable on success, null on error</returns>
        public static BlockEntry[]? ParseBlockTable(Stream data, long initialOffset, ArchiveHeader header, MoPaQDecrypter decrypter)
        {
            // Get the block table offset
            long offset = initialOffset + ((uint)header.BlockTablePositionHi << 23) | header.BlockTablePosition;
            if (offset <= initialOffset || offset >= data.Length)
                return null;

            // Get the entry count
            uint entryCount = header.BlockTableSize;

            // Preprocess the table
            byte[]? tableBytes = decrypter.LoadTable(data,
                offset,
                header.BlockTableMD5,
                (uint)header.BlockTableSizeLong,
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
        /// Parse a Stream into a HashTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <param name="header">Archive header to get information from</param>
        /// <param name="decrypter">Decrypter for processing table data</param>
        /// <returns>Filled HashTable on success, null on error</returns>
        public static HashEntry[]? ParseHashTable(Stream data, long initialOffset, ArchiveHeader header, MoPaQDecrypter decrypter)
        {
            // Get the hash table offset
            long offset = initialOffset + (((uint)header.HashTablePositionHi << 23) | header.HashTablePosition);
            if (offset <= initialOffset || offset >= data.Length)
                return null;

            // Get the entry count
            uint entryCount = header.HashTableSize;

            // Preprocess the table
            byte[]? tableBytes = decrypter.LoadTable(data,
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
        /// <param name="decrypter">Decrypter for processing table data</param>
        /// <returns>Filled HetTable on success, null on error</returns>
        public static HetTable? ParseHetTable(Stream data, long initialOffset, ArchiveHeader header, MoPaQDecrypter decrypter)
        {
            // Get the HET table offset
            long offset = initialOffset + (long)header.HetTablePosition;
            if (offset <= initialOffset || offset >= data.Length)
                return null;

            // Preprocess the table
            byte[]? tableBytes = decrypter.LoadTable(data,
                offset,
                header.HetTableMD5,
                (uint)header.HetTableSize,
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
            // Get the hi-block table offset
            long offset = initialOffset + (long)header.HiBlockTablePosition;
            if (offset <= initialOffset || offset >= data.Length)
                return null;

            // Get the entry count
            ulong entryCount = header.HiBlockTableSize >> 1;

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

        /// <summary>
        /// Parse a Stream into a BetTable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BetTable on success, null on error</returns>
        private static BetTable ParseBetTable(byte[] data, ref int offset)
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
        /// Parse decrypted data into an BlockEntry
        /// </summary>
        /// <param name="data">Decrypted data to parse</param>
        /// <returns>Filled BlockEntry on success, null on error</returns>
        private static BlockEntry ParseBlockEntry(byte[] data, ref int offset)
        {
            var obj = new BlockEntry();

            obj.FilePosition = data.ReadUInt32LittleEndian(ref offset);
            obj.CompressedSize = data.ReadUInt32LittleEndian(ref offset);
            obj.UncompressedSize = data.ReadUInt32LittleEndian(ref offset);
            obj.Flags = (FileFlags)data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse decrypted data into an HashEntry
        /// </summary>
        /// <param name="data">Decrypted data to parse</param>
        /// <returns>Filled HashEntry on success, null on error</returns>
        private static HashEntry ParseHashEntry(byte[] data, ref int offset)
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
        /// Parse a decrypted data into a HetTable
        /// </summary>
        /// <param name="data">Decrypted data to parse</param>
        /// <returns>Filled HetTable on success, null on error</returns>
        private static HetTable ParseHetTable(byte[] data, ref int offset)
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
    }
}
