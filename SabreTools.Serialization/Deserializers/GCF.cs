using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.GCF;

namespace SabreTools.Serialization.Deserializers
{
    public class GCF : BaseBinaryDeserializer<Models.GCF.File>
    {
        /// <inheritdoc/>
        public override Models.GCF.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life Game Cache to fill
            var file = new Models.GCF.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the game cache header
            file.Header = header;

            #endregion

            #region Block Entry Header

            // Try to parse the block entry header
            var blockEntryHeader = ParseBlockEntryHeader(data);
            if (blockEntryHeader == null)
                return null;

            // Set the game cache block entry header
            file.BlockEntryHeader = blockEntryHeader;

            #endregion

            #region Block Entries

            // Create the block entry array
            file.BlockEntries = new BlockEntry[blockEntryHeader.BlockCount];

            // Try to parse the block entries
            for (int i = 0; i < blockEntryHeader.BlockCount; i++)
            {
                var blockEntry = ParseBlockEntry(data);
                if (blockEntry == null)
                    return null;

                file.BlockEntries[i] = blockEntry;
            }

            #endregion

            #region Fragmentation Map Header

            // Try to parse the fragmentation map header
            var fragmentationMapHeader = ParseFragmentationMapHeader(data);
            if (fragmentationMapHeader == null)
                return null;

            // Set the game cache fragmentation map header
            file.FragmentationMapHeader = fragmentationMapHeader;

            #endregion

            #region Fragmentation Maps

            // Create the fragmentation map array
            file.FragmentationMaps = new FragmentationMap[fragmentationMapHeader.BlockCount];

            // Try to parse the fragmentation maps
            for (int i = 0; i < fragmentationMapHeader.BlockCount; i++)
            {
                var fragmentationMap = ParseFragmentationMap(data);
                if (fragmentationMap == null)
                    return null;

                file.FragmentationMaps[i] = fragmentationMap;
            }

            #endregion

            #region Block Entry Map Header

            if (header.MinorVersion < 6)
            {
                // Try to parse the block entry map header
                var blockEntryMapHeader = ParseBlockEntryMapHeader(data);
                if (blockEntryMapHeader == null)
                    return null;

                // Set the game cache block entry map header
                file.BlockEntryMapHeader = blockEntryMapHeader;
            }

            #endregion

            #region Block Entry Maps

            if (header.MinorVersion < 6)
            {
                // Create the block entry map array
                file.BlockEntryMaps = new BlockEntryMap[file.BlockEntryMapHeader!.BlockCount];

                // Try to parse the block entry maps
                for (int i = 0; i < file.BlockEntryMapHeader.BlockCount; i++)
                {
                    var blockEntryMap = ParseBlockEntryMap(data);
                    if (blockEntryMap == null)
                        return null;

                    file.BlockEntryMaps[i] = blockEntryMap;
                }
            }

            #endregion

            // Cache the current offset
            initialOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectoryHeader(data);
            if (directoryHeader == null)
                return null;

            // Set the game cache directory header
            file.DirectoryHeader = directoryHeader;

            #endregion

            #region Directory Entries

            // Create the directory entry array
            file.DirectoryEntries = new DirectoryEntry[directoryHeader.ItemCount];

            // Try to parse the directory entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);
                if (directoryEntry == null)
                    return null;
                
                file.DirectoryEntries[i] = directoryEntry;
            }

            #endregion

            #region Directory Names

            if (directoryHeader.NameSize > 0)
            {
                // Get the current offset for adjustment
                long directoryNamesStart = data.Position;

                // Get the ending offset
                long directoryNamesEnd = data.Position + directoryHeader.NameSize;

                // Create the string dictionary
                file.DirectoryNames = [];

                // Loop and read the null-terminated strings
                while (data.Position < directoryNamesEnd)
                {
                    long nameOffset = data.Position - directoryNamesStart;
                    string? directoryName = data.ReadNullTerminatedAnsiString();
                    if (data.Position > directoryNamesEnd)
                    {
                        data.Seek(-directoryName?.Length ?? 0, SeekOrigin.Current);
                        byte[] endingData = data.ReadBytes((int)(directoryNamesEnd - data.Position));
                        directoryName = Encoding.ASCII.GetString(endingData);
                    }

                    file.DirectoryNames[nameOffset] = directoryName;
                }
            }

            #endregion

            #region Directory Info 1 Entries

            // Create the directory info 1 entry array
            file.DirectoryInfo1Entries = new DirectoryInfo1Entry[directoryHeader.Info1Count];

            // Try to parse the directory info 1 entries
            for (int i = 0; i < directoryHeader.Info1Count; i++)
            {
                var directoryInfo1Entry = ParseDirectoryInfo1Entry(data);
                if (directoryInfo1Entry == null)
                    return null;

                file.DirectoryInfo1Entries[i] = directoryInfo1Entry;
            }

