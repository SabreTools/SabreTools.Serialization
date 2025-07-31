using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CFB;

namespace SabreTools.Serialization.Wrappers
{
    public class CFB : WrapperBase<Binary>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Compact File Binary";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Binary.Header"/>
        public FileHeader? Header => Model.Header;

        /// <inheritdoc cref="Binary.DirectoryEntries"/>
        public DirectoryEntry[]? DirectoryEntries => Model.DirectoryEntries;

        /// <inheritdoc cref="Binary.FATSectorNumbers"/>
        public SectorNumber[]? FATSectorNumbers => Model.FATSectorNumbers;

        /// <inheritdoc cref="Binary.MiniFATSectorNumbers"/>
        public SectorNumber[]? MiniFATSectorNumbers => Model.MiniFATSectorNumbers;

        /// <summary>
        /// Byte array representing the mini stream
        /// </summary>
        public byte[] MiniStreamData
        {
            get
            {
                // Use the cached value, if it exists
                if (_miniStreamData != null)
                    return _miniStreamData;

                // If there are no directory entries
                if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                    return [];

                // Get the mini stream offset from root object
                var startingSector = (SectorNumber)DirectoryEntries[0].StartingSectorLocation;

                // Get the mini stream data
                _miniStreamData = GetFATSectorChainData(startingSector);
                return _miniStreamData ?? [];
            }
        }
        private byte[]? _miniStreamData;

        /// <summary>
        /// Normal sector size in bytes
        /// </summary>
        public long SectorSize => (long)Math.Pow(2, Header?.SectorShift ?? 0);

        /// <summary>
        /// Mini sector size in bytes
        /// </summary>
        public long MiniSectorSize => (long)Math.Pow(2, Header?.MiniSectorShift ?? 0);

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public CFB(Binary? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public CFB(Binary? model, Stream? data)
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
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory, bool includeDebug)
        {
            // If we have no files
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all directory entries to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractEntry(i, outputDirectory, includeDebug);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the CFB to an output directory by index
        /// </summary>
        /// <param name="index">Entry index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractEntry(int index, string outputDirectory, bool includeDebug)
        {
            // If we have no entries
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= DirectoryEntries.Length)
                return false;

            // Get the entry information
            var entry = DirectoryEntries[index];
            if (entry == null)
                return false;

            // Only try to extract stream objects
            if (entry.ObjectType != ObjectType.StreamObject)
                return true;

            // Get the entry data
            byte[]? data = GetDirectoryEntryData(entry);
            if (data == null)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Ensure the output filename is trimmed
            string filename = entry.Name ?? $"entry{index}";
            byte[] nameBytes = Encoding.UTF8.GetBytes(filename);
            if (nameBytes[0] == 0xe4 && nameBytes[1] == 0xa1 && nameBytes[2] == 0x80)
                filename = Encoding.UTF8.GetString(nameBytes, 3, nameBytes.Length - 3);

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

            // Ensure directory separators are consistent
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
                fs.Write(data);
                fs.Flush();
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read the entry data for a single directory entry, if possible
        /// </summary>
        /// <param name="entry">Entry to try to retrieve data for</param>
        /// <returns>Byte array representing the entry data on success, null otherwise</returns>
        private byte[]? GetDirectoryEntryData(DirectoryEntry entry)
        {
            // If the CFB is invalid
            if (Header == null)
                return null;

            // Only try to extract stream objects
            if (entry.ObjectType != ObjectType.StreamObject)
                return null;

            // Determine which FAT is being used
            bool miniFat = entry.StreamSize < Header.MiniStreamCutoffSize;

            // Get the chain data
            var chain = miniFat
                ? GetMiniFATSectorChainData((SectorNumber)entry.StartingSectorLocation)
                : GetFATSectorChainData((SectorNumber)entry.StartingSectorLocation);
            if (chain == null)
                return null;

            // Return only the proper amount of data
            byte[] data = new byte[entry.StreamSize];
            Array.Copy(chain, 0, data, 0, (int)Math.Min(chain.Length, (long)entry.StreamSize));
            return data;
        }

        #endregion

        #region FAT Sector Data

        /// <summary>
        /// Get the ordered FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<SectorNumber>? GetFATSectorChain(SectorNumber? startingSector)
        {
            // If we have an invalid sector
            if (startingSector == null || startingSector < 0 || FATSectorNumbers == null || (long)startingSector >= FATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<SectorNumber> { startingSector.Value };

            var lastSector = startingSector;
            while (true)
            {
                if (lastSector == null)
                    break;

                // Get the next sector from the lookup table
                var nextSector = FATSectorNumbers[(uint)lastSector!.Value];

                // If we have an invalid sector
                if (nextSector >= SectorNumber.MAXREGSECT)
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
        public byte[]? GetFATSectorChainData(SectorNumber startingSector)
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
        public long FATSectorToFileOffset(SectorNumber? sector)
        {
            // If we have an invalid sector number
            if (sector == null || sector > SectorNumber.MAXREGSECT)
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
        public List<SectorNumber>? GetMiniFATSectorChain(SectorNumber? startingSector)
        {
            // If we have an invalid sector
            if (startingSector == null || startingSector < 0 || MiniFATSectorNumbers == null || (long)startingSector >= MiniFATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<SectorNumber> { startingSector.Value };

            var lastSector = startingSector;
            while (true)
            {
                if (lastSector == null)
                    break;

                // Get the next sector from the lookup table
                var nextSector = MiniFATSectorNumbers[(uint)lastSector!.Value];

                // If we have an invalid sector
                if (nextSector >= SectorNumber.MAXREGSECT)
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
        public byte[]? GetMiniFATSectorChainData(SectorNumber startingSector)
        {
            // Validate the mini stream data
            if (MiniStreamData == null)
                return null;

            // Get the sector chain
            var sectorChain = GetMiniFATSectorChain(startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the mini stream data offset
                int streamDataOffset = (int)MiniFATSectorToMiniStreamOffset(sectorChain[i]);
                if (streamDataOffset < 0 || streamDataOffset > MiniStreamData.Length)
                    return null;

                // Try to read the sector data
                var sectorData = MiniStreamData.ReadBytes(ref streamDataOffset, (int)MiniSectorSize);
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
        /// <returns>Stream offset in bytes, -1 on error</returns>
        /// <remarks>Offset is within the mini stream, not the full file</remarks>
        public long MiniFATSectorToMiniStreamOffset(SectorNumber? sector)
        {
            // If we have an invalid sector number
            if (sector == null || sector > SectorNumber.MAXREGSECT)
                return -1;

            // Get the mini stream location
            return (long)sector * MiniSectorSize;
        }

        #endregion
    }
}
