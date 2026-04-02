using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.ZArchive;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class ZArchive : BaseBinaryReader<Archive>
    {
        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if (data.Length - data.Position < Constants.FooterSize)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var archive = new Archive();

                // Parse the footer first
                data.SeekIfPossible(-Constants.FooterSize, SeekOrigin.End);
                var footer = ParseFooter(data, initialOffset);
                if (footer is null)
                    return null;

                archive.Footer = footer;

                // Check offset records offset validity
                long offsetRecordsOffset = initialOffset + (long)archive.Footer.SectionOffsetRecords.Offset;
                if (offsetRecordsOffset < 0 || offsetRecordsOffset + (long)archive.Footer.SectionOffsetRecords.Size >= data.Length)
                    return null;

                // Seek to and then read the compression offset records
                data.SeekIfPossible(offsetRecordsOffset, SeekOrigin.Begin);
                var offsetRecords = ParseOffsetRecords(data, archive.Footer.SectionOffsetRecords.Size);
                if (offsetRecords is null)
                    return null;

                archive.OffsetRecords = offsetRecords;

                // Check name table section validity
                long nameTableOffset = initialOffset + (long)archive.Footer.SectionNameTable.Offset;
                if (nameTableOffset < 0 || nameTableOffset + (long)archive.Footer.SectionNameTable.Size >= data.Length)
                    return null;

                // Seek to and then read the name table entries
                data.SeekIfPossible(nameTableOffset, SeekOrigin.Begin);
                var nameTable = ParseNameTable(data, archive.Footer.SectionNameTable.Size);
                if (nameTable is null)
                    return null;

                archive.NameTable = nameTable;

                // Check name table section validity
                long fileTreeOffset = initialOffset + (long)archive.Footer.SectionFileTree.Offset;
                if (fileTreeOffset < 0 || fileTreeOffset + (long)archive.Footer.SectionFileTree.Size >= data.Length)
                    return null;

                // Seek to and then read the file tree entries
                data.SeekIfPossible(fileTreeOffset, SeekOrigin.Begin);
                var fileTree = ParseFileTree(data, archive.Footer.SectionFileTree.Size, archive.Footer.SectionNameTable.Size);
                if (fileTree is null)
                    return null;

                archive.FileTree = fileTree;

                // Do not attempt to read compressed data into memory

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an ZArchive footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZArchive footer on success, null on error</returns>
        public static Footer? ParseFooter(Stream data, long initialOffset)
        {
            var obj = new Footer();

            // Read and validate compressed data section offset and size values
            obj.SectionCompressedData.Offset = data.ReadUInt64BigEndian();
            obj.SectionCompressedData.Size = data.ReadUInt64BigEndian();
            if (obj.SectionCompressedData.Offset + obj.SectionCompressedData.Size > (ulong)data.Length)
                return null;

            // Read and validate offset records section offset and size values
            obj.SectionOffsetRecords.Offset = data.ReadUInt64BigEndian();
            obj.SectionOffsetRecords.Size = data.ReadUInt64BigEndian();
            if (obj.SectionOffsetRecords.Offset + obj.SectionOffsetRecords.Size > (ulong)data.Length)
                return null;
            if (obj.SectionOffsetRecords.Size > Constants.MaxOffsetRecordsSize)
                return null;
            if (obj.SectionOffsetRecords.Size % Constants.OffsetRecordSize != 0)
                return null;

            // Read and validate name table section offset and size values
            obj.SectionNameTable.Offset = data.ReadUInt64BigEndian();
            obj.SectionNameTable.Size = data.ReadUInt64BigEndian();
            if (obj.SectionNameTable.Offset + obj.SectionNameTable.Size > (ulong)data.Length)
                return null;
            if (obj.SectionNameTable.Size > Constants.MaxNameTableSize)
                return null;

            // Read and validate file tree section offset and size values
            obj.SectionFileTree.Offset = data.ReadUInt64BigEndian();
            obj.SectionFileTree.Size = data.ReadUInt64BigEndian();
            if (obj.SectionFileTree.Offset + obj.SectionFileTree.Size > (ulong)data.Length)
                return null;
            if (obj.SectionFileTree.Size > Constants.MaxFileTreeSize)
                return null;
            if (obj.SectionFileTree.Size % Constants.FileDirectoryEntrySize != 0)
                return null;

            // Read and validate metadirectory section offset and size values
            obj.SectionMetaDirectory.Offset = data.ReadUInt64BigEndian();
            obj.SectionMetaDirectory.Size = data.ReadUInt64BigEndian();
            if (obj.SectionMetaDirectory.Offset + obj.SectionMetaDirectory.Size > (ulong)data.Length)
                return null;

            // Read and validate metadata section offset and size values
            obj.SectionMetaData.Offset = data.ReadUInt64BigEndian();
            obj.SectionMetaData.Size = data.ReadUInt64BigEndian();
            if (obj.SectionMetaData.Offset + obj.SectionMetaData.Size > (ulong)data.Length)
                return null;

            // Read and validate archive integrity hash
            obj.IntegrityHash = data.ReadBytes(32);
            // data.SeekIfPossible(initialOffset, SeekOrigin.Begin);
            // TODO: Read all bytes and hash them with SHA256
            // TODO: Compare obj.Integrity with calculated hash

            // Read and validate archive size
            obj.Size = data.ReadUInt64BigEndian();
            if (obj.Size != (ulong)(data.Length - initialOffset))
                return null;

            // Read and validate version bytes, only Version 1 is supported
            obj.Version = data.ReadBytes(4);
            if (!obj.Version.EqualsExactly(Constants.Version1Bytes))
                return null;

            // Read and validate magic bytes
            obj.Magic = data.ReadBytes(4);
            if (!obj.Magic.EqualsExactly(Constants.MagicBytes))
                return null;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ZArchive OffsetRecords section
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="size">Size of OffsetRecords section</param>
        /// <returns>Filled ZArchive OffsetRecords section on success, null on error</returns>
        public static OffsetRecord[]? ParseOffsetRecords(Stream data, ulong size)
        {
            int entries = (int)(size / Constants.OffsetRecordSize);

            var obj = new OffsetRecord[entries];

            for (int i = 0; i < entries; i++)
            {
                var offset = data.ReadUInt64BigEndian();
                obj[i] = new OffsetRecord();
                obj[i].Offset = offset;
                for (int block = 0; block < Constants.BlocksPerOffsetRecord; block++)
                {
                    obj[i].Size[block] = data.ReadUInt16BigEndian();
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ZArchive NameTable section
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="size">Size of NameTable section</param>
        /// <returns>Filled ZArchive NameTable section on success, null on error</returns>
        public static NameTable? ParseNameTable(Stream data, ulong size)
        {
            var obj = new NameTable();
            var nameEntries = new List<NameEntry>();
            var nameOffsets = new List<uint>();

            uint bytesRead = 0;

            while (bytesRead < (uint)size)
            {
                var nameEntry = new NameEntry();

                // Cache the offset into the NameEntry table
                nameOffsets.Add(bytesRead);

                // Read length of name
                uint nameLength = data.ReadByteValue();
                bytesRead += 1;
                if ((nameLength & 0x80) == 0x80)
                {
                    nameLength += (uint)data.ReadByteValue() << 7;
                    bytesRead += 1;
                    nameEntry.NodeLengthLong = (ushort)nameLength;
                }
                else
                {
                    nameEntry.NodeLengthShort = (byte)nameLength;
                }

                // Validate name length
                if (bytesRead + nameLength > (uint)size)
                    return null;

                // Add valid name entry to the table
                nameEntry.NodeName = data.ReadBytes((int)nameLength);
                bytesRead += nameLength;
                nameEntries.Add(nameEntry);
            }

            obj.NameEntries = [.. nameEntries];
            obj.NameTableOffsets = [.. nameOffsets];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ZArchive FileTree section
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="size">Size of FileTree section</param>
        /// <returns>Filled ZArchive FileTree section on success, null on error</returns>
        public static FileDirectoryEntry[]? ParseFileTree(Stream data, ulong size, ulong nameTableSize)
        {
            int entries = (int)(size / Constants.FileDirectoryEntrySize);

            var obj = new FileDirectoryEntry[entries];

            for (int i = 0; i < entries; i++)
            {
                var nameOffsetAndFlag = data.ReadUInt32BigEndian();

                // Validate name table offset value
                if ((nameOffsetAndFlag & Constants.RootNode) > nameTableSize && nameOffsetAndFlag != Constants.RootNode)
                    return null;

                // Check if node is file or directory
                if ((nameOffsetAndFlag & Constants.FileFlag) == Constants.FileFlag)
                {
                    var fileEntry = new FileEntry();
                    fileEntry.NameOffsetAndTypeFlag = nameOffsetAndFlag;
                    fileEntry.FileOffsetLow = data.ReadUInt32BigEndian();
                    fileEntry.FileSizeLow = data.ReadUInt32BigEndian();
                    fileEntry.FileSizeHigh = data.ReadUInt16BigEndian();
                    fileEntry.FileOffsetHigh = data.ReadUInt16BigEndian();
                    obj[i] = fileEntry;
                }
                else
                {
                    var directoryEntry = new DirectoryEntry();
                    directoryEntry.NameOffsetAndTypeFlag = nameOffsetAndFlag;
                    directoryEntry.NodeStartIndex = data.ReadUInt32BigEndian();
                    directoryEntry.Count = data.ReadUInt32BigEndian();
                    directoryEntry.Reserved = data.ReadUInt32BigEndian();
                    obj[i] = directoryEntry;
                }
            }

            // First entry of file tree must be root directory
            if ((obj[0].NameOffsetAndTypeFlag & Constants.RootNode) != Constants.RootNode)
                return null;

            return obj;
        }
    }
}