            #endregion

            #region Directory Info 2 Entries

            // Create the directory info 2 entry array
            file.DirectoryInfo2Entries = new DirectoryInfo2Entry[directoryHeader.ItemCount];

            // Try to parse the directory info 2 entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var directoryInfo2Entry = ParseDirectoryInfo2Entry(data);
                if (directoryInfo2Entry == null)
                    return null;

                file.DirectoryInfo2Entries[i] = directoryInfo2Entry;
            }

            #endregion

            #region Directory Copy Entries

            // Create the directory copy entry array
            file.DirectoryCopyEntries = new DirectoryCopyEntry[directoryHeader.CopyCount];

            // Try to parse the directory copy entries
            for (int i = 0; i < directoryHeader.CopyCount; i++)
            {
                var directoryCopyEntry = ParseDirectoryCopyEntry(data);
                if (directoryCopyEntry == null)
                    return null;

                file.DirectoryCopyEntries[i] = directoryCopyEntry;
            }

            #endregion

            #region Directory Local Entries

            // Create the directory local entry array
            file.DirectoryLocalEntries = new DirectoryLocalEntry[directoryHeader.LocalCount];

            // Try to parse the directory local entries
            for (int i = 0; i < directoryHeader.LocalCount; i++)
            {
                var directoryLocalEntry = ParseDirectoryLocalEntry(data);
                if (directoryLocalEntry == null)
                    return null;

                file.DirectoryLocalEntries[i] = directoryLocalEntry;
            }

            #endregion

            // Seek to end of directory section, just in case
            data.Seek(initialOffset + directoryHeader.DirectorySize, SeekOrigin.Begin);

            #region Directory Map Header

            if (header.MinorVersion >= 5)
            {
                // Try to parse the directory map header
                var directoryMapHeader = ParseDirectoryMapHeader(data);
                if (directoryMapHeader == null)
                    return null;

                // Set the game cache directory map header
                file.DirectoryMapHeader = directoryMapHeader;
            }

            #endregion

            #region Directory Map Entries

            // Create the directory map entry array
            file.DirectoryMapEntries = new DirectoryMapEntry[directoryHeader.ItemCount];

            // Try to parse the directory map entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var directoryMapEntry = ParseDirectoryMapEntry(data);
                if (directoryMapEntry == null)
                    return null;

                file.DirectoryMapEntries[i] = directoryMapEntry;
            }

            #endregion

            #region Checksum Header

            // Try to parse the checksum header
            var checksumHeader = ParseChecksumHeader(data);
            if (checksumHeader == null)
                return null;

            // Set the game cache checksum header
            file.ChecksumHeader = checksumHeader;

            #endregion

            // Cache the current offset
            initialOffset = data.Position;

            #region Checksum Map Header

            // Try to parse the checksum map header
            var checksumMapHeader = ParseChecksumMapHeader(data);
            if (checksumMapHeader == null)
                return null;

            // Set the game cache checksum map header
            file.ChecksumMapHeader = checksumMapHeader;

            #endregion

            #region Checksum Map Entries

            // Create the checksum map entry array
            file.ChecksumMapEntries = new ChecksumMapEntry[checksumMapHeader.ItemCount];

            // Try to parse the checksum map entries
            for (int i = 0; i < checksumMapHeader.ItemCount; i++)
            {
                var checksumMapEntry = ParseChecksumMapEntry(data);
                if (checksumMapEntry == null)
                    return null;

                file.ChecksumMapEntries[i] = checksumMapEntry;
            }

            #endregion

            #region Checksum Entries

            // Create the checksum entry array
            file.ChecksumEntries = new ChecksumEntry[checksumMapHeader.ChecksumCount];

            // Try to parse the checksum entries
            for (int i = 0; i < checksumMapHeader.ChecksumCount; i++)
            {
                var checksumEntry = ParseChecksumEntry(data);
                if (checksumEntry == null)
                    return null;

                file.ChecksumEntries[i] = checksumEntry;
            }

            #endregion

            // Seek to end of checksum section, just in case
            data.Seek(initialOffset + checksumHeader.ChecksumSize, SeekOrigin.Begin);

            #region Data Block Header

            // Try to parse the data block header
            var dataBlockHeader = ParseDataBlockHeader(data, header.MinorVersion);
            if (dataBlockHeader == null)
                return null;

            // Set the game cache data block header
            file.DataBlockHeader = dataBlockHeader;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            var header = data.ReadType<Header>();

            if (header == null)
                return null;
            if (header.Dummy0 != 0x00000001)
                return null;
            if (header.MajorVersion != 0x00000001)
                return null;
            if (header.MinorVersion != 3 && header.MinorVersion != 5 && header.MinorVersion != 6)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry header on success, null on error</returns>
        private static BlockEntryHeader? ParseBlockEntryHeader(Stream data)
        {
            return data.ReadType<BlockEntryHeader>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry on success, null on error</returns>
        private static BlockEntry? ParseBlockEntry(Stream data)
        {
            return data.ReadType<BlockEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache fragmentation map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache fragmentation map header on success, null on error</returns>
        private static FragmentationMapHeader? ParseFragmentationMapHeader(Stream data)
        {
            return data.ReadType<FragmentationMapHeader>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache fragmentation map
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache fragmentation map on success, null on error</returns>
        private static FragmentationMap? ParseFragmentationMap(Stream data)
        {
            return data.ReadType<FragmentationMap>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry map header on success, null on error</returns>
        private static BlockEntryMapHeader? ParseBlockEntryMapHeader(Stream data)
        {
            return data.ReadType<BlockEntryMapHeader>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache block entry map
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache block entry map on success, null on error</returns>
        private static BlockEntryMap? ParseBlockEntryMap(Stream data)
        {
            return data.ReadType<BlockEntryMap>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory header on success, null on error</returns>
        private static DirectoryHeader? ParseDirectoryHeader(Stream data)
        {
            return data.ReadType<DirectoryHeader>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory entry on success, null on error</returns>
        private static DirectoryEntry? ParseDirectoryEntry(Stream data)
        {
            return data.ReadType<DirectoryEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory info 1 entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory info 1 entry on success, null on error</returns>
        private static DirectoryInfo1Entry? ParseDirectoryInfo1Entry(Stream data)
        {
            return data.ReadType<DirectoryInfo1Entry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory info 2 entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory info 2 entry on success, null on error</returns>
        private static DirectoryInfo2Entry? ParseDirectoryInfo2Entry(Stream data)
        {
            return data.ReadType<DirectoryInfo2Entry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory copy entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory copy entry on success, null on error</returns>
        private static DirectoryCopyEntry? ParseDirectoryCopyEntry(Stream data)
        {
            return data.ReadType<DirectoryCopyEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory local entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory local entry on success, null on error</returns>
        private static DirectoryLocalEntry? ParseDirectoryLocalEntry(Stream data)
        {
            return data.ReadType<DirectoryLocalEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory map header on success, null on error</returns>
        private static DirectoryMapHeader? ParseDirectoryMapHeader(Stream data)
        {
            var directoryMapHeader = data.ReadType<DirectoryMapHeader>();

            if (directoryMapHeader == null)
                return null;
            if (directoryMapHeader.Dummy0 != 0x00000001)
                return null;
            if (directoryMapHeader.Dummy1 != 0x00000000)
                return null;

            return directoryMapHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache directory map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache directory map entry on success, null on error</returns>
        private static DirectoryMapEntry? ParseDirectoryMapEntry(Stream data)
        {
            return data.ReadType<DirectoryMapEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum header on success, null on error</returns>
        private static ChecksumHeader? ParseChecksumHeader(Stream data)
        {
            var checksumHeader = data.ReadType<ChecksumHeader>();

            if (checksumHeader == null)
                return null;
            if (checksumHeader.Dummy0 != 0x00000001)
                return null;

            return checksumHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum map header on success, null on error</returns>
        private static ChecksumMapHeader? ParseChecksumMapHeader(Stream data)
        {
            var checksumMapHeader = data.ReadType<ChecksumMapHeader>();

            if (checksumMapHeader == null)
                return null;
            if (checksumMapHeader.Dummy0 != 0x14893721)
                return null;
            if (checksumMapHeader.Dummy1 != 0x00000001)
                return null;

            return checksumMapHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum map entry on success, null on error</returns>
        private static ChecksumMapEntry? ParseChecksumMapEntry(Stream data)
        {
            return data.ReadType<ChecksumMapEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache checksum entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Game Cache checksum entry on success, null on error</returns>
        private static ChecksumEntry? ParseChecksumEntry(Stream data)
        {
            return data.ReadType<ChecksumEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Game Cache data block header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="minorVersion">Minor version field from the header</param>
        /// <returns>Filled Half-Life Game Cache data block header on success, null on error</returns>
        private static DataBlockHeader? ParseDataBlockHeader(Stream data, uint minorVersion)
        {
            var dataBlockHeader = new DataBlockHeader();

            // In version 3 the DataBlockHeader is missing the LastVersionPlayed field.
            if (minorVersion >= 5)
                dataBlockHeader.LastVersionPlayed = data.ReadUInt32();

            dataBlockHeader.BlockCount = data.ReadUInt32();
            dataBlockHeader.BlockSize = data.ReadUInt32();
            dataBlockHeader.FirstBlockOffset = data.ReadUInt32();
            dataBlockHeader.BlocksUsed = data.ReadUInt32();
            dataBlockHeader.Checksum = data.ReadUInt32();

            return dataBlockHeader;
        }
    }
}