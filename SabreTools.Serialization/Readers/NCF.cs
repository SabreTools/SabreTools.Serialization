using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.NCF;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class NCF : BaseBinaryReader<Data.Models.NCF.File>
    {
        /// <inheritdoc/>
        public override Data.Models.NCF.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Half-Life No Cache to fill
                var file = new Data.Models.NCF.File();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header.Dummy0 != 0x00000001)
                    return null;
                if (header.MajorVersion != 0x00000002)
                    return null;
                if (header.MinorVersion != 1)
                    return null;

                // Set the no cache header
                file.Header = header;

                #endregion

                // Cache the current offset
                long afterHeaderPosition = data.Position;

                #region Directory Header

                // Try to parse the directory header
                var directoryHeader = ParseDirectoryHeader(data);
                if (directoryHeader.Dummy0 != 0x00000004)
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
                    file.DirectoryEntries[i] = ParseDirectoryEntry(data);
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
                file.DirectoryInfo1Entries = new DirectoryInfo1Entry[directoryHeader.Info1Count];

                // Try to parse the directory info 1 entries
                for (int i = 0; i < directoryHeader.Info1Count; i++)
                {
                    file.DirectoryInfo1Entries[i] = ParseDirectoryInfo1Entry(data);
                }

                #endregion

                #region Directory Info 2 Entries

                // Create the directory info 2 entry array
                file.DirectoryInfo2Entries = new DirectoryInfo2Entry[directoryHeader.ItemCount];

                // Try to parse the directory info 2 entries
                for (int i = 0; i < directoryHeader.ItemCount; i++)
                {
                    file.DirectoryInfo2Entries[i] = ParseDirectoryInfo2Entry(data);
                }

                #endregion

                #region Directory Copy Entries

                // Create the directory copy entry array
                file.DirectoryCopyEntries = new DirectoryCopyEntry[directoryHeader.CopyCount];

                // Try to parse the directory copy entries
                for (int i = 0; i < directoryHeader.CopyCount; i++)
                {
                    file.DirectoryCopyEntries[i] = ParseDirectoryCopyEntry(data);
                }

                #endregion

                #region Directory Local Entries

                // Create the directory local entry array
                file.DirectoryLocalEntries = new DirectoryLocalEntry[directoryHeader.LocalCount];

                // Try to parse the directory local entries
                for (int i = 0; i < directoryHeader.LocalCount; i++)
                {
                    file.DirectoryLocalEntries[i] = ParseDirectoryLocalEntry(data);
                }

                #endregion

                // Seek to end of directory section, just in case
                data.SeekIfPossible(afterHeaderPosition + directoryHeader.DirectorySize, SeekOrigin.Begin);

                #region Unknown Header

                // Try to parse the unknown header
                var unknownHeader = ParseUnknownHeader(data);
                if (unknownHeader.Dummy0 != 0x00000001)
                    return null;
                if (unknownHeader.Dummy1 != 0x00000000)
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
                    file.UnknownEntries[i] = ParseUnknownEntry(data);
                }

                #endregion

                #region Checksum Header

                // Try to parse the checksum header
                var checksumHeader = ParseChecksumHeader(data);
                if (checksumHeader.Dummy0 != 0x00000001)
                    return null;

                // Set the game cache checksum header
                file.ChecksumHeader = checksumHeader;

                #endregion

                // Cache the current offset
                initialOffset = data.Position;

                #region Checksum Map Header

                // Try to parse the checksum map header
                var checksumMapHeader = ParseChecksumMapHeader(data);
                if (checksumMapHeader.Dummy0 != 0x14893721)
                    return null;
                if (checksumMapHeader.Dummy1 != 0x00000001)
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
                data.SeekIfPossible(afterHeaderPosition + checksumHeader.ChecksumSize, SeekOrigin.Begin);

                return file;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
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
            obj.DirectoryFlags = (HL_NCF_FLAG)data.ReadUInt32LittleEndian();
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
            obj.ChecksumDataLength = data.ReadUInt32LittleEndian();
            obj.DirectorySize = data.ReadUInt32LittleEndian();
            obj.NameSize = data.ReadUInt32LittleEndian();
            obj.Info1Count = data.ReadUInt32LittleEndian();
            obj.CopyCount = data.ReadUInt32LittleEndian();
            obj.LocalCount = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();
            obj.Dummy2 = data.ReadUInt32LittleEndian();
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

        /// <summary>
        /// Parse a Stream into a UnknownEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled UnknownEntry on success, null on error</returns>
        public static UnknownEntry ParseUnknownEntry(Stream data)
        {
            var obj = new UnknownEntry();

            obj.Dummy0 = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a UnknownHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled UnknownHeader on success, null on error</returns>
        public static UnknownHeader ParseUnknownHeader(Stream data)
        {
            var obj = new UnknownHeader();

            obj.Dummy0 = data.ReadUInt32LittleEndian();
            obj.Dummy1 = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
