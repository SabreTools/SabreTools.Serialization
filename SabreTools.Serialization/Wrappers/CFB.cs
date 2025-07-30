using System;
using System.Collections.Generic;
using System.IO;

namespace SabreTools.Serialization.Wrappers
{
    public class CFB : WrapperBase<Models.CFB.Binary>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Compact File Binary";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Models.CFB.Binary.DirectoryEntries"/>
        public Models.CFB.DirectoryEntry[]? DirectoryEntries => Model.DirectoryEntries;

        /// <summary>
        /// Normal sector size in bytes
        /// </summary>
        public long SectorSize => (long)Math.Pow(2, Model.Header?.SectorShift ?? 0);

        /// <summary>
        /// Mini sector size in bytes
        /// </summary>
        public long MiniSectorSize => (long)Math.Pow(2, Model.Header?.MiniSectorShift ?? 0);

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public CFB(Models.CFB.Binary? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public CFB(Models.CFB.Binary? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a Compound File Binary from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Compound File Binary wrapper on success, null on failure</returns>
        public static CFB? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Compound File Binary from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Compound File Binary wrapper on success, null on failure</returns>
        public static CFB? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var binary = Deserializers.CFB.DeserializeStream(data);
                if (binary == null)
                    return null;

                return new CFB(binary, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Extraction

         /// <summary>
        /// Extract all files from the CFB to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all directory entries to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractEntry(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the CFB to an output directory by index
        /// </summary>
        /// <param name="index">Entry index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractEntry(int index, string outputDirectory)
        {
            // If we have no files
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= DirectoryEntries.Length)
                return false;

            // Get the file information
            var entry = DirectoryEntries[index];
            if (entry == null)
                return false;

            // TODO: Determine how to read the data for a single directory entry

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure directory separators are consistent
            string filename = entry.Name ?? $"file{index}";
            if (Path.DirectorySeparatorChar == '\\')
                filename = filename.Replace('/', '\\');
            else if (Path.DirectorySeparatorChar == '/')
                filename = filename.Replace('\\', '/');

            // Ensure the full output directory exists
            filename = Path.Combine(outputDirectory, filename);
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using FileStream fs = File.OpenWrite(filename);
                // TODO: Write out the data
                fs.Flush();
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region FAT Sector Data

        /// <summary>
        /// Get the ordered FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<Models.CFB.SectorNumber>? GetFATSectorChain(Models.CFB.SectorNumber? startingSector)
        {
            // If we have an invalid sector
            if (startingSector == null || startingSector < 0 || Model.FATSectorNumbers == null || (long)startingSector >= Model.FATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<Models.CFB.SectorNumber> { startingSector.Value };

            var lastSector = startingSector;
            while (true)
            {
                if (lastSector == null)
                    break;

                // Get the next sector from the lookup table
                var nextSector = Model.FATSectorNumbers[(uint)lastSector!.Value];

                // If we have an end of chain or free sector
                if (nextSector == Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == Models.CFB.SectorNumber.FREESECT)
                    break;

                // Add the next sector to the list and replace the last sector
                sectors.Add(nextSector);
                lastSector = nextSector;
            }

            return sectors;
        }

        /// <summary>
        /// Get the data for the FAT sector chain starting at a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public byte[]? GetFATSectorChainData(Models.CFB.SectorNumber startingSector)
        {
            // Get the sector chain first
            var sectorChain = GetFATSectorChain(startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the sector data offset
                int sectorDataOffset = (int)FATSectorToFileOffset(sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= GetEndOfFile())
                    return null;

                // Try to read the sector data
                var sectorData = ReadFromDataSource(sectorDataOffset, (int)SectorSize);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return [.. data];
        }

        /// <summary>
        /// Convert a FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
        public long FATSectorToFileOffset(Models.CFB.SectorNumber? sector)
        {
            // If we have an invalid sector number
            if (sector == null || sector > Models.CFB.SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)(sector + 1) * SectorSize;
        }

        #endregion

        #region Mini FAT Sector Data

        /// <summary>
        /// Get the ordered Mini FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<Models.CFB.SectorNumber>? GetMiniFATSectorChain(Models.CFB.SectorNumber? startingSector)
        {
            // If we have an invalid sector
            if (startingSector == null || startingSector < 0 || Model.MiniFATSectorNumbers == null || (long)startingSector >= Model.MiniFATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<Models.CFB.SectorNumber> { startingSector.Value };

            var lastSector = startingSector;
            while (true)
            {
                if (lastSector == null)
                    break;

                // Get the next sector from the lookup table
                var nextSector = Model.MiniFATSectorNumbers[(uint)lastSector!.Value];

                // If we have an end of chain or free sector
                if (nextSector == Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == Models.CFB.SectorNumber.FREESECT)
                    break;

                // Add the next sector to the list and replace the last sector
                sectors.Add(nextSector);
                lastSector = nextSector;
            }

            return sectors;
        }

        /// <summary>
        /// Get the data for the Mini FAT sector chain starting at a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public byte[]? GetMiniFATSectorChainData(Models.CFB.SectorNumber startingSector)
        {
            // Get the sector chain first
            var sectorChain = GetMiniFATSectorChain(startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the sector data offset
                int sectorDataOffset = (int)MiniFATSectorToFileOffset(sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= GetEndOfFile())
                    return null;

                // Try to read the sector data
                var sectorData = ReadFromDataSource(sectorDataOffset, (int)MiniSectorSize);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return [.. data];
        }

        /// <summary>
        /// Convert a Mini FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
        public long MiniFATSectorToFileOffset(Models.CFB.SectorNumber? sector)
        {
            // If we have an invalid sector number
            if (sector == null || sector > Models.CFB.SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)(sector + 1) * MiniSectorSize;
        }

        #endregion
    }
}
