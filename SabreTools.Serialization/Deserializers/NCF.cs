using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.NCF;

namespace SabreTools.Serialization.Deserializers
{
    public class NCF : BaseBinaryDeserializer<Models.NCF.File>
    {
        /// <inheritdoc/>
        public override Models.NCF.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life No Cache to fill
            var file = new Models.NCF.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the no cache header
            file.Header = header;

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
                file.DirectoryNames = new Dictionary<long, string?>();

                // Loop and read the null-terminated strings
                while (data.Position < directoryNamesEnd)
                {
                    long nameOffset = data.Position - directoryNamesStart;
                    string? directoryName = data.ReadNullTerminatedAnsiString();
                    if (data.Position > directoryNamesEnd)
                    {
                        data.Seek(-directoryName?.Length ?? 0, SeekOrigin.Current);
                        byte[]? endingData = data.ReadBytes((int)(directoryNamesEnd - data.Position));
                        if (endingData != null)
                            directoryName = Encoding.ASCII.GetString(endingData);
                        else
                            directoryName = null;
                    }

                    file.DirectoryNames[nameOffset] = directoryName;
                }

                // Loop and assign to entries
                foreach (var directoryEntry in file.DirectoryEntries)
                {
                    if (directoryEntry != null)
                        directoryEntry.Name = file.DirectoryNames[directoryEntry.NameOffset];
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

            #region Unknown Header

            // Try to parse the unknown header
            var unknownHeader = ParseUnknownHeader(data);
            if (unknownHeader == null)
                return null;

            // Set the game cache unknown header
            file.UnknownHeader = unknownHeader;

            #endregion

            #region Unknown Entries

            // Create the unknown entry array
            file.UnknownEntries = new UnknownEntry[directoryHeader.ItemCount];

            // Try to parse the unknown entries
            for (int i = 0; i < directoryHeader.ItemCount; i++)
            {
                var unknownEntry = ParseUnknownEntry(data);
                if (unknownEntry == null)
                    return null;

                file.UnknownEntries[i] = unknownEntry;
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

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            var header = data.ReadType<Header>();

            if (header == null)
                return null;
            if (header.Dummy0 != 0x00000001)
                return null;
            if (header.MajorVersion != 0x00000002)
                return null;
            if (header.MinorVersion != 1)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache directory header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache directory header on success, null on error</returns>
        private static DirectoryHeader? ParseDirectoryHeader(Stream data)
        {
            var directoryHeader = data.ReadType<DirectoryHeader>();

            if (directoryHeader == null)
                return null;
            if (directoryHeader.Dummy0 != 0x00000004)
                return null;

            return directoryHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache directory entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache directory entry on success, null on error</returns>
        private static DirectoryEntry? ParseDirectoryEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var directoryEntry = new DirectoryEntry();

            directoryEntry.NameOffset = data.ReadUInt32();
            directoryEntry.ItemSize = data.ReadUInt32();
            directoryEntry.ChecksumIndex = data.ReadUInt32();
            directoryEntry.DirectoryFlags = (HL_NCF_FLAG)data.ReadUInt32();
            directoryEntry.ParentIndex = data.ReadUInt32();
            directoryEntry.NextIndex = data.ReadUInt32();
            directoryEntry.FirstIndex = data.ReadUInt32();

            return directoryEntry;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache directory info 1 entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache directory info 1 entry on success, null on error</returns>
        private static DirectoryInfo1Entry? ParseDirectoryInfo1Entry(Stream data)
        {
            return data.ReadType<DirectoryInfo1Entry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache directory info 2 entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache directory info 2 entry on success, null on error</returns>
        private static DirectoryInfo2Entry? ParseDirectoryInfo2Entry(Stream data)
        {
            return data.ReadType<DirectoryInfo2Entry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache directory copy entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache directory copy entry on success, null on error</returns>
        private static DirectoryCopyEntry? ParseDirectoryCopyEntry(Stream data)
        {
            return data.ReadType<DirectoryCopyEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache directory local entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache directory local entry on success, null on error</returns>
        private static DirectoryLocalEntry? ParseDirectoryLocalEntry(Stream data)
        {
            return data.ReadType<DirectoryLocalEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache unknown header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache unknown header on success, null on error</returns>
        private static UnknownHeader? ParseUnknownHeader(Stream data)
        {
            var unknownHeader = data.ReadType<UnknownHeader>();

            if (unknownHeader == null)
                return null;
            if (unknownHeader.Dummy0 != 0x00000001)
                return null;
            if (unknownHeader.Dummy1 != 0x00000000)
                return null;

            return unknownHeader;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache unknown entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cacheunknown entry on success, null on error</returns>
        private static UnknownEntry? ParseUnknownEntry(Stream data)
        {
            return data.ReadType<UnknownEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache checksum header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache checksum header on success, null on error</returns>
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
        /// Parse a Stream into a Half-Life No Cache checksum map header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache checksum map header on success, null on error</returns>
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
        /// Parse a Stream into a Half-Life No Cache checksum map entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache checksum map entry on success, null on error</returns>
        private static ChecksumMapEntry? ParseChecksumMapEntry(Stream data)
        {
            return data.ReadType<ChecksumMapEntry>();
        }

        /// <summary>
        /// Parse a Stream into a Half-Life No Cache checksum entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life No Cache checksum entry on success, null on error</returns>
        private static ChecksumEntry? ParseChecksumEntry(Stream data)
        {
            return data.ReadType<ChecksumEntry>();
        }
    }
}