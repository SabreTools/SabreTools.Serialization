using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Models.NewExecutable;

namespace SabreTools.Serialization.Wrappers
{
    public partial class NewExecutable : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "New Executable (NE)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Executable.Header"/>
        public ExecutableHeader? Header => Model.Header;

        /// <inheritdoc cref="Executable.ImportedNameTable"/>
        public Dictionary<ushort, ImportedNameTableEntry>? ImportedNameTable => Model.ImportedNameTable;

        /// <inheritdoc cref="Executable.NonResidentNameTable"/>
        public NonResidentNameTableEntry[]? NonResidentNameTable => Model.NonResidentNameTable;

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        /// <see href="https://codeberg.org/CYBERDEV/REWise/src/branch/master/src/exefile.c"/>
        public long OverlayAddress
        {
            get
            {
                lock (_overlayAddressLock)
                {
                    // Use the cached data if possible
                    if (_overlayAddress != null)
                        return _overlayAddress.Value;

                    // Get the available source length, if possible
                    long dataLength = Length;
                    if (dataLength == -1)
                        return -1;

                    // If a required property is missing
                    if (Header == null || SegmentTable == null || ResourceTable?.ResourceTypes == null)
                        return -1;

                    // Search through the segments table to find the furthest
                    long endOfSectionData = -1;
                    foreach (var entry in SegmentTable)
                    {
                        // Get end of segment data
                        long offset = (entry.Offset * (1 << Header.SegmentAlignmentShiftCount)) + entry.Length;

                        // Read and find the end of the relocation data
#if NET20 || NET35
                        if ((entry.FlagWord & SegmentTableEntryFlag.RELOCINFO) != 0)
#else
                        if (entry.FlagWord.HasFlag(SegmentTableEntryFlag.RELOCINFO))
#endif
                        {
                            lock (_dataSourceLock)
                            {
                                _dataSource.Seek(offset, SeekOrigin.Begin);
                                var relocationData = Deserializers.NewExecutable.ParsePerSegmentData(_dataSource);

                                offset = _dataSource.Position;
                            }
                        }

                        if (offset > endOfSectionData)
                            endOfSectionData = offset;
                    }

                    // Search through the resources table to find the furthest
                    foreach (var entry in ResourceTable.ResourceTypes)
                    {
                        // Skip invalid entries
                        if (entry.ResourceCount == 0 || entry.Resources == null || entry.Resources.Length == 0)
                            continue;

                        foreach (var resource in entry.Resources)
                        {
                            int offset = (resource.Offset << ResourceTable.AlignmentShiftCount) + resource.Length;
                            if (offset > endOfSectionData)
                                endOfSectionData = offset;
                        }
                    }

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                        endOfSectionData = -1;

                    // Adjust the position of the data by 705 bytes
                    // TODO: Investigate what the byte data is
                    endOfSectionData += 705;

                    // Cache and return the position
                    _overlayAddress = endOfSectionData;
                    return _overlayAddress.Value;
                }
            }
        }

        /// <summary>
        /// Overlay data, if it exists
        /// </summary>
        /// <see href="https://codeberg.org/CYBERDEV/REWise/src/branch/master/src/exefile.c"/>
        public byte[] OverlayData
        {
            get
            {
                lock (_overlayDataLock)
                {
                    // Use the cached data if possible
                    if (_overlayData != null)
                        return _overlayData;

                    // Get the available source length, if possible
                    long dataLength = Length;
                    if (dataLength == -1)
                    {
                        _overlayData = [];
                        return _overlayData;
                    }

                    // If a required property is missing
                    if (Header == null || SegmentTable == null || ResourceTable?.ResourceTypes == null)
                    {
                        _overlayData = [];
                        return _overlayData;
                    }

                    // Get the overlay address if possible
                    long endOfSectionData = OverlayAddress;

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                    {
                        _overlayData = [];
                        return _overlayData;
                    }

                    // If we're at the end of the file, cache an empty byte array
                    if (endOfSectionData >= dataLength)
                    {
                        _overlayData = [];
                        return _overlayData;
                    }

                    // Otherwise, cache and return the data
                    long overlayLength = dataLength - endOfSectionData;
                    _overlayData = ReadRangeFromSource((int)endOfSectionData, (int)overlayLength);
                    return _overlayData;
                }
            }
        }

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
        public List<string> OverlayStrings
        {
            get
            {
                lock (_overlayStringsLock)
                {
                    // Use the cached data if possible
                    if (_overlayStrings != null)
                        return _overlayStrings;

                    // Get the available source length, if possible
                    long dataLength = Length;
                    if (dataLength == -1)
                    {
                        _overlayStrings = [];
                        return _overlayStrings;
                    }

                    // Get the overlay data, if possible
                    var overlayData = OverlayData;
                    if (overlayData.Length == 0)
                    {
                        _overlayStrings = [];
                        return _overlayStrings;
                    }

                    // Otherwise, cache and return the strings
                    _overlayStrings = overlayData.ReadStringsFrom(charLimit: 3) ?? [];
                    return _overlayStrings;
                }
            }
        }

