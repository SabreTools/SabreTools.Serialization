using System.IO;
using System.Text;
using SabreTools.Data.Models.GCF;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class GCF : BaseBinaryReader<Data.Models.GCF.File>
    {
        /// <inheritdoc/>
        public override Data.Models.GCF.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Half-Life Game Cache to fill
                var file = new Data.Models.GCF.File();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Dummy0 != 0x00000001)
                    return null;
                if (header.MajorVersion != 0x00000001)
                    return null;
                if (header.MinorVersion != 3 && header.MinorVersion != 5 && header.MinorVersion != 6)
                    return null;

                // Set the game cache header
                file.Header = header;

                #endregion

                #region Block Entry Header

                // Set the game cache block entry header
                file.BlockEntryHeader = ParseBlockEntryHeader(data);

                #endregion

                #region Block Entries

                // Create the block entry array
                file.BlockEntries = new BlockEntry[file.BlockEntryHeader.BlockCount];

                // Try to parse the block entries
                for (int i = 0; i < file.BlockEntryHeader.BlockCount; i++)
                {
                    file.BlockEntries[i] = ParseBlockEntry(data);
                }

                #endregion

                #region Fragmentation Map Header

                // Set the game cache fragmentation map header
                file.FragmentationMapHeader = ParseFragmentationMapHeader(data);

                #endregion

                #region Fragmentation Maps

                // Create the fragmentation map array
                file.FragmentationMaps = new FragmentationMap[file.FragmentationMapHeader.BlockCount];

                // Try to parse the fragmentation maps
                for (int i = 0; i < file.FragmentationMapHeader.BlockCount; i++)
                {
                    file.FragmentationMaps[i] = ParseFragmentationMap(data);
                }

                #endregion

                #region Block Entry Map Header

                // Set the game cache block entry map header
                if (header.MinorVersion < 6)
                    file.BlockEntryMapHeader = ParseBlockEntryMapHeader(data);

                #endregion

                #region Block Entry Maps

                if (header.MinorVersion < 6)
                {
                    // Create the block entry map array
                    file.BlockEntryMaps = new BlockEntryMap[file.BlockEntryMapHeader!.BlockCount];

                    // Try to parse the block entry maps
                    for (int i = 0; i < file.BlockEntryMapHeader.BlockCount; i++)
                    {
                        file.BlockEntryMaps[i] = ParseBlockEntryMap(data);
                    }
                }

                #endregion

                // Cache the current offset
                long afterMapPosition = data.Position;

                #region Directory Header

                // Try to parse game cache directory header
                var directoryHeader = ParseDirectoryHeader(data);
                if (directoryHeader.Dummy0 != 0x00000004)
                    return null;
                if (directoryHeader.Dummy1 != 0x00008000)
                    return null;

                // Set the game cache directory header
                file.DirectoryHeader = directoryHeader;

                #endregion

                #region Directory Entries

                // Create the directory entry array
                file.DirectoryEntries = new DirectoryEntry[file.DirectoryHeader.ItemCount];

                // Try to parse the directory entries
                for (int i = 0; i < file.DirectoryHeader.ItemCount; i++)
                {
                    file.DirectoryEntries[i] = ParseDirectoryEntry(data);
                }

                #endregion

                #region Directory Names

                if (file.DirectoryHeader.NameSize > 0)
                {
                    // Get the current offset for adjustment
                    long directoryNamesStart = data.Position;

                    // Get the ending offset
                    long directoryNamesEnd = data.Position + file.DirectoryHeader.NameSize;

                    // Create the string dictionary
                    file.DirectoryNames = [];

                    // Loop and read the null-terminated strings
                    while (data.Position < directoryNamesEnd)
                    {
                        long nameOffset = data.Position - directoryNamesStart;
                        string? directoryName = data.ReadNullTerminatedAnsiString();
                        if (data.Position > directoryNamesEnd)
                        {
                            data.SeekIfPossible(-directoryName?.Length ?? 0, SeekOrigin.Current);
                            byte[] endingData = data.ReadBytes((int)(directoryNamesEnd - data.Position));
                            directoryName = Encoding.ASCII.GetString(endingData);
                        }

                        file.DirectoryNames[nameOffset] = directoryName;
                    }
                }

                #endregion

                #region Directory Info 1 Entries

                // Create the directory info 1 entry array
                file.DirectoryInfo1Entries = new DirectoryInfo1Entry[file.DirectoryHeader.Info1Count];

                // Try to parse the directory info 1 entries
                for (int i = 0; i < file.DirectoryHeader.Info1Count; i++)
                {
                    file.DirectoryInfo1Entries[i] = ParseDirectoryInfo1Entry(data);
                }

                #endregion

                #region Directory Info 2 Entries

                // Create the directory info 2 entry array
                file.DirectoryInfo2Entries = new DirectoryInfo2Entry[file.DirectoryHeader.ItemCount];

                // Try to parse the directory info 2 entries
                for (int i = 0; i < file.DirectoryHeader.ItemCount; i++)
                {
                    file.DirectoryInfo2Entries[i] = ParseDirectoryInfo2Entry(data);
                }

                #endregion

                #region Directory Copy Entries

                // Create the directory copy entry array
                file.DirectoryCopyEntries = new DirectoryCopyEntry[file.DirectoryHeader.CopyCount];

                // Try to parse the directory copy entries
                for (int i = 0; i < file.DirectoryHeader.CopyCount; i++)
                {
                    file.DirectoryCopyEntries[i] = ParseDirectoryCopyEntry(data);
                }

                #endregion

                #region Directory Local Entries

                // Create the directory local entry array
                file.DirectoryLocalEntries = new DirectoryLocalEntry[file.DirectoryHeader.LocalCount];

                // Try to parse the directory local entries
                for (int i = 0; i < file.DirectoryHeader.LocalCount; i++)
                {
                    file.DirectoryLocalEntries[i] = ParseDirectoryLocalEntry(data);
                }

                #endregion

                // Seek to end of directory section, just in case
                data.SeekIfPossible(afterMapPosition + file.DirectoryHeader.DirectorySize, SeekOrigin.Begin);

                #region Directory Map Header

                if (header.MinorVersion >= 5)
                {
                    // Try to parse the directory map header
                    var directoryMapHeader = ParseDirectoryMapHeader(data);
                    if (directoryMapHeader.Dummy0 != 0x00000001)
                        return null;
                    if (directoryMapHeader.Dummy1 != 0x00000000)
                        return null;

                    // Set the game cache directory map header
                    file.DirectoryMapHeader = directoryMapHeader;
                }

                #endregion

                #region Directory Map Entries

                // Create the directory map entry array
                file.DirectoryMapEntries = new DirectoryMapEntry[file.DirectoryHeader.ItemCount];

                // Try to parse the directory map entries
                for (int i = 0; i < file.DirectoryHeader.ItemCount; i++)
                {
                    file.DirectoryMapEntries[i] = ParseDirectoryMapEntry(data);
                }

                #endregion

                #region Checksum Header

                // Try to parse the checksum header
                var checksumHeader = ParseChecksumHeader(data);
                if (checksumHeader?.Dummy0 != 0x00000001)
                    return null;

                // Set the game cache checksum header
                file.ChecksumHeader = checksumHeader;

                #endregion

                // Cache the current offset
                long afterChecksumPosition = data.Position;

                #region Checksum Map Header

                // Try to parse the checksum map header
                var checksumMapHeader = ParseChecksumMapHeader(data);
                if (checksumMapHeader?.Dummy0 != 0x14893721)
                    return null;
                if (checksumMapHeader?.Dummy1 != 0x00000001)
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
                    file.ChecksumMapEntries[i] = ParseChecksumMapEntry(data);
                }

                #endregion

                #region Checksum Entries

                // Create the checksum entry array
                file.ChecksumEntries = new ChecksumEntry[checksumMapHeader.ChecksumCount];

                // Try to parse the checksum entries
                for (int i = 0; i < checksumMapHeader.ChecksumCount; i++)
                {
                    file.ChecksumEntries[i] = ParseChecksumEntry(data);
                }

                #endregion

                // Seek to end of checksum section, just in case
                data.SeekIfPossible(afterChecksumPosition + checksumHeader.ChecksumSize, SeekOrigin.Begin);

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
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a BlockEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BlockEntry on success, null on error</returns>
        public static BlockEntry ParseBlockEntry(Stream data)
        {
            var obj = new BlockEntry();

            obj.EntryFlags = data.ReadUInt32LittleEndian();
            obj.FileDataOffset = data.ReadUInt32LittleEndian();
            obj.FileDataSize = data.ReadUInt32LittleEndian();
            obj.FirstDataBlockIndex = data.ReadUInt32LittleEndian();
            obj.NextBlockEntryIndex = data.ReadUInt32LittleEndian();
            obj.PreviousBlockEntryIndex = data.ReadUInt32LittleEndian();
            obj.DirectoryIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a BlockEntryHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BlockEntryHeader on success, null on error</returns>
        public static BlockEntryHeader ParseBlockEntryHeader(Stream data)
        {
            var obj = new BlockEntryHeader();

            obj.BlockCount = data.ReadUInt32LittleEndian();
            obj.BlocksUsed = data.ReadUInt32LittleEndian();
            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();
            obj.Dummy2 = data.ReadUInt32LittleEndian();
            obj.Dummy3 = data.ReadUInt32LittleEndian();
            obj.Dummy4 = data.ReadUInt32LittleEndian();
            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a BlockEntryMap
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BlockEntryMap on success, null on error</returns>
        public static BlockEntryMap ParseBlockEntryMap(Stream data)
        {
            var obj = new BlockEntryMap();

            obj.PreviousBlockEntryIndex = data.ReadUInt32LittleEndian();
            obj.NextBlockEntryIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a BlockEntryMapHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BlockEntryMapHeader on success, null on error</returns>
        public static BlockEntryMapHeader ParseBlockEntryMapHeader(Stream data)
        {
            var obj = new BlockEntryMapHeader();

            obj.BlockCount = data.ReadUInt32LittleEndian();
            obj.FirstBlockEntryIndex = data.ReadUInt32LittleEndian();
            obj.LastBlockEntryIndex = data.ReadUInt32LittleEndian();
            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ChecksumEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ChecksumEntry on success, null on error</returns>
        public static ChecksumEntry ParseChecksumEntry(Stream data)
        {
            var obj = new ChecksumEntry();

            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ChecksumHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ChecksumHeader on success, null on error</returns>
        public static ChecksumHeader ParseChecksumHeader(Stream data)
        {
            var obj = new ChecksumHeader();

            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.ChecksumSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ChecksumMapEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ChecksumMapEntry on success, null on error</returns>
        public static ChecksumMapEntry ParseChecksumMapEntry(Stream data)
        {
            var obj = new ChecksumMapEntry();

            obj.ChecksumCount = data.ReadUInt32LittleEndian();
            obj.FirstChecksumIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ChecksumMapHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ChecksumMapHeader on success, null on error</returns>
        public static ChecksumMapHeader ParseChecksumMapHeader(Stream data)
        {
            var obj = new ChecksumMapHeader();

            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();
            obj.ItemCount = data.ReadUInt32LittleEndian();
            obj.ChecksumCount = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DataBlockHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="minorVersion">Minor version field from the header</param>
        /// <returns>Filled DataBlockHeader on success, null on error</returns>
        public static DataBlockHeader ParseDataBlockHeader(Stream data, uint minorVersion)
        {
            var obj = new DataBlockHeader();

            // In version 3 the DataBlockHeader is missing the LastVersionPlayed field.
            if (minorVersion >= 5)
                obj.LastVersionPlayed = data.ReadUInt32LittleEndian();

            obj.BlockCount = data.ReadUInt32LittleEndian();
            obj.BlockSize = data.ReadUInt32LittleEndian();
            obj.FirstBlockOffset = data.ReadUInt32LittleEndian();
            obj.BlocksUsed = data.ReadUInt32LittleEndian();
            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryCopyEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryCopyEntry on success, null on error</returns>
        public static DirectoryCopyEntry ParseDirectoryCopyEntry(Stream data)
        {
            var obj = new DirectoryCopyEntry();

            obj.DirectoryIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryEntry on success, null on error</returns>
        public static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            var obj = new DirectoryEntry();

            obj.NameOffset = data.ReadUInt32LittleEndian();
            obj.ItemSize = data.ReadUInt32LittleEndian();
            obj.ChecksumIndex = data.ReadUInt32LittleEndian();
            obj.DirectoryFlags = (HL_GCF_FLAG)data.ReadUInt32LittleEndian();
            obj.ParentIndex = data.ReadUInt32LittleEndian();
            obj.NextIndex = data.ReadUInt32LittleEndian();
            obj.FirstIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryHeader on success, null on error</returns>
        public static DirectoryHeader ParseDirectoryHeader(Stream data)
        {
            var obj = new DirectoryHeader();

            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.CacheID = data.ReadUInt32LittleEndian();
            obj.LastVersionPlayed = data.ReadUInt32LittleEndian();
            obj.ItemCount = data.ReadUInt32LittleEndian();
            obj.FileCount = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();
            obj.DirectorySize = data.ReadUInt32LittleEndian();
            obj.NameSize = data.ReadUInt32LittleEndian();
            obj.Info1Count = data.ReadUInt32LittleEndian();
            obj.CopyCount = data.ReadUInt32LittleEndian();
            obj.LocalCount = data.ReadUInt32LittleEndian();
            obj.Dummy2 = data.ReadUInt32LittleEndian();
            obj.Dummy3 = data.ReadUInt32LittleEndian();
            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryInfo1Entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryInfo1Entry on success, null on error</returns>
        public static DirectoryInfo1Entry ParseDirectoryInfo1Entry(Stream data)
        {
            var obj = new DirectoryInfo1Entry();

            obj.Dummy0 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryInfo2Entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryInfo2Entry on success, null on error</returns>
        public static DirectoryInfo2Entry ParseDirectoryInfo2Entry(Stream data)
        {
            var obj = new DirectoryInfo2Entry();

            obj.Dummy0 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryLocalEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryLocalEntry on success, null on error</returns>
        public static DirectoryLocalEntry ParseDirectoryLocalEntry(Stream data)
        {
            var obj = new DirectoryLocalEntry();

            obj.DirectoryIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryMapEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryMapEntry on success, null on error</returns>
        public static DirectoryMapEntry ParseDirectoryMapEntry(Stream data)
        {
            var obj = new DirectoryMapEntry();

            obj.FirstBlockIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryMapHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryMapHeader on success, null on error</returns>
        public static DirectoryMapHeader ParseDirectoryMapHeader(Stream data)
        {
            var obj = new DirectoryMapHeader();

            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FragmentationMap
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FragmentationMap on success, null on error</returns>
        public static FragmentationMap ParseFragmentationMap(Stream data)
        {
            var obj = new FragmentationMap();

            obj.NextDataBlockIndex = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FragmentationMapHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FragmentationMapHeader on success, null on error</returns>
        public static FragmentationMapHeader ParseFragmentationMapHeader(Stream data)
        {
            var obj = new FragmentationMapHeader();

            obj.BlockCount = data.ReadUInt32LittleEndian();
            obj.FirstUnusedEntry = data.ReadUInt32LittleEndian();
            obj.Terminator = data.ReadUInt32LittleEndian();
            obj.Checksum = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.MajorVersion = data.ReadUInt32LittleEndian();
            obj.MinorVersion = data.ReadUInt32LittleEndian();
            obj.CacheID = data.ReadUInt32LittleEndian();
            obj.LastVersionPlayed = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();
            obj.Dummy2 = data.ReadUInt32LittleEndian();
            obj.FileSize = data.ReadUInt32LittleEndian();
            obj.BlockSize = data.ReadUInt32LittleEndian();
            obj.BlockCount = data.ReadUInt32LittleEndian();
            obj.Dummy3 = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
