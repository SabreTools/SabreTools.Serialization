using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.CFB;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class CFB : WrapperBase<Binary>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Compact File Binary";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Binary.Header"/>
        public FileHeader Header => Model.Header;

        /// <inheritdoc cref="Binary.DirectoryEntries"/>
        public DirectoryEntry[] DirectoryEntries => Model.DirectoryEntries;

        /// <inheritdoc cref="Binary.FATSectorNumbers"/>
        public SectorNumber[] FATSectorNumbers => Model.FATSectorNumbers;

        /// <inheritdoc cref="Binary.MiniFATSectorNumbers"/>
        public SectorNumber[] MiniFATSectorNumbers => Model.MiniFATSectorNumbers;

        /// <summary>
        /// Byte array representing the mini stream
        /// </summary>
        public byte[] MiniStreamData
        {
            get
            {
                // Use the cached value, if it exists
                if (field != null)
                    return field;

                // If there are no directory entries
                if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                    return [];

                // Get the mini stream offset from root object
                var startingSector = (SectorNumber)DirectoryEntries[0].StartingSectorLocation;

                // Get the mini stream data
                field = GetFATSectorChainData(startingSector);
                return field ?? [];
            }
        } = null;

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
        public CFB(Binary model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public CFB(Binary model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public CFB(Binary model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public CFB(Binary model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public CFB(Binary model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public CFB(Binary model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

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
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.CFB().Deserialize(data);
                if (model == null)
                    return null;

                return new CFB(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
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
                if (sectorDataOffset < 0 || sectorDataOffset >= Length)
                    return null;

                // Try to read the sector data
                var sectorData = ReadRangeFromSource(sectorDataOffset, (int)SectorSize);
                if (sectorData.Length == 0)
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
