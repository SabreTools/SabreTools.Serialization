using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabreTools.Serialization.Wrappers
{
    public class CFB : WrapperBase<Models.CFB.Binary>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Compact File Binary";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Normal sector size in bytes
        /// </summary>
#if NET48
        public long SectorSize => (long)Math.Pow(2, this.Model.Header.SectorShift);
#else
        public long SectorSize => (long)Math.Pow(2, this.Model.Header?.SectorShift ?? 0);
#endif

        /// <summary>
        /// Mini sector size in bytes
        /// </summary>
#if NET48
        public long MiniSectorSize => (long)Math.Pow(2, this.Model.Header.MiniSectorShift);
#else
        public long MiniSectorSize => (long)Math.Pow(2, this.Model.Header?.MiniSectorShift ?? 0);
#endif

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public CFB(Models.CFB.Binary model, byte[] data, int offset)
#else
        public CFB(Models.CFB.Binary? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public CFB(Models.CFB.Binary model, Stream data)
#else
        public CFB(Models.CFB.Binary? model, Stream? data)
#endif
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
#if NET48
        public static CFB Create(byte[] data, int offset)
#else
        public static CFB? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Compound File Binary from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Compound File Binary wrapper on success, null on failure</returns>
#if NET48
        public static CFB Create(Stream data)
#else
        public static CFB? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var binary = new Streams.CFB().Deserialize(data);
            if (binary == null)
                return null;

            try
            {
                return new CFB(binary, data);
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
#if NET48
        public List<Models.CFB.SectorNumber> GetFATSectorChain(Models.CFB.SectorNumber startingSector)
#else
        public List<Models.CFB.SectorNumber?>? GetFATSectorChain(Models.CFB.SectorNumber? startingSector)
#endif
        {
            // If we have an invalid sector
#if NET48
            if (startingSector < 0 || this.Model.FATSectorNumbers == null || (long)startingSector >= this.Model.FATSectorNumbers.Length)
#else
            if (startingSector == null || startingSector < 0 || this.Model.FATSectorNumbers == null || (long)startingSector >= this.Model.FATSectorNumbers.Length)
#endif
                return null;

            // Setup the returned list
#if NET48
            var sectors = new List<Models.CFB.SectorNumber> { startingSector };
#else
            var sectors = new List<Models.CFB.SectorNumber?> { startingSector };
#endif

            var lastSector = startingSector;
            while (true)
            {
#if NET6_0_OR_GREATER
                if (lastSector == null)
                    break;
#endif

                // Get the next sector from the lookup table
#if NET48
                var nextSector = this.Model.FATSectorNumbers[(uint)lastSector];
#else
                var nextSector = this.Model.FATSectorNumbers[(uint)lastSector!.Value];
#endif

                // If we have an end of chain or free sector
                if (nextSector == SabreTools.Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == SabreTools.Models.CFB.SectorNumber.FREESECT)
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
#if NET48
        public byte[] GetFATSectorChainData(Models.CFB.SectorNumber startingSector)
#else
        public byte[]? GetFATSectorChainData(Models.CFB.SectorNumber startingSector)
#endif
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

            return data.ToArray();
        }

        /// <summary>
        /// Convert a FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
#if NET48
        public long FATSectorToFileOffset(Models.CFB.SectorNumber sector)
#else
        public long FATSectorToFileOffset(Models.CFB.SectorNumber? sector)
#endif
        {
            // If we have an invalid sector number
#if NET48
            if (sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#else
            if (sector == null || sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#endif
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
#if NET48
        public List<Models.CFB.SectorNumber> GetMiniFATSectorChain(Models.CFB.SectorNumber startingSector)
#else
        public List<Models.CFB.SectorNumber?>? GetMiniFATSectorChain(Models.CFB.SectorNumber? startingSector)
#endif
        {
            // If we have an invalid sector
#if NET48
            if (startingSector < 0 || this.Model.MiniFATSectorNumbers == null || (long)startingSector >= this.Model.MiniFATSectorNumbers.Length)
#else
            if (startingSector == null || startingSector < 0 || this.Model.MiniFATSectorNumbers == null || (long)startingSector >= this.Model.MiniFATSectorNumbers.Length)
#endif
                return null;

            // Setup the returned list
#if NET48
            var sectors = new List<Models.CFB.SectorNumber> { startingSector };
#else
            var sectors = new List<Models.CFB.SectorNumber?> { startingSector };
#endif

            var lastSector = startingSector;
            while (true)
            {
#if NET6_0_OR_GREATER
                if (lastSector == null)
                    break;
#endif

                // Get the next sector from the lookup table
#if NET48
                var nextSector = this.Model.MiniFATSectorNumbers[(uint)lastSector];
#else
                var nextSector = this.Model.MiniFATSectorNumbers[(uint)lastSector!.Value];
#endif

                // If we have an end of chain or free sector
                if (nextSector == SabreTools.Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == SabreTools.Models.CFB.SectorNumber.FREESECT)
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
#if NET48
        public byte[] GetMiniFATSectorChainData(Models.CFB.SectorNumber startingSector)
#else
        public byte[]? GetMiniFATSectorChainData(Models.CFB.SectorNumber startingSector)
#endif
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

            return data.ToArray();
        }

        /// <summary>
        /// Convert a Mini FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
#if NET48
        public long MiniFATSectorToFileOffset(Models.CFB.SectorNumber sector)
#else
        public long MiniFATSectorToFileOffset(Models.CFB.SectorNumber? sector)
#endif
        {
            // If we have an invalid sector number
#if NET48
            if (sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#else
            if (sector == null || sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#endif
                return -1;

            // Convert based on the sector shift value
            return (long)(sector + 1) * MiniSectorSize;
        }

        #endregion
    }
}