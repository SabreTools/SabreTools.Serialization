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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Valve Package to fill
                var file = new Models.VPK.File();

                #region Header

                // The original version had no signature.
                var header = ParseHeader(data);
                if (header.Signature != SignatureUInt32)
                    return null;
                if (header.Version > 2)
                    return null;

                // Set the package header
                file.Header = header;

                #endregion

                #region Extended Header

                // Set the package extended header
                if (header.Version == 2)
                    file.ExtendedHeader = ParseExtendedHeader(data);

                #endregion

                #region Directory Items

                // Set the directory items
                file.DirectoryItems = ParseDirectoryItemTree(data, initialOffset);

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
                    long afterHeaderOffset = data.Position;

                    // Try to parse the directory items
                    while (data.Position < afterHeaderOffset + file.ExtendedHeader.ArchiveMD5SectionSize)
                    {
                        var archiveHash = ParseArchiveHash(data);
                        archiveHashes.Add(archiveHash);
                    }

                    file.ArchiveHashes = [.. archiveHashes];
                }

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
        /// Parse a Stream into a ArchiveHash
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ArchiveHash on success, null on error</returns>
        public static ArchiveHash ParseArchiveHash(Stream data)
        {
            var obj = new ArchiveHash();

            obj.ArchiveIndex = data.ReadUInt32LittleEndian();
            obj.ArchiveOffset = data.ReadUInt32LittleEndian();
            obj.Length = data.ReadUInt32LittleEndian();
            obj.Hash = data.ReadBytes(0x10);

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

            obj.CRC = data.ReadUInt32LittleEndian();
            obj.PreloadBytes = data.ReadUInt16LittleEndian();
            obj.ArchiveIndex = data.ReadUInt16LittleEndian();
            obj.EntryOffset = data.ReadUInt32LittleEndian();
            obj.EntryLength = data.ReadUInt32LittleEndian();
            obj.Dummy0 = data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory item
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <returns>Filled Valve Package directory item on success, null on error</returns>
        public static DirectoryItem ParseDirectoryItem(Stream data, long initialOffset, string extension, string path, string name)
        {
            var obj = new DirectoryItem();

            obj.Extension = extension;
            obj.Path = path;
            obj.Name = name;

            // Set the directory entry
            obj.DirectoryEntry = ParseDirectoryEntry(data);

            // Get the preload data pointer
            long preloadDataPointer = -1; int preloadDataLength = -1;
            if (obj.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE
                && obj.DirectoryEntry.EntryLength > 0
                && data.Position + obj.DirectoryEntry.EntryLength <= data.Length)
            {
                preloadDataPointer = initialOffset + obj.DirectoryEntry.EntryOffset;
                preloadDataLength = (int)obj.DirectoryEntry.EntryLength;
            }
            else if (obj.DirectoryEntry.PreloadBytes > 0)
            {
                preloadDataPointer = data.Position;
                preloadDataLength = obj.DirectoryEntry.PreloadBytes;
            }

            // If we had a valid preload data pointer
            byte[]? preloadData = null;
            if (preloadDataPointer >= initialOffset
                && preloadDataLength > 0
                && data.Position + preloadDataLength <= data.Length)
            {
                // Cache the current offset
                long currentOffset = data.Position;

                // Seek to the preload data offset
                data.Seek(preloadDataPointer, SeekOrigin.Begin);

                // Read the preload data
                preloadData = data.ReadBytes(preloadDataLength);

                // Seek back to the original offset
                data.Seek(currentOffset, SeekOrigin.Begin);
            }

            // Set the preload data
            obj.PreloadData = preloadData;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Valve Package directory item tree
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset to use in address comparisons</param>
        /// <returns>Filled Valve Package directory item tree on success, null on error</returns>
        public static DirectoryItem[] ParseDirectoryItemTree(Stream data, long initialOffset)
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
                        var directoryItem = ParseDirectoryItem(data, initialOffset, extensionString!, pathString!, nameString!);

                        // Add the directory item
                        directoryItems.Add(directoryItem);
                    }
                }
            }

            return [.. directoryItems];
        }

        /// <summary>
        /// Parse a Stream into a ExtendedHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExtendedHeader on success, null on error</returns>
        public static ExtendedHeader ParseExtendedHeader(Stream data)
        {
            var obj = new ExtendedHeader();

            obj.FileDataSectionSize = data.ReadUInt32LittleEndian();
            obj.ArchiveMD5SectionSize = data.ReadUInt32LittleEndian();
            obj.OtherMD5SectionSize = data.ReadUInt32LittleEndian();
            obj.SignatureSectionSize = data.ReadUInt32LittleEndian();

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

            obj.Signature = data.ReadUInt32LittleEndian();
            obj.Version = data.ReadUInt32LittleEndian();
            obj.TreeSize = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
