using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.VPK;
using static SabreTools.Models.VPK.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class VPK : BaseBinaryDeserializer<Models.VPK.File>
    {
        /// <inheritdoc/>
        public override Models.VPK.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Create a new Valve Package to fill
            var file = new Models.VPK.File();

            #region Header

            // Try to parse the header
            // The original version had no signature.
            var header = data.ReadType<Header>();
            if (header?.Signature != SignatureUInt32)
                return null;
            if (header.Version > 2)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            #region Extended Header

            if (header.Version == 2)
            {
                // Try to parse the extended header
                var extendedHeader = data.ReadType<ExtendedHeader>();
                if (extendedHeader == null)
                    return null;

                // Set the package extended header
                file.ExtendedHeader = extendedHeader;
            }

            #endregion

            #region Directory Items

            // Create the directory items tree
            var directoryItems = ParseDirectoryItemTree(data);
            if (directoryItems == null)
                return null;

            // Set the directory items
            file.DirectoryItems = directoryItems;

            #endregion

            #region Archive Hashes

            if (header?.Version == 2
                && file.ExtendedHeader != null
                && file.ExtendedHeader.ArchiveMD5SectionSize > 0
                && data.Position + file.ExtendedHeader.ArchiveMD5SectionSize <= data.Length)
            {
                // Create the archive hashes list
                var archiveHashes = new List<ArchiveHash>();

                // Cache the current offset
                long initialOffset = data.Position;

                // Try to parse the directory items
                while (data.Position < initialOffset + file.ExtendedHeader.ArchiveMD5SectionSize)
                {
                    var archiveHash = data.ReadType<ArchiveHash>();
                    if (archiveHash == null)
                        return null;
                    
                    archiveHashes.Add(archiveHash);
                }

                file.ArchiveHashes = [.. archiveHashes];
            }

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory item tree
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package directory item tree on success, null on error</returns>
        private static DirectoryItem[]? ParseDirectoryItemTree(Stream data)
        {
            // Create the directory items list
            var directoryItems = new List<DirectoryItem>();

            while (true)
            {
                // Get the extension
                string? extensionString = data.ReadNullTerminatedAnsiString();
                if (string.IsNullOrEmpty(extensionString))
                    break;

                // Sanitize the extension
                for (int i = 0; i < 0x20; i++)
                {
                    extensionString = extensionString!.Replace($"{(char)i}", string.Empty);
                }

                while (true)
                {
                    // Get the path
                    string? pathString = data.ReadNullTerminatedAnsiString();
                    if (string.IsNullOrEmpty(pathString))
                        break;

                    // Sanitize the path
                    for (int i = 0; i < 0x20; i++)
                    {
                        pathString = pathString!.Replace($"{(char)i}", string.Empty);
                    }

                    while (true)
                    {
                        // Get the name
                        string? nameString = data.ReadNullTerminatedAnsiString();
                        if (string.IsNullOrEmpty(nameString))
                            break;

                        // Sanitize the name
                        for (int i = 0; i < 0x20; i++)
                        {
                            nameString = nameString!.Replace($"{(char)i}", string.Empty);
                        }

                        // Get the directory item
                        var directoryItem = ParseDirectoryItem(data, extensionString!, pathString!, nameString!);
                        if (directoryItem == null)
                            return null;

                        // Add the directory item
                        directoryItems.Add(directoryItem);
                    }
                }
            }

            return [.. directoryItems];
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory item
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Valve Package directory item on success, null on error</returns>
        private static DirectoryItem? ParseDirectoryItem(Stream data, string extension, string path, string name)
        {
            var directoryItem = new DirectoryItem();

            directoryItem.Extension = extension;
            directoryItem.Path = path;
            directoryItem.Name = name;

            // Get the directory entry
            var directoryEntry = data.ReadType<DirectoryEntry>();
            if (directoryEntry == null)
                return null;

            // Set the directory entry
            directoryItem.DirectoryEntry = directoryEntry;

            // Get the preload data pointer
            long preloadDataPointer = -1; int preloadDataLength = -1;
            if (directoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE
                && directoryEntry.EntryLength > 0
                && data.Position + directoryEntry.EntryLength <= data.Length)
            {
                preloadDataPointer = directoryEntry.EntryOffset;
                preloadDataLength = (int)directoryEntry.EntryLength;
            }
            else if (directoryEntry.PreloadBytes > 0)
            {
                preloadDataPointer = data.Position;
                preloadDataLength = directoryEntry.PreloadBytes;
            }

            // If we had a valid preload data pointer
            byte[]? preloadData = null;
            if (preloadDataPointer >= 0
                && preloadDataLength > 0
                && data.Position + preloadDataLength <= data.Length)
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Seek to the preload data offset
                data.Seek(preloadDataPointer, SeekOrigin.Begin);

                // Read the preload data
                preloadData = data.ReadBytes(preloadDataLength);

                // Seek back to the original offset
                data.Seek(initialOffset, SeekOrigin.Begin);
            }

            // Set the preload data
            directoryItem.PreloadData = preloadData;

            return directoryItem;
        }
    }
}