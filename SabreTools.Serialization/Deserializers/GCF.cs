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
            if (data == null || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new Half-Life Game Cache to fill
            var file = new Models.GCF.File();

            #region Header

            // Try to parse the header
            var header = data.ReadType<Header>();
            if (header?.Dummy0 != 0x00000001)
                return null;
            if (header?.MajorVersion != 0x00000001)
                return null;
            if (header.MinorVersion != 3 && header.MinorVersion != 5 && header.MinorVersion != 6)
                return null;

            // Set the game cache header
            file.Header = header;

            #endregion

            #region Block Entry Header

            // Try to parse the block entry header
            var blockEntryHeader = data.ReadType<BlockEntryHeader>();
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
                var blockEntry = data.ReadType<BlockEntry>();
                if (blockEntry == null)
                    return null;

                file.BlockEntries[i] = blockEntry;
            }

            #endregion

            #region Fragmentation Map Header

            // Try to parse the fragmentation map header
            var fragmentationMapHeader = data.ReadType<FragmentationMapHeader>();
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
                var fragmentationMap = data.ReadType<FragmentationMap>();
                if (fragmentationMap == null)
                    return null;

                file.FragmentationMaps[i] = fragmentationMap;
            }

            #endregion

            #region Block Entry Map Header

            if (header.MinorVersion < 6)
            {
                // Try to parse the block entry map header
                var blockEntryMapHeader = data.ReadType<BlockEntryMapHeader>();
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
                    var blockEntryMap = data.ReadType<BlockEntryMap>();
                    if (blockEntryMap == null)
                        return null;

                    file.BlockEntryMaps[i] = blockEntryMap;
                }
            }

            #endregion

            // Cache the current offset
            long initialOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = data.ReadType<DirectoryHeader>();
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
                var directoryEntry = data.ReadType<DirectoryEntry>();
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
                var directoryInfo1Entry = data.ReadType<DirectoryInfo1Entry>();
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
                var directoryInfo2Entry = data.ReadType<DirectoryInfo2Entry>();
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
                var directoryCopyEntry = data.ReadType<DirectoryCopyEntry>();
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
                var directoryLocalEntry = data.ReadType<DirectoryLocalEntry>();
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
                var directoryMapHeader = data.ReadType<DirectoryMapHeader>();
            if (directoryMapHeader?.Dummy0 != 0x00000001)
                return null;
            if (directoryMapHeader?.Dummy1 != 0x00000000)
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
                var directoryMapEntry = data.ReadType<DirectoryMapEntry>();
                if (directoryMapEntry == null)
                    return null;

                file.DirectoryMapEntries[i] = directoryMapEntry;
            }

            #endregion

            #region Checksum Header

            // Try to parse the checksum header
            var checksumHeader = data.ReadType<ChecksumHeader>();
            if (checksumHeader?.Dummy0 != 0x00000001)
                return null;

            // Set the game cache checksum header
            file.ChecksumHeader = checksumHeader;

            #endregion

            // Cache the current offset
            initialOffset = data.Position;

            #region Checksum Map Header

            // Try to parse the checksum map header
            var checksumMapHeader = data.ReadType<ChecksumMapHeader>();
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
                var checksumMapEntry = data.ReadType<ChecksumMapEntry>();
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
                var checksumEntry = data.ReadType<ChecksumEntry>();
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