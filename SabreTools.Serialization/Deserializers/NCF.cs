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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new Half-Life No Cache to fill
                var file = new Models.NCF.File();

                #region Header

                // Try to parse the header
                var header = data.ReadType<Header>();
                if (header?.Dummy0 != 0x00000001)
                    return null;
                if (header?.MajorVersion != 0x00000002)
                    return null;
                if (header?.MinorVersion != 1)
                    return null;

                // Set the no cache header
                file.Header = header;

                #endregion

                // Cache the current offset
                long initialOffset = data.Position;

                #region Directory Header

                // Try to parse the directory header
                var directoryHeader = data.ReadType<DirectoryHeader>();
                if (directoryHeader?.Dummy0 != 0x00000004)
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
                    file.DirectoryNames = new Dictionary<long, string?>();

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

                #region Unknown Header

                // Try to parse the unknown header
                var unknownHeader = data.ReadType<UnknownHeader>();
                if (unknownHeader?.Dummy0 != 0x00000001)
                    return null;
                if (unknownHeader?.Dummy1 != 0x00000000)
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
                    var unknownEntry = data.ReadType<UnknownEntry>();
                    if (unknownEntry == null)
                        return null;

                    file.UnknownEntries[i] = unknownEntry;
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

                return file;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}