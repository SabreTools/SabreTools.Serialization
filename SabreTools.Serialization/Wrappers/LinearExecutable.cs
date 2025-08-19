using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.LinearExecutable;

namespace SabreTools.Serialization.Wrappers
{
    public class LinearExecutable : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Linear Executable (LE/LX)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Executable.ObjectPageMap"/>
        public InformationBlock? InformationBlock => Model.InformationBlock;

        /// <inheritdoc cref="Executable.ObjectPageMap"/>
        public ObjectPageMapEntry[]? ObjectPageMap => Model.ObjectPageMap;

        /// <inheritdoc cref="Executable.ResourceTable"/>
        public ResourceTableEntry[]? ResourceTable => Model.ResourceTable;

        /// <inheritdoc cref="Executable.Stub"/>
        public Models.MSDOS.Executable? Stub => Model.Stub;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public LinearExecutable(Executable? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public LinearExecutable(Executable? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an LE/LX executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable? Create(byte[]? data, int offset)
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
        /// Create an LE/LX executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var executable = Deserializers.LinearExecutable.DeserializeStream(data);
                if (executable == null)
                    return null;

                return new LinearExecutable(executable, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Object Page Map

        /// <summary>
        /// Get a single object page map entry
        /// </summary>
        /// <param name="index">Object page map index to retrieve</param>
        /// <returns>Entry on success, null otherwise</returns>
        public ObjectPageMapEntry? GetObjectPageMapEntry(int index)
        {
            // If the object page map table is invalid
            if (ObjectPageMap == null || ObjectPageMap.Length == 0)
                return null;

            // If the index is invalid
            if (index < 0 || index >= ObjectPageMap.Length)
                return null;

            // Return the entry
            return ObjectPageMap[index];
        }

        /// <summary>
        /// Get the data for a single object page map entry
        /// </summary>
        /// <param name="index">Object page map index to retrieve</param>
        /// <returns>Entry data on success, null otherwise</returns>
        public byte[]? GetObjectPageMapEntryData(int index)
        {
            // Get the entry offset
            int offset = GetObjectPageMapEntryOffset(index);
            if (offset < 0)
                return null;

            // Get the entry length
            int length = GetObjectPageMapEntryLength(index);
            if (length < 0)
                return null;
            else if (length == 0)
                return [];

            // Read the entry data and return
            return ReadFromDataSource(offset, length);
        }

        /// <summary>
        /// Get the data length for a object page map entry
        /// </summary>
        /// <param name="index">Object page map index to retrieve</param>
        /// <returns>Entry length on success, -1 otherwise</returns>
        public int GetObjectPageMapEntryLength(int index)
        {
            // Get the matching entry
            var entry = GetObjectPageMapEntry(index);
            if (entry == null)
                return -1;

            // Return the reported length
            return entry.DataSize;
        }

        /// <summary>
        /// Get the data offset for a single object page map entry
        /// </summary>
        /// <param name="index">Object page map index to retrieve</param>
        /// <returns>Entry offset on success, -1 otherwise</returns>
        public int GetObjectPageMapEntryOffset(int index)
        {
            // If the information block is invalid
            if (InformationBlock == null)
                return -1;

            // Get the end of the file, if possible
            long endOfFile = GetEndOfFile();
            if (endOfFile == -1)
                return -1;

            // Get the matching entry
            var entry = GetObjectPageMapEntry(index);
            if (entry == null)
                return -1;

            // Verify the entry offset
            int offset = (int)(entry.PageDataOffset << (int)InformationBlock.BytesOnLastPage);
            if (offset < 0 || offset + entry.DataSize >= endOfFile)
                return -1;

            // Return the verified offset
            return offset;
        }

        #endregion

        #region Resource Table

        /// <summary>
        /// Get a single resource table entry
        /// </summary>
        /// <param name="index">Resource table index to retrieve</param>
        /// <returns>Entry on success, null otherwise</returns>
        public ResourceTableEntry? GetResourceTableEntry(int index)
        {
            // If the resource table table is invalid
            if (ResourceTable == null || ResourceTable.Length == 0)
                return null;

            // If the index is invalid
            if (index < 0 || index >= ResourceTable.Length)
                return null;

            // Return the entry
            return ResourceTable[index];
        }

        /// <summary>
        /// Get the data for a single resource table entry
        /// </summary>
        /// <param name="index">Resource table index to retrieve</param>
        /// <returns>Entry data on success, null otherwise</returns>
        public byte[]? GetResourceTableEntryData(int index)
        {
            // Get the entry
            var entry = GetResourceTableEntry(index);
            if (entry == null)
                return null;

            // Get the entry offset
            int offset = GetResourceTableEntryOffset(index);
            if (offset < 0)
                return null;

            // Get the entry length
            int length = GetResourceTableEntryLength(index);
            if (length < 0)
                return null;
            else if (length == 0)
                return [];

            // Get the matching object data
            var objectData = GetObjectPageMapEntryData(entry.ObjectNumber);
            if (objectData == null)
                return null;

            // Read the entry data and return
            return objectData.ReadBytes(ref offset, length);
        }

        /// <summary>
        /// Get the data length for a resource table entry
        /// </summary>
        /// <param name="index">Resource table index to retrieve</param>
        /// <returns>Entry length on success, -1 otherwise</returns>
        public int GetResourceTableEntryLength(int index)
        {
            // Get the matching entry
            var entry = GetResourceTableEntry(index);
            if (entry == null)
                return -1;

            // Return the reported length
            return (int)entry.ResourceSize;
        }

        /// <summary>
        /// Get the data offset for a single resource table entry
        /// </summary>
        /// <param name="index">Resource table index to retrieve</param>
        /// <returns>Entry offset on success, -1 otherwise</returns>
        public int GetResourceTableEntryOffset(int index)
        {
            // If the information block is invalid
            if (InformationBlock == null)
                return -1;

            // Get the end of the file, if possible
            long endOfFile = GetEndOfFile();
            if (endOfFile == -1)
                return -1;

            // Get the matching entry
            var entry = GetResourceTableEntry(index);
            if (entry == null)
                return -1;

            // Get the matching object length
            int objectLength = GetObjectPageMapEntryLength(entry.ObjectNumber);
            if (objectLength == -1)
                return -1;

            // Verify the entry offset
            if (entry.Offset < 0 || entry.Offset + entry.ResourceSize >= objectLength)
                return -1;

            // Return the verified offset
            return (int)entry.Offset;
        }

        #endregion

        #region REMOVE -- DO NOT USE

        /// <summary>
        /// Read an arbitrary range from the source
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns>Byte array representing the range, null on error</returns>
        [Obsolete]
        public byte[]? ReadArbitraryRange(int rangeStart = -1, long length = -1)
        {
            // If we have an unset range start, read from the start of the source
            if (rangeStart == -1)
                rangeStart = 0;

            // If we have an unset length, read the whole source
            if (length == -1)
                length = GetEndOfFile();

            return ReadFromDataSource(rangeStart, (int)length);
        }

        #endregion
    }
}