        /// <inheritdoc cref="Executable.ResidentNameTable"/>
        public ResidentNameTableEntry[]? ResidentNameTable => Model.ResidentNameTable;

        /// <inheritdoc cref="Executable.ResourceTable"/>
        public ResourceTable? ResourceTable => Model.ResourceTable;

        /// <inheritdoc cref="Executable.SegmentTable"/>
        public SegmentTableEntry[]? SegmentTable => Model.SegmentTable;

        /// <inheritdoc cref="Executable.Stub"/>
        public Models.MSDOS.Executable? Stub => Model.Stub;

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
        public byte[] StubExecutableData
        {
            get
            {
                lock (_stubExecutableDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_stubExecutableData != null)
                        return _stubExecutableData;

                    if (Stub?.Header?.NewExeHeaderAddr == null)
                    {
                        _stubExecutableData = [];
                        return _stubExecutableData;
                    }

                    // Populate the raw stub executable data based on the source
                    int endOfStubHeader = 0x40;
                    int lengthOfStubExecutableData = (int)Stub.Header.NewExeHeaderAddr - endOfStubHeader;
                    _stubExecutableData = ReadRangeFromSource(endOfStubHeader, lengthOfStubExecutableData);

                    // Cache and return the stub executable data, even if null
                    return _stubExecutableData;
                }
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        private long? _overlayAddress = null;

        /// <summary>
        /// Lock object for <see cref="_overlayAddress"/> 
        /// </summary>
        private readonly object _overlayAddressLock = new();

        /// <summary>
        /// Overlay data, if it exists
        /// </summary>
        private byte[]? _overlayData = null;

        /// <summary>
        /// Lock object for <see cref="_overlayData"/> 
        /// </summary>
        private readonly object _overlayDataLock = new();

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
        private List<string>? _overlayStrings = null;

        /// <summary>
        /// Lock object for <see cref="_overlayStrings"/> 
        /// </summary>
        private readonly object _overlayStringsLock = new();

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
        private byte[]? _stubExecutableData = null;

        /// <summary>
        /// Lock object for <see cref="_stubExecutableData"/> 
        /// </summary>
        private readonly object _stubExecutableDataLock = new();

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public NewExecutable(Executable model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public NewExecutable(Executable model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NewExecutable(Executable model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public NewExecutable(Executable model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public NewExecutable(Executable model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NewExecutable(Executable model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an NE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
        public static NewExecutable? Create(byte[]? data, int offset)
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
        /// Create an NE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
        public static NewExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.NewExecutable.DeserializeStream(data);
                if (model == null)
                    return null;

                return new NewExecutable(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Resources

        /// <summary>
        /// Find the location of a Wise overlay header, if it exists
        /// </summary>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Offset to the overlay header on success, -1 otherwise</returns>
        public long FindWiseOverlayHeader()
        {
            // Get the overlay offset
            long overlayOffset = OverlayAddress;
            if (overlayOffset < 0 || overlayOffset >= Length)
                return -1;

            lock (_dataSourceLock)
            {
                // Attempt to get the overlay header
                _dataSource.Seek(overlayOffset, SeekOrigin.Begin);
                var header = WiseOverlayHeader.Create(_dataSource);
                if (header != null)
                    return overlayOffset;

                // Align and loop to see if it can be found
                _dataSource.Seek(overlayOffset, SeekOrigin.Begin);
                _dataSource.AlignToBoundary(0x10);
                overlayOffset = _dataSource.Position;
                while (_dataSource.Position < Length)
                {
                    _dataSource.Seek(overlayOffset, SeekOrigin.Begin);
                    header = WiseOverlayHeader.Create(_dataSource);
                    if (header != null)
                        return overlayOffset;

                    overlayOffset += 0x10;
                }

                header = null;
                return -1;
            }
        }

        /// <summary>
        /// Get a single resource entry
        /// </summary>
        /// <param name="id">Resource ID to retrieve</param>
        /// <returns>Resource on success, null otherwise</returns>
        public ResourceTypeResourceEntry? GetResource(int id)
        {
            // If the header is invalid
            if (Header == null)
                return null;

            // Get the available source length, if possible
            long dataLength = Length;
            if (dataLength == -1)
                return null;

            // If the resource table is invalid
            if (ResourceTable?.ResourceTypes == null || ResourceTable.ResourceTypes.Length == 0)
                return null;

            // Loop through the resources to find a matching ID
            foreach (var resourceType in ResourceTable.ResourceTypes)
            {
                // Skip invalid resource types
                if (resourceType.ResourceCount == 0 || resourceType.Resources == null || resourceType.Resources.Length == 0)
                    continue;

                // Loop through the entries to find a matching ID
                foreach (var resource in resourceType.Resources)
                {
                    // Skip non-matching entries
                    if (resource.ResourceID != id)
                        continue;

                    // Return the resource
                    return resource;
                }
            }

            // No entry could be found
            return null;
        }

        /// <summary>
        /// Get the data for a single resource entry
        /// </summary>
        /// <param name="id">Resource ID to retrieve</param>
        /// <returns>Resource data on success, null otherwise</returns>
        public byte[]? GetResourceData(int id)
        {
            // Get the resource offset
            int offset = GetResourceOffset(id);
            if (offset < 0)
                return null;

            // Get the resource length
            int length = GetResourceLength(id);
            if (length < 0)
                return null;
            else if (length == 0)
                return [];

            // Read the resource data and return
            return ReadRangeFromSource(offset, length);
        }

        /// <summary>
        /// Get the data length for a single resource entry
        /// </summary>
        /// <param name="id">Resource ID to retrieve</param>
        /// <returns>Resource length on success, -1 otherwise</returns>
        public int GetResourceLength(int id)
        {
            // Get the matching resource
            var resource = GetSegment(id);
            if (resource == null)
                return -1;

            // Return the reported length
            return resource.Length;
        }

        /// <summary>
        /// Get the data offset for a single resource entry
        /// </summary>
        /// <param name="id">Resource ID to retrieve</param>
        /// <returns>Resource offset on success, -1 otherwise</returns>
        public int GetResourceOffset(int id)
        {
            // Get the available source length, if possible
            long dataLength = Length;
            if (dataLength == -1)
                return -1;

            // If the resource table is invalid
            if (ResourceTable == null)
                return -1;

            // Get the matching resource
            var resource = GetSegment(id);
            if (resource == null)
                return -1;

            // Verify the resource offset
            int offset = resource.Offset << ResourceTable.AlignmentShiftCount;
            if (offset < 0 || offset + resource.Length >= dataLength)
                return -1;

            // Return the verified offset
            return offset;
        }

        #endregion

        #region Segments

        /// <summary>
        /// Get a single segment
        /// </summary>
        /// <param name="index">Segment index to retrieve</param>
        /// <returns>Segment on success, null otherwise</returns>
        public SegmentTableEntry? GetSegment(int index)
        {
            // If the segment table is invalid
            if (SegmentTable == null || SegmentTable.Length == 0)
                return null;

            // If the index is invalid
            if (index < 0 || index >= SegmentTable.Length)
                return null;

            // Return the segment
            return SegmentTable[index];
        }

        /// <summary>
        /// Get the data for a single segment
        /// </summary>
        /// <param name="index">Segment index to retrieve</param>
        /// <returns>Segment data on success, null otherwise</returns>
        public byte[]? GetSegmentData(int index)
        {
            // Get the segment offset
            int offset = GetSegmentOffset(index);
            if (offset < 0)
                return null;

            // Get the segment length
            int length = GetSegmentLength(index);
            if (length < 0)
                return null;
            else if (length == 0)
                return [];

            // Read the segment data and return
            return ReadRangeFromSource(offset, length);
        }

        /// <summary>
        /// Get the data length for a single segment
        /// </summary>
        /// <param name="index">Segment index to retrieve</param>
        /// <returns>Segment length on success, -1 otherwise</returns>
        public int GetSegmentLength(int index)
        {
            // Get the matching segment
            var segment = GetSegment(index);
            if (segment == null)
                return -1;

            // Return the reported length
            return segment.Length;
        }

        /// <summary>
        /// Get the data offset for a single segment
        /// </summary>
        /// <param name="index">Segment index to retrieve</param>
        /// <returns>Segment offset on success, -1 otherwise</returns>
        public int GetSegmentOffset(int index)
        {
            // If the header is invalid
            if (Header == null)
                return -1;

            // Get the available source length, if possible
            long dataLength = Length;
            if (dataLength == -1)
                return -1;

            // Get the matching segment
            var segment = GetSegment(index);
            if (segment == null)
                return -1;

            // Verify the segment offset
            int offset = segment.Offset << Header.SegmentAlignmentShiftCount;
            if (offset < 0 || offset + segment.Length >= dataLength)
                return -1;

            // Return the verified offset
            return offset;
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
                length = Length;

            return ReadRangeFromSource(rangeStart, (int)length);
        }

        #endregion
    }
}
