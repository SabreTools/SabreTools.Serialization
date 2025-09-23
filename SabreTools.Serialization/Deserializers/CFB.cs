using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CFB;
using SabreTools.Serialization.Extensions;
using static SabreTools.Models.CFB.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class CFB : BaseBinaryDeserializer<Binary>
    {
        /// <inheritdoc/>
        public override Binary? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new binary to fill
                var binary = new Binary();

                #region Header

                // Try to parse the file header
                var fileHeader = ParseFileHeader(data);
                if (fileHeader?.Signature != SignatureUInt64)
                    return null;
                if (fileHeader.ByteOrder != 0xFFFE)
                    return null;
                if (fileHeader.MajorVersion == 3 && fileHeader.SectorShift != 0x0009)
                    return null;
                else if (fileHeader.MajorVersion == 4 && fileHeader.SectorShift != 0x000C)
                    return null;
                if (fileHeader.MajorVersion == 3 && fileHeader.NumberOfDirectorySectors != 0)
                    return null;
                if (fileHeader.MiniStreamCutoffSize != 0x00001000)
                    return null;

                // Set the file header
                binary.Header = fileHeader;

                #endregion

                #region DIFAT Sector Numbers

                // Create a DIFAT sector table
                var difatSectors = new List<SectorNumber>();

                // Add the sectors from the header
                if (fileHeader.DIFAT != null)
                    difatSectors.AddRange(fileHeader.DIFAT);

                // Loop through and add the DIFAT sectors
                var currentSector = (SectorNumber?)fileHeader.FirstDIFATSectorLocation;
                for (int i = 0; i < fileHeader.NumberOfDIFATSectors; i++)
                {
                    // If we have an unreadable sector
                    if (currentSector > SectorNumber.MAXREGSECT)
                        break;

                    // Get the new next sector information
                    long sectorOffset = initialOffset
                        + (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < initialOffset || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var sectorNumbers = ParseSectorNumbers(data, fileHeader.SectorShift);
                    if (sectorNumbers == null)
                        return null;

                    // Add all but the last sector number that was parsed
                    for (int j = 0; j < sectorNumbers.Length - 1; j++)
                    {
                        difatSectors.Add(sectorNumbers[j]);
                    }

                    // Get the next sector from the final sector number
                    currentSector = sectorNumbers[sectorNumbers.Length - 1];
                }

                // Assign the DIFAT sectors table
                binary.DIFATSectorNumbers = [.. difatSectors];

                #endregion

                #region FAT Sector Numbers

                // Create a FAT sector table
                var fatSectors = new List<SectorNumber>();

                // Loop through and add the FAT sectors
                for (int i = 0; i < fileHeader.NumberOfFATSectors; i++)
                {
                    // Get the next sector from the DIFAT
                    currentSector = binary.DIFATSectorNumbers[i];

                    // If we have an unreadable sector
                    if (currentSector > SectorNumber.MAXREGSECT)
                        break;

                    // Get the new next sector information
                    long sectorOffset = initialOffset
                        + (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < initialOffset || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var sectorNumbers = ParseSectorNumbers(data, fileHeader.SectorShift);
                    if (sectorNumbers == null)
                        return null;

                    // Add the sector shifts
                    fatSectors.AddRange(sectorNumbers);
                }

                // Assign the FAT sectors table
                binary.FATSectorNumbers = [.. fatSectors];

                #endregion

                #region Mini FAT Sector Numbers

                // Create a mini FAT sector table
                var miniFatSectors = new List<SectorNumber>();

                // Loop through and add the mini FAT sectors
                currentSector = (SectorNumber)fileHeader.FirstMiniFATSectorLocation;
                for (int i = 0; i < fileHeader.NumberOfMiniFATSectors; i++)
                {
                    // If we have an unreadable sector
                    if (currentSector > SectorNumber.MAXREGSECT)
                        break;

                    // Get the new next sector information
                    long sectorOffset = initialOffset
                        + (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < initialOffset || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var sectorNumbers = ParseSectorNumbers(data, fileHeader.SectorShift);
                    if (sectorNumbers == null)
                        return null;

                    // Add the sector shifts
                    miniFatSectors.AddRange(sectorNumbers);

                    // Get the next sector from the FAT
                    currentSector = binary.FATSectorNumbers[(int)currentSector];
                }

                // Assign the mini FAT sectors table
                binary.MiniFATSectorNumbers = [.. miniFatSectors];

                #endregion

                #region Directory Entries

                // Create a directory sector table
                var directorySectors = new List<DirectoryEntry>();

                // Get the number of directory sectors
                uint directorySectorCount = 0;
                switch (fileHeader.MajorVersion)
                {
                    case 3:
                        directorySectorCount = int.MaxValue;
                        break;
                    case 4:
                        directorySectorCount = fileHeader.NumberOfDirectorySectors;
                        break;
                }

                // Loop through and add the directory sectors
                currentSector = (SectorNumber)fileHeader.FirstDirectorySectorLocation;
                for (int i = 0; i < directorySectorCount; i++)
                {
                    // If we have an end of chain
                    if (currentSector == SectorNumber.ENDOFCHAIN)
                        break;

                    // If we have an unusable sector for a version 3 file
                    if (directorySectorCount == int.MaxValue && currentSector > SectorNumber.MAXREGSECT)
                        break;

                    // Get the new next sector information
                    long sectorOffset = initialOffset
                        + (long)((long)(currentSector + 1) * Math.Pow(2, fileHeader.SectorShift));
                    if (sectorOffset < initialOffset || sectorOffset >= data.Length)
                        return null;

                    // Seek to the next sector
                    data.Seek(sectorOffset, SeekOrigin.Begin);

                    // Try to parse the sectors
                    var directoryEntries = ParseDirectoryEntries(data, fileHeader.SectorShift, fileHeader.MajorVersion);
                    if (directoryEntries == null)
                        return null;

                    // Add the sector shifts
                    directorySectors.AddRange(directoryEntries);

                    // Get the next sector from the FAT
                    currentSector = binary.FATSectorNumbers[(int)currentSector];
                }

                // Assign the Directory sectors table
                binary.DirectoryEntries = [.. directorySectors];

                #endregion

                return binary;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a DirectoryEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryEntry on success, null on error</returns>
        public static DirectoryEntry ParseDirectoryEntry(Stream data)
        {
            var obj = new DirectoryEntry();

            byte[] name = data.ReadBytes(64);
            obj.Name = Encoding.Unicode.GetString(name).DecodeStreamName()?.TrimEnd('\0');
            obj.NameLength = data.ReadUInt16LittleEndian();
            obj.ObjectType = (ObjectType)data.ReadByteValue();
            obj.ColorFlag = (ColorFlag)data.ReadByteValue();
            obj.LeftSiblingID = (StreamID)data.ReadUInt32LittleEndian();
            obj.RightSiblingID = (StreamID)data.ReadUInt32LittleEndian();
            obj.ChildID = (StreamID)data.ReadUInt32LittleEndian();
            obj.CLSID = data.ReadGuid();
            obj.StateBits = data.ReadUInt32LittleEndian();
            obj.CreationTime = data.ReadUInt64LittleEndian();
            obj.ModifiedTime = data.ReadUInt64LittleEndian();
            obj.StartingSectorLocation = data.ReadUInt32LittleEndian();
            obj.StreamSize = data.ReadUInt64LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FileHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileHeader on success, null on error</returns>
        public static FileHeader ParseFileHeader(Stream data)
        {
            var obj = new FileHeader();

            obj.Signature = data.ReadUInt64LittleEndian();
            obj.CLSID = data.ReadGuid();
            obj.MinorVersion = data.ReadUInt16LittleEndian();
            obj.MajorVersion = data.ReadUInt16LittleEndian();
            obj.ByteOrder = data.ReadUInt16LittleEndian();
            obj.SectorShift = data.ReadUInt16LittleEndian();
            obj.MiniSectorShift = data.ReadUInt16LittleEndian();
            obj.Reserved = data.ReadBytes(6);
            obj.NumberOfDirectorySectors = data.ReadUInt32LittleEndian();
            obj.NumberOfFATSectors = data.ReadUInt32LittleEndian();
            obj.FirstDirectorySectorLocation = data.ReadUInt32LittleEndian();
            obj.TransactionSignatureNumber = data.ReadUInt32LittleEndian();
            obj.MiniStreamCutoffSize = data.ReadUInt32LittleEndian();
            obj.FirstMiniFATSectorLocation = data.ReadUInt32LittleEndian();
            obj.NumberOfMiniFATSectors = data.ReadUInt32LittleEndian();
            obj.FirstDIFATSectorLocation = data.ReadUInt32LittleEndian();
            obj.NumberOfDIFATSectors = data.ReadUInt32LittleEndian();
            obj.DIFAT = new SectorNumber[109];
            for (int i = 0; i < 109; i++)
            {
                obj.DIFAT[i] = (SectorNumber)data.ReadUInt32LittleEndian();
            }

            // Skip rest of sector for version 4
            if (obj.MajorVersion == 4)
                _ = data.ReadBytes(3584);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a sector full of sector numbers
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorShift">Sector shift from the header</param>
        /// <returns>Filled sector full of sector numbers on success, null on error</returns>
        private static SectorNumber[] ParseSectorNumbers(Stream data, ushort sectorShift)
        {
            int sectorCount = (int)(Math.Pow(2, sectorShift) / sizeof(uint));
            var sectorNumbers = new SectorNumber[sectorCount];

            for (int i = 0; i < sectorNumbers.Length; i++)
            {
                sectorNumbers[i] = (SectorNumber)data.ReadUInt32LittleEndian();
            }

            return sectorNumbers;
        }

        /// <summary>
        /// Parse a Stream into a sector full of directory entries
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorShift">Sector shift from the header</param>
        /// <param name="majorVersion">Major version from the header</param>
        /// <returns>Filled sector full of directory entries on success, null on error</returns>
        private static DirectoryEntry[]? ParseDirectoryEntries(Stream data, ushort sectorShift, ushort majorVersion)
        {
            // <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-CFB/%5bMS-CFB%5d.pdf"/>
            int directoryEntrySize = 128;

            int dirsPerSector = (int)(Math.Pow(2, sectorShift) / directoryEntrySize);
            var directoryEntries = new DirectoryEntry[dirsPerSector];

            for (int i = 0; i < directoryEntries.Length; i++)
            {
                var directoryEntry = ParseDirectoryEntry(data);

                // Handle version 3 entries
                if (majorVersion == 3)
                    directoryEntry.StreamSize &= 0x00000000FFFFFFFF;

                directoryEntries[i] = directoryEntry;
            }

            return directoryEntries;
        }
    }
}
