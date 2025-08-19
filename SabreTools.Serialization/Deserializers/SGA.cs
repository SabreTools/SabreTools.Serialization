using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.SGA;
using static SabreTools.Models.SGA.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class SGA : BaseBinaryDeserializer<Archive>
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

                // Create a new SGA to fill
                var archive = new Archive();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header == null)
                    return null;

                // Set the SGA header
                archive.Header = header;

                #endregion

                #region Directory

                // Try to parse the directory
                var directory = ParseDirectory(data, header.MajorVersion);
                if (directory == null)
                    return null;

                // Set the SGA directory
                archive.Directory = directory;

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
        /// Parse a Stream into an SGA header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA header on success, null on error</returns>
        private static Header? ParseHeader(Stream data)
        {
            byte[] signatureBytes = data.ReadBytes(8);
            string signature = Encoding.ASCII.GetString(signatureBytes);
            if (signature != SignatureString)
                return null;

            ushort majorVersion = data.ReadUInt16LittleEndian();
            ushort minorVersion = data.ReadUInt16LittleEndian();
            if (minorVersion != 0)
                return null;

            switch (majorVersion)
            {
                // Versions 4 and 5 share the same header
                case 4:
                case 5:
                    var header4 = new Header4();

                    header4.Signature = signature;
                    header4.MajorVersion = majorVersion;
                    header4.MinorVersion = minorVersion;
                    header4.FileMD5 = data.ReadBytes(0x10);
                    byte[] header4Name = data.ReadBytes(count: 128);
                    header4.Name = Encoding.Unicode.GetString(header4Name).TrimEnd('\0');
                    header4.HeaderMD5 = data.ReadBytes(0x10);
                    header4.HeaderLength = data.ReadUInt32LittleEndian();
                    header4.FileDataOffset = data.ReadUInt32LittleEndian();
                    header4.Dummy0 = data.ReadUInt32LittleEndian();

                    return header4;

                // Versions 6 and 7 share the same header
                case 6:
                case 7:
                    var header6 = new Header6();

                    header6.Signature = signature;
                    header6.MajorVersion = majorVersion;
                    header6.MinorVersion = minorVersion;
                    byte[] header6Name = data.ReadBytes(count: 128);
                    header6.Name = Encoding.Unicode.GetString(header6Name).TrimEnd('\0');
                    header6.HeaderLength = data.ReadUInt32LittleEndian();
                    header6.FileDataOffset = data.ReadUInt32LittleEndian();
                    header6.Dummy0 = data.ReadUInt32LittleEndian();

                    return header6;

                // No other major versions are recognized
                default:
                    return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an SGA directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA directory on success, null on error</returns>
        private static Models.SGA.Directory? ParseDirectory(Stream data, ushort majorVersion)
        {
            return majorVersion switch
            {
                4 => ParseDirectory4(data),
                5 => ParseDirectory5(data),
                6 => ParseDirectory6(data),
                7 => ParseDirectory7(data),
                _ => null,
            };
        }

        /// <summary>
        /// Parse a Stream into an SGA directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory on success, null on error</returns>
        private static Directory4? ParseDirectory4(Stream data)
        {
            var directory = new Directory4();

            // Cache the current offset
            long currentOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectory4Header(data);
            if (directoryHeader == null)
                return null;

            // Set the directory header
            directory.DirectoryHeader = directoryHeader;

            #endregion

            #region Sections

            // Get and adjust the sections offset
            long sectionOffset = currentOffset + directoryHeader.SectionOffset;

            // Validate the offset
            if (sectionOffset < currentOffset || sectionOffset >= data.Length)
                return null;

            // Seek to the sections
            data.Seek(sectionOffset, SeekOrigin.Begin);

            // Create the sections array
            directory.Sections = new Section4[directoryHeader.SectionCount];

            // Try to parse the sections
            for (int i = 0; i < directory.Sections.Length; i++)
            {
                directory.Sections[i] = ParseSection4(data);
            }

            #endregion

            #region Folders

            // Get and adjust the folders offset
            long folderOffset = currentOffset + directoryHeader.FolderOffset;

            // Validate the offset
            if (folderOffset < currentOffset || folderOffset >= data.Length)
                return null;

            // Seek to the folders
            data.Seek(folderOffset, SeekOrigin.Begin);

            // Create the folders array
            directory.Folders = new Folder4[directoryHeader.FolderCount];

            // Try to parse the folders
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                directory.Folders[i] = ParseFolder4(data);
            }

            #endregion

            #region Files

            // Get and adjust the files offset
            long fileOffset = currentOffset + directoryHeader.FileOffset;

            // Validate the offset
            if (fileOffset < currentOffset || fileOffset >= data.Length)
                return null;

            // Seek to the files
            data.Seek(fileOffset, SeekOrigin.Begin);

            // Get the file count
            uint fileCount = directoryHeader.FileCount;

            // Create the files array
            directory.Files = new File4[fileCount];

            // Try to parse the files
            for (int i = 0; i < directory.Files.Length; i++)
            {
                directory.Files[i] = ParseFile4(data);
            }

            #endregion

            #region String Table

            // Get and adjust the string table offset
            long stringTableOffset = currentOffset + directoryHeader.StringTableOffset;

            // Validate the offset
            if (stringTableOffset < currentOffset || stringTableOffset >= data.Length)
                return null;

            // Seek to the string table
            data.Seek(stringTableOffset, SeekOrigin.Begin);

            // TODO: Are these strings actually indexed by number and not position?
            // TODO: If indexed by position, I think it needs to be adjusted by start of table

            // Create the strings dictionary
            directory.StringTable = new Dictionary<long, string?>((int)directoryHeader.StringTableCount);

            // Get the current position to adjust the offsets
            long stringTableStart = data.Position;

            // Try to parse the strings
            for (int i = 0; i < directoryHeader.StringTableCount; i++)
            {
                long currentPosition = data.Position - stringTableStart;
                directory.StringTable[currentPosition] = data.ReadNullTerminatedAnsiString();
            }

            // Loop through all folders to assign names
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                var folder = directory.Folders[i];
                if (folder == null)
                    continue;

                folder.Name = directory.StringTable[folder.NameOffset];
            }

            // Loop through all files to assign names
            for (int i = 0; i < directory.Files.Length; i++)
            {
                var file = directory.Files[i];
                if (file == null)
                    continue;

                file.Name = directory.StringTable[file.NameOffset];
            }

            #endregion

            return directory;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory on success, null on error</returns>
        private static Directory5? ParseDirectory5(Stream data)
        {
            var directory = new Directory5();

            // Cache the current offset
            long currentOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectory5Header(data);
            if (directoryHeader == null)
                return null;

            // Set the directory header
            directory.DirectoryHeader = directoryHeader;

            #endregion

            #region Sections

            // Get and adjust the sections offset
            long sectionOffset = currentOffset + directoryHeader.SectionOffset;

            // Validate the offset
            if (sectionOffset < currentOffset || sectionOffset >= data.Length)
                return null;

            // Seek to the sections
            data.Seek(sectionOffset, SeekOrigin.Begin);

            // Create the sections array
            directory.Sections = new Section5[directoryHeader.SectionCount];

            // Try to parse the sections
            for (int i = 0; i < directory.Sections.Length; i++)
            {
                directory.Sections[i] = ParseSection5(data);
            }

            #endregion

            #region Folders

            // Get and adjust the folders offset
            long folderOffset = currentOffset + directoryHeader.FolderOffset;

            // Validate the offset
            if (folderOffset < currentOffset || folderOffset >= data.Length)
                return null;

            // Seek to the folders
            data.Seek(folderOffset, SeekOrigin.Begin);

            // Create the folders array
            directory.Folders = new Folder5[directoryHeader.FolderCount];

            // Try to parse the folders
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                directory.Folders[i] = ParseFolder5(data);
            }

            #endregion

            #region Files

            // Get and adjust the files offset
            long fileOffset = currentOffset + directoryHeader.FileOffset;

            // Validate the offset
            if (fileOffset < currentOffset || fileOffset >= data.Length)
                return null;

            // Seek to the files
            data.Seek(fileOffset, SeekOrigin.Begin);

            // Create the files array
            directory.Files = new File4[directoryHeader.FileCount];

            // Try to parse the files
            for (int i = 0; i < directory.Files.Length; i++)
            {
                directory.Files[i] = ParseFile4(data);
            }

            #endregion

            #region String Table

            // Get and adjust the string table offset
            long stringTableOffset = currentOffset + directoryHeader.StringTableOffset;

            // Validate the offset
            if (stringTableOffset < currentOffset || stringTableOffset >= data.Length)
                return null;

            // Seek to the string table
            data.Seek(stringTableOffset, SeekOrigin.Begin);

            // TODO: Are these strings actually indexed by number and not position?
            // TODO: If indexed by position, I think it needs to be adjusted by start of table

            // Create the strings dictionary
            directory.StringTable = new Dictionary<long, string?>((int)directoryHeader.StringTableCount);

            // Get the current position to adjust the offsets
            long stringTableStart = data.Position;

            // Try to parse the strings
            for (int i = 0; i < directoryHeader.StringTableCount; i++)
            {
                long currentPosition = data.Position - stringTableStart;
                directory.StringTable[currentPosition] = data.ReadNullTerminatedAnsiString();
            }

            // Loop through all folders to assign names
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                var folder = directory.Folders[i];
                if (folder == null)
                    continue;

                folder.Name = directory.StringTable[folder.NameOffset];
            }

            // Loop through all files to assign names
            for (int i = 0; i < directory.Files.Length; i++)
            {
                var file = directory.Files[i];
                if (file == null)
                    continue;

                file.Name = directory.StringTable[file.NameOffset];
            }

            #endregion

            return directory;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory on success, null on error</returns>
        private static Directory6? ParseDirectory6(Stream data)
        {
            var directory = new Directory6();

            // Cache the current offset
            long currentOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectory5Header(data);
            if (directoryHeader == null)
                return null;

            // Set the directory header
            directory.DirectoryHeader = directoryHeader;

            #endregion

            #region Sections

            // Get and adjust the sections offset
            long sectionOffset = currentOffset + directoryHeader.SectionOffset;

            // Validate the offset
            if (sectionOffset < currentOffset || sectionOffset >= data.Length)
                return null;

            // Seek to the sections
            data.Seek(sectionOffset, SeekOrigin.Begin);

            // Create the sections array
            directory.Sections = new Section5[directoryHeader.SectionCount];

            // Try to parse the sections
            for (int i = 0; i < directory.Sections.Length; i++)
            {
                directory.Sections[i] = ParseSection5(data);
            }

            #endregion

            #region Folders

            // Get and adjust the folders offset
            long folderOffset = currentOffset + directoryHeader.FolderOffset;

            // Validate the offset
            if (folderOffset < currentOffset || folderOffset >= data.Length)
                return null;

            // Seek to the folders
            data.Seek(folderOffset, SeekOrigin.Begin);

            // Create the folders array
            directory.Folders = new Folder5[directoryHeader.FolderCount];

            // Try to parse the folders
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                directory.Folders[i] = ParseFolder5(data);
            }

            #endregion

            #region Files

            // Get and adjust the files offset
            long fileOffset = currentOffset + directoryHeader.FileOffset;

            // Validate the offset
            if (fileOffset < currentOffset || fileOffset >= data.Length)
                return null;

            // Seek to the files
            data.Seek(fileOffset, SeekOrigin.Begin);

            // Create the files array
            directory.Files = new File6[directoryHeader.FileCount];

            // Try to parse the files
            for (int i = 0; i < directory.Files.Length; i++)
            {
                directory.Files[i] = ParseFile6(data);
            }

            #endregion

            #region String Table

            // Get and adjust the string table offset
            long stringTableOffset = currentOffset + directoryHeader.StringTableOffset;

            // Validate the offset
            if (stringTableOffset < currentOffset || stringTableOffset >= data.Length)
                return null;

            // Seek to the string table
            data.Seek(stringTableOffset, SeekOrigin.Begin);

            // TODO: Are these strings actually indexed by number and not position?
            // TODO: If indexed by position, I think it needs to be adjusted by start of table

            // Create the strings dictionary
            directory.StringTable = new Dictionary<long, string?>((int)directoryHeader.StringTableCount);

            // Get the current position to adjust the offsets
            long stringTableStart = data.Position;

            // Try to parse the strings
            for (int i = 0; i < directoryHeader.StringTableCount; i++)
            {
                long currentPosition = data.Position - stringTableStart;
                directory.StringTable[currentPosition] = data.ReadNullTerminatedAnsiString();
            }

            // Loop through all folders to assign names
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                var folder = directory.Folders[i];
                if (folder == null)
                    continue;

                folder.Name = directory.StringTable[folder.NameOffset];
            }

            // Loop through all files to assign names
            for (int i = 0; i < directory.Files.Length; i++)
            {
                var file = directory.Files[i];
                if (file == null)
                    continue;

                file.Name = directory.StringTable[file.NameOffset];
            }

            #endregion

            return directory;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory on success, null on error</returns>
        private static Directory7? ParseDirectory7(Stream data)
        {
            var directory = new Directory7();

            // Cache the current offset
            long currentOffset = data.Position;

            #region Directory Header

            // Try to parse the directory header
            var directoryHeader = ParseDirectory7Header(data);
            if (directoryHeader == null)
                return null;

            // Set the directory header
            directory.DirectoryHeader = directoryHeader;

            #endregion

            #region Sections

            // Get and adjust the sections offset
            long sectionOffset = currentOffset + directoryHeader.SectionOffset;

            // Validate the offset
            if (sectionOffset < currentOffset || sectionOffset >= data.Length)
                return null;

            // Seek to the sections
            data.Seek(sectionOffset, SeekOrigin.Begin);

            // Create the sections array
            directory.Sections = new Section5[directoryHeader.SectionCount];

            // Try to parse the sections
            for (int i = 0; i < directory.Sections.Length; i++)
            {
                directory.Sections[i] = ParseSection5(data);
            }

            #endregion

            #region Folders

            // Get and adjust the folders offset
            long folderOffset = currentOffset + directoryHeader.FolderOffset;

            // Validate the offset
            if (folderOffset < currentOffset || folderOffset >= data.Length)
                return null;

            // Seek to the folders
            data.Seek(folderOffset, SeekOrigin.Begin);

            // Create the folders array
            directory.Folders = new Folder5[directoryHeader.FolderCount];

            // Try to parse the folders
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                directory.Folders[i] = ParseFolder5(data);
            }

            #endregion

            #region Files

            // Get and adjust the files offset
            long fileOffset = currentOffset + directoryHeader.FileOffset;

            // Validate the offset
            if (fileOffset < currentOffset || fileOffset >= data.Length)
                return null;

            // Seek to the files
            data.Seek(fileOffset, SeekOrigin.Begin);

            // Create the files array
            directory.Files = new File7[directoryHeader.FileCount];

            // Try to parse the files
            for (int i = 0; i < directory.Files.Length; i++)
            {
                directory.Files[i] = ParseFile7(data);
            }

            #endregion

            #region String Table

            // Get and adjust the string table offset
            long stringTableOffset = currentOffset + directoryHeader.StringTableOffset;

            // Validate the offset
            if (stringTableOffset < currentOffset || stringTableOffset >= data.Length)
                return null;

            // Seek to the string table
            data.Seek(stringTableOffset, SeekOrigin.Begin);

            // TODO: Are these strings actually indexed by number and not position?
            // TODO: If indexed by position, I think it needs to be adjusted by start of table

            // Create the strings dictionary
            directory.StringTable = new Dictionary<long, string?>((int)directoryHeader.StringTableCount);

            // Get the current position to adjust the offsets
            long stringTableStart = data.Position;

            // Try to parse the strings
            for (int i = 0; i < directoryHeader.StringTableCount; i++)
            {
                long currentPosition = data.Position - stringTableStart;
                directory.StringTable[currentPosition] = data.ReadNullTerminatedAnsiString();
            }

            // Loop through all folders to assign names
            for (int i = 0; i < directory.Folders.Length; i++)
            {
                var folder = directory.Folders[i];
                if (folder == null)
                    continue;

                folder.Name = directory.StringTable[folder.NameOffset];
            }

            // Loop through all files to assign names
            for (int i = 0; i < directory.Files.Length; i++)
            {
                var file = directory.Files[i];
                if (file == null)
                    continue;

                file.Name = directory.StringTable[file.NameOffset];
            }

            #endregion

            return directory;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory header version 4 on success, null on error</returns>
        private static DirectoryHeader4 ParseDirectory4Header(Stream data)
        {
            var directoryHeader4 = new DirectoryHeader4();

            directoryHeader4.SectionOffset = data.ReadUInt32LittleEndian();
            directoryHeader4.SectionCount = data.ReadUInt16LittleEndian();
            directoryHeader4.FolderOffset = data.ReadUInt32LittleEndian();
            directoryHeader4.FolderCount = data.ReadUInt16LittleEndian();
            directoryHeader4.FileOffset = data.ReadUInt32LittleEndian();
            directoryHeader4.FileCount = data.ReadUInt16LittleEndian();
            directoryHeader4.StringTableOffset = data.ReadUInt32LittleEndian();
            directoryHeader4.StringTableCount = data.ReadUInt16LittleEndian();

            return directoryHeader4;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header version 5
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory header version 5 on success, null on error</returns>
        private static DirectoryHeader5 ParseDirectory5Header(Stream data)
        {
            var directoryHeader5 = new DirectoryHeader5();

            directoryHeader5.SectionOffset = data.ReadUInt32LittleEndian();
            directoryHeader5.SectionCount = data.ReadUInt32LittleEndian();
            directoryHeader5.FolderOffset = data.ReadUInt32LittleEndian();
            directoryHeader5.FolderCount = data.ReadUInt32LittleEndian();
            directoryHeader5.FileOffset = data.ReadUInt32LittleEndian();
            directoryHeader5.FileCount = data.ReadUInt32LittleEndian();
            directoryHeader5.StringTableOffset = data.ReadUInt32LittleEndian();
            directoryHeader5.StringTableCount = data.ReadUInt32LittleEndian();

            return directoryHeader5;
        }

        /// <summary>
        /// Parse a Stream into an SGA directory header version 7
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SGA directory header version 7 on success, null on error</returns>
        private static DirectoryHeader7 ParseDirectory7Header(Stream data)
        {
            var directoryHeader7 = new DirectoryHeader7();

            directoryHeader7.SectionOffset = data.ReadUInt32LittleEndian();
            directoryHeader7.SectionCount = data.ReadUInt32LittleEndian();
            directoryHeader7.FolderOffset = data.ReadUInt32LittleEndian();
            directoryHeader7.FolderCount = data.ReadUInt32LittleEndian();
            directoryHeader7.FileOffset = data.ReadUInt32LittleEndian();
            directoryHeader7.FileCount = data.ReadUInt32LittleEndian();
            directoryHeader7.StringTableOffset = data.ReadUInt32LittleEndian();
            directoryHeader7.StringTableCount = data.ReadUInt32LittleEndian();
            directoryHeader7.HashTableOffset = data.ReadUInt32LittleEndian();
            directoryHeader7.BlockSize = data.ReadUInt32LittleEndian();

            return directoryHeader7;
        }

        /// <summary>
        /// Parse a Stream into an SGA section version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA section version 4 on success, null on error</returns>
        private static Section4 ParseSection4(Stream data)
        {
            var section4 = new Section4();

            byte[] section4Alias = data.ReadBytes(64);
            section4.Alias = Encoding.ASCII.GetString(section4Alias).TrimEnd('\0');
            byte[] section4Name = data.ReadBytes(64);
            section4.Name = Encoding.ASCII.GetString(section4Name).TrimEnd('\0');
            section4.FolderStartIndex = data.ReadUInt16LittleEndian();
            section4.FolderEndIndex = data.ReadUInt16LittleEndian();
            section4.FileStartIndex = data.ReadUInt16LittleEndian();
            section4.FileEndIndex = data.ReadUInt16LittleEndian();
            section4.FolderRootIndex = data.ReadUInt16LittleEndian();

            return section4;
        }

        /// <summary>
        /// Parse a Stream into an SGA section version 5
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA section version 5 on success, null on error</returns>
        private static Section5 ParseSection5(Stream data)
        {
            var section5 = new Section5();

            byte[] section5Alias = data.ReadBytes(64);
            section5.Alias = Encoding.ASCII.GetString(section5Alias).TrimEnd('\0');
            byte[] section5Name = data.ReadBytes(64);
            section5.Name = Encoding.ASCII.GetString(section5Name).TrimEnd('\0');
            section5.FolderStartIndex = data.ReadUInt32LittleEndian();
            section5.FolderEndIndex = data.ReadUInt32LittleEndian();
            section5.FileStartIndex = data.ReadUInt32LittleEndian();
            section5.FileEndIndex = data.ReadUInt32LittleEndian();
            section5.FolderRootIndex = data.ReadUInt32LittleEndian();

            return section5;
        }

        /// <summary>
        /// Parse a Stream into an SGA folder version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA folder version 4 on success, null on error</returns>
        private static Folder4 ParseFolder4(Stream data)
        {
            var folder4 = new Folder4();

            folder4.NameOffset = data.ReadUInt32LittleEndian();
            folder4.Name = null; // Read from string table
            folder4.FolderStartIndex = data.ReadUInt16LittleEndian();
            folder4.FolderEndIndex = data.ReadUInt16LittleEndian();
            folder4.FileStartIndex = data.ReadUInt16LittleEndian();
            folder4.FileEndIndex = data.ReadUInt16LittleEndian();

            return folder4;
        }

        /// <summary>
        /// Parse a Stream into an SGA folder version 5
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA folder version 5 on success, null on error</returns>
        private static Folder5 ParseFolder5(Stream data)
        {
            var folder5 = new Folder5();

            folder5.NameOffset = data.ReadUInt32LittleEndian();
            folder5.Name = null; // Read from string table
            folder5.FolderStartIndex = data.ReadUInt32LittleEndian();
            folder5.FolderEndIndex = data.ReadUInt32LittleEndian();
            folder5.FileStartIndex = data.ReadUInt32LittleEndian();
            folder5.FileEndIndex = data.ReadUInt32LittleEndian();

            return folder5;
        }

        /// <summary>
        /// Parse a Stream into an SGA file version 4
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA file version 4 on success, null on error</returns>
        private static File4 ParseFile4(Stream data)
        {
            var file4 = new File4();

            file4.NameOffset = data.ReadUInt32LittleEndian();
            file4.Name = null; // Read from string table
            file4.Offset = data.ReadUInt32LittleEndian();
            file4.SizeOnDisk = data.ReadUInt32LittleEndian();
            file4.Size = data.ReadUInt32LittleEndian();
            file4.TimeModified = data.ReadUInt32LittleEndian();
            file4.Dummy0 = data.ReadByteValue();
            file4.Type = data.ReadByteValue();

            return file4;
        }

        /// <summary>
        /// Parse a Stream into an SGA file version 6
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA file version 6 on success, null on error</returns>
        private static File6 ParseFile6(Stream data)
        {
            var file6 = new File6();

            file6.NameOffset = data.ReadUInt32LittleEndian();
            file6.Name = null; // Read from string table
            file6.Offset = data.ReadUInt32LittleEndian();
            file6.SizeOnDisk = data.ReadUInt32LittleEndian();
            file6.Size = data.ReadUInt32LittleEndian();
            file6.TimeModified = data.ReadUInt32LittleEndian();
            file6.Dummy0 = data.ReadByteValue();
            file6.Type = data.ReadByteValue();
            file6.CRC32 = data.ReadUInt32LittleEndian();

            return file6;
        }

        /// <summary>
        /// Parse a Stream into an SGA file version 7
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">SGA major version</param>
        /// <returns>Filled SGA file version 7 on success, null on error</returns>
        private static File7 ParseFile7(Stream data)
        {
            var file7 = new File7();

            file7.NameOffset = data.ReadUInt32LittleEndian();
            file7.Name = null; // Read from string table
            file7.Offset = data.ReadUInt32LittleEndian();
            file7.SizeOnDisk = data.ReadUInt32LittleEndian();
            file7.Size = data.ReadUInt32LittleEndian();
            file7.TimeModified = data.ReadUInt32LittleEndian();
            file7.Dummy0 = data.ReadByteValue();
            file7.Type = data.ReadByteValue();
            file7.CRC32 = data.ReadUInt32LittleEndian();
            file7.HashOffset = data.ReadUInt32LittleEndian();

            return file7;
        }
    }
}
