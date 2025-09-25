using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.COFF;
using SabreTools.Models.PortableExecutable;
using SabreTools.Models.PortableExecutable.Resource.Entries;
using SabreTools.Serialization.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PortableExecutable : WrapperBase<Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Portable Executable (PE)";

        #endregion

        #region Extension Properties

        /// <inheritdoc cref="Executable.FileHeader"/>
        public FileHeader? COFFFileHeader => Model.FileHeader;

        /// <summary>
        /// Dictionary of debug data
        /// </summary>
        public Dictionary<int, object> DebugData
        {
            get
            {
                lock (_debugDataLock)
                {
                    // Use the cached data if possible
                    if (_debugData.Count != 0)
                        return _debugData;

                    // If we have no resource table, just return
                    if (DebugDirectoryTable == null || DebugDirectoryTable.Length == 0)
                        return _debugData;

                    // Otherwise, build and return the cached dictionary
                    ParseDebugTable();
                    return _debugData;
                }
            }
        }

        /// <inheritdoc cref="Models.PortableExecutable.DebugData.Table.Table"/>
        public Models.PortableExecutable.DebugData.Entry[]? DebugDirectoryTable
            => Model.DebugTable?.DebugDirectoryTable;

        /// <summary>
        /// Entry point data, if it exists
        /// </summary>
        public byte[] EntryPointData
        {
            get
            {
                lock (_entryPointDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_entryPointData != null)
                        return _entryPointData;

                    // If the section table is missing
                    if (SectionTable == null)
                    {
                        _entryPointData = [];
                        return _entryPointData;
                    }

                    // If the address is missing
                    if (OptionalHeader?.AddressOfEntryPoint == null)
                    {
                        _entryPointData = [];
                        return _entryPointData;
                    }

                    // If we have no entry point
                    int entryPointAddress = (int)OptionalHeader.AddressOfEntryPoint.ConvertVirtualAddress(SectionTable);
                    if (entryPointAddress == 0)
                    {
                        _entryPointData = [];
                        return _entryPointData;
                    }

                    // If the entry point matches with the start of a section, use that
                    int entryPointSection = FindEntryPointSectionIndex();
                    if (entryPointSection >= 0 && OptionalHeader.AddressOfEntryPoint == SectionTable[entryPointSection]?.VirtualAddress)
                    {
                        _entryPointData = GetSectionData(entryPointSection) ?? [];
                        return _entryPointData;
                    }

                    // Read the first 128 bytes of the entry point
                    _entryPointData = ReadRangeFromSource(entryPointAddress, length: 128) ?? [];
                    return _entryPointData;
                }
            }
        }

        /// <inheritdoc cref="Executable.ExportAddressTable"/>
        public Models.PortableExecutable.Export.AddressTableEntry[]? ExportTable => Model.ExportAddressTable;

        /// <inheritdoc cref="Executable.ExportDirectoryTable"/>
        public Models.PortableExecutable.Export.DirectoryTable? ExportDirectoryTable => Model.ExportDirectoryTable;

        /// <inheritdoc cref="Executable.NamePointerTable"/>
        public Models.PortableExecutable.Export.NamePointerTable? ExportNamePointerTable => Model.NamePointerTable;

        /// <inheritdoc cref="Executable.ExportNameTable"/>
        public Models.PortableExecutable.Export.NameTable? ExportNameTable => Model.ExportNameTable;

        /// <inheritdoc cref="Executable.OrdinalTable"/>
        public Models.PortableExecutable.Export.OrdinalTable? ExportOrdinalTable => Model.OrdinalTable;

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
        public byte[] HeaderPaddingData
        {
            get
            {
                lock (_headerPaddingDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_headerPaddingData != null)
                        return _headerPaddingData;

                    // TODO: Don't scan the known header data as well

                    // If any required pieces are missing
                    if (Stub?.Header == null || SectionTable == null)
                    {
                        _headerPaddingData = [];
                        return _headerPaddingData;
                    }

                    // Populate the raw header padding data based on the source
                    uint headerStartAddress = Stub.Header.NewExeHeaderAddr;
                    uint firstSectionAddress = uint.MaxValue;
                    foreach (var s in SectionTable)
                    {
                        if (s == null || s.PointerToRawData == 0)
                            continue;
                        if (s.PointerToRawData < headerStartAddress)
                            continue;

                        if (s.PointerToRawData < firstSectionAddress)
                            firstSectionAddress = s.PointerToRawData;
                    }

                    // Check if the header length is more than 0 before reading data
                    int headerLength = (int)(firstSectionAddress - headerStartAddress);
                    if (headerLength <= 0)
                    {
                        _headerPaddingData = [];
                        return _headerPaddingData;
                    }

                    _headerPaddingData = ReadRangeFromSource((int)headerStartAddress, headerLength) ?? [];
                    return _headerPaddingData;
                }
            }
        }

        /// <summary>
        /// Header padding strings, if they exist
        /// </summary>
        public List<string> HeaderPaddingStrings
        {
            get
            {
                lock (_headerPaddingStringsLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_headerPaddingStrings != null)
                        return _headerPaddingStrings;

                    // Get the header padding data, if possible
                    byte[] headerPaddingData = HeaderPaddingData;
                    if (headerPaddingData.Length == 0)
                    {
                        _headerPaddingStrings = [];
                        return _headerPaddingStrings;
                    }

                    // Otherwise, cache and return the strings
                    _headerPaddingStrings = headerPaddingData.ReadStringsFrom(charLimit: 3) ?? [];
                    return _headerPaddingStrings;
                }
            }
        }

        /// <inheritdoc cref="Executable.ImportAddressTables"/>
        public Dictionary<int, Models.PortableExecutable.Import.AddressTableEntry[]?>? ImportAddressTables => Model.ImportAddressTables;

        /// <inheritdoc cref="Executable.ImportDirectoryTable"/>
        public Models.PortableExecutable.Import.DirectoryTableEntry[]? ImportDirectoryTable => Model.ImportDirectoryTable;

        /// <inheritdoc cref="Executable.HintNameTable"/>
        public Models.PortableExecutable.Import.HintNameTableEntry[]? ImportHintNameTable => Model.HintNameTable;

        /// <inheritdoc cref="Executable.ImportLookupTables"/>
        public Dictionary<int, Models.PortableExecutable.Import.LookupTableEntry[]?>? ImportLookupTables => Model.ImportLookupTables;

        /// <summary>
        /// SecuROM Matroschka package wrapper, if it exists
        /// </summary>
        public SecuROMMatroschkaPackage? MatroschkaPackage
        {
            get
            {
                lock (_matroschkaPackageLock)
                {
                    // Use the cached data if possible
                    if (_matroschkaPackage != null)
                        return _matroschkaPackage;

                    // Check to see if creation has already been attempted
                    if (_matroschkaPackageFailed)
                        return null;

                    // Get the available source length, if possible
                    var dataLength = Length;
                    if (dataLength == -1)
                    {
                        _matroschkaPackageFailed = true;
                        return null;
                    }

                    // If the section table is missing
                    if (SectionTable == null)
                    {
                        _matroschkaPackageFailed = true;
                        return null;
                    }

                    // Find the matrosch or rcpacker section
                    SectionHeader? section = null;
                    foreach (var searchedSection in SectionTable)
                    {
                        string sectionName = Encoding.ASCII.GetString(searchedSection.Name ?? []).TrimEnd('\0');
                        if (sectionName != "matrosch" && sectionName != "rcpacker")
                            continue;

                        section = searchedSection;
                        break;
                    }

                    // Otherwise, it could not be found
                    if (section == null)
                    {
                        _matroschkaPackageFailed = true;
                        return null;
                    }

                    // Get the offset
                    long offset = section.VirtualAddress.ConvertVirtualAddress(SectionTable);
                    if (offset < 0 || offset >= Length)
                    {
                        _matroschkaPackageFailed = true;
                        return null;
                    }

                    // Read the section into a local array
                    var sectionLength = (int)section.VirtualSize;
                    var sectionData = ReadRangeFromSource(offset, sectionLength);
                    if (sectionData.Length == 0)
                    {
                        _matroschkaPackageFailed = true;
                        return null;
                    }

                    // Parse the package
                    _matroschkaPackage = SecuROMMatroschkaPackage.Create(sectionData, 0);
                    if (_matroschkaPackage?.Entries == null)
                        _matroschkaPackageFailed = true;

                    return _matroschkaPackage;
                }
            }
        }

        /// <inheritdoc cref="Executable.OptionalHeader"/>
        public Models.PortableExecutable.OptionalHeader? OptionalHeader => Model.OptionalHeader;

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        /// <see href="https://www.autoitscript.com/forum/topic/153277-pe-file-overlay-extraction/"/>
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
                    {
                        _overlayAddress = -1;
                        return _overlayAddress.Value;
                    }

                    // If the section table is missing
                    if (SectionTable == null)
                    {
                        _overlayAddress = -1;
                        return _overlayAddress.Value;
                    }

                    // If we have certificate data, use that as the end
                    if (OptionalHeader?.CertificateTable != null)
                    {
                        int certificateTableAddress = (int)OptionalHeader.CertificateTable.VirtualAddress;
                        if (certificateTableAddress != 0 && certificateTableAddress < dataLength)
                            dataLength = certificateTableAddress;
                    }

                    // Search through all sections and find the furthest a section goes
                    int endOfSectionData = -1;
                    foreach (var section in SectionTable)
                    {
                        // If we have an invalid section
                        if (section == null)
                            continue;

                        // If we have an invalid section address
                        int sectionAddress = (int)section.VirtualAddress.ConvertVirtualAddress(SectionTable);
                        if (sectionAddress == 0)
                            continue;

                        // If we have an invalid section size
                        if (section.SizeOfRawData == 0 && section.VirtualSize == 0)
                            continue;

                        // Get the real section size
                        int sectionSize = (int)section.SizeOfRawData;

                        // Compare and set the end of section data
                        if (sectionAddress + sectionSize > endOfSectionData)
                            endOfSectionData = sectionAddress + sectionSize;
                    }

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                        endOfSectionData = -1;

                    // If the section data is followed by the end of the data
                    if (endOfSectionData >= dataLength)
                        endOfSectionData = -1;

                    // Cache and return the position
                    _overlayAddress = endOfSectionData;
                    return _overlayAddress.Value;
                }
            }
        }

        /// <summary>
        /// Overlay data, if it exists
        /// </summary>
        /// <remarks>Can only cache up to <see cref="int.MaxValue"/> bytes</remarks> 
        /// <see href="https://www.autoitscript.com/forum/topic/153277-pe-file-overlay-extraction/"/>
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

                    // Get the overlay address and size if possible
                    long endOfSectionData = OverlayAddress;
                    long overlaySize = OverlaySize;

                    // If we didn't find the address or size
                    if (endOfSectionData <= 0 || overlaySize <= 0)
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
                    overlaySize = Math.Min(overlaySize, int.MaxValue);

                    _overlayData = ReadRangeFromSource(endOfSectionData, (int)overlaySize) ?? [];
                    return _overlayData;
                }
            }
        }

        /// <summary>
        /// Size of the overlay data, if it exists
        /// </summary>
        /// <see href="https://www.autoitscript.com/forum/topic/153277-pe-file-overlay-extraction/"/>
        public long OverlaySize
        {
            get
            {
                lock (_overlaySizeLock)
                {
                    // Use the cached data if possible
                    if (_overlaySize >= 0)
                        return _overlaySize;

                    // Get the available source length, if possible
                    long dataLength = Length;
                    if (dataLength == -1)
                    {
                        _overlaySize = 0;
                        return _overlaySize;
                    }

                    // If the section table is missing
                    if (SectionTable == null)
                    {
                        _overlaySize = 0;
                        return _overlaySize;
                    }

                    // If we have certificate data, use that as the end
                    if (OptionalHeader?.CertificateTable != null)
                    {
                        int certificateTableAddress = (int)OptionalHeader.CertificateTable.VirtualAddress;
                        if (certificateTableAddress != 0 && certificateTableAddress < dataLength)
                            dataLength = certificateTableAddress;
                    }

                    // Get the overlay address if possible
                    long endOfSectionData = OverlayAddress;

                    // If we didn't find the end of section data
                    if (endOfSectionData <= 0)
                    {
                        _overlaySize = 0;
                        return _overlaySize;
                    }

                    // If we're at the end of the file, cache an empty byte array
                    if (endOfSectionData >= dataLength)
                    {
                        _overlaySize = 0;
                        return _overlaySize;
                    }

                    // Otherwise, cache and return the length
                    _overlaySize = dataLength - endOfSectionData;
                    return _overlaySize;
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

        /// <inheritdoc cref="Executable.ResourceDirectoryTable"/>
        public Models.PortableExecutable.Resource.DirectoryTable? ResourceDirectoryTable => Model.ResourceDirectoryTable;

        /// <summary>
        /// Sanitized section names
        /// </summary>
        public string[] SectionNames
        {
            get
            {
                lock (_sectionNamesLock)
                {
                    // Use the cached data if possible
                    if (_sectionNames != null)
                        return _sectionNames;

                    // If there are no sections
                    if (SectionTable == null)
                    {
                        _sectionNames = [];
                        return _sectionNames;
                    }

                    // Otherwise, build and return the cached array
                    _sectionNames = new string[SectionTable.Length];
                    for (int i = 0; i < _sectionNames.Length; i++)
                    {
                        var section = SectionTable[i];
                        if (section == null)
                            continue;

                        // TODO: Handle long section names with leading `/`
                        byte[]? sectionNameBytes = section.Name;
                        if (sectionNameBytes != null)
                        {
                            string sectionNameString = Encoding.UTF8.GetString(sectionNameBytes).TrimEnd('\0');
                            _sectionNames[i] = sectionNameString;
                        }
                    }

                    return _sectionNames;
                }
            }
        }

        /// <inheritdoc cref="Executable.SectionTable"/>
        public SectionHeader[]? SectionTable => Model.SectionTable;

        /// <summary>
        /// Data after the section table, if it exists
        /// </summary>
        public byte[] SectionTableTrailerData
        {
            get
            {
                lock (_sectionTableTrailerDataLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_sectionTableTrailerData != null)
                        return _sectionTableTrailerData;

                    if (Stub?.Header?.NewExeHeaderAddr == null)
                    {
                        _sectionTableTrailerData = [];
                        return _sectionTableTrailerData;
                    }
                    if (COFFFileHeader == null)
                    {
                        _sectionTableTrailerData = [];
                        return _sectionTableTrailerData;
                    }

                    // Get the offset from the end of the section table
                    long endOfSectionTable = Stub.Header.NewExeHeaderAddr
                        + 24 // Signature size + file header size
                        + COFFFileHeader.SizeOfOptionalHeader
                        + (COFFFileHeader.NumberOfSections * 40); // Size of a section header

                    // Assume the extra data aligns to 512-byte segments
                    int alignment = (int)(OptionalHeader?.FileAlignment ?? 0x200);
                    int trailerDataSize = alignment - (int)(endOfSectionTable % alignment);

                    // Cache and return the section table trailer data, even if null
                    _sectionTableTrailerData = ReadRangeFromSource(endOfSectionTable, trailerDataSize);
                    return _sectionTableTrailerData;
                }
            }
        }

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

        /// <summary>
        /// Dictionary of resource data
        /// </summary>
        public Dictionary<string, object?> ResourceData
        {
            get
            {
                lock (_resourceDataLock)
                {
                    // Use the cached data if possible
                    if (_resourceData.Count != 0)
                        return _resourceData;

                    // If we have no resource table, just return
                    if (OptionalHeader?.ResourceTable == null
                        || OptionalHeader.ResourceTable.VirtualAddress == 0
                        || ResourceDirectoryTable == null)
                    {
                        return _resourceData;
                    }

                    // Otherwise, build and return the cached dictionary
                    ParseResourceDirectoryTable(ResourceDirectoryTable, types: []);
                    return _resourceData;
                }
            }
        }

        /// <summary>
        /// Wise section wrapper, if it exists
        /// </summary>
        public WiseSectionHeader? WiseSection
        {
            get
            {
                lock (_wiseSectionHeaderLock)
                {
                    // If we already have cached data, just use that immediately
                    if (_wiseSectionHeader != null)
                        return _wiseSectionHeader;

                    // If the header will not be found due to missing section data
                    if (_wiseSectionHeaderMissing)
                        return null;

                    // If the section table is invalid
                    if (SectionTable == null)
                    {
                        _wiseSectionHeaderMissing = true;
                        return null;
                    }

                    // Find the .WISE section
                    SectionHeader? wiseSection = null;
                    foreach (var section in SectionTable)
                    {
                        string sectionName = Encoding.ASCII.GetString(section.Name ?? []).TrimEnd('\0');
                        if (sectionName != ".WISE")
                            continue;

                        wiseSection = section;
                        break;
                    }

                    // If the section cannot be found
                    if (wiseSection == null)
                    {
                        _wiseSectionHeaderMissing = true;
                        return null;
                    }

                    // Get the physical offset of the section
                    long offset = wiseSection.VirtualAddress.ConvertVirtualAddress(SectionTable);
                    if (offset < 0 || offset >= Length)
                    {
                        _wiseSectionHeaderMissing = true;
                        return null;
                    }

                    // Read the section into a local array
                    int sectionLength = (int)wiseSection.VirtualSize;
                    byte[] sectionData = ReadRangeFromSource(offset, sectionLength);
                    if (sectionData.Length == 0)
                    {
                        _wiseSectionHeaderMissing = true;
                        return null;
                    }

                    // Parse the section header
                    _wiseSectionHeader = WiseSectionHeader.Create(sectionData, 0);
                    if (_wiseSectionHeader == null)
                        _wiseSectionHeaderMissing = true;

                    return _wiseSectionHeader;
                }
            }
        }

        #region Version Information

        /// <summary>
        /// "Build GUID"
        /// </summary/>
        public string? BuildGuid => GetVersionInfoString("BuildGuid");

        /// <summary>
        /// "Build signature"
        /// </summary/>
        public string? BuildSignature => GetVersionInfoString("BuildSignature");

        /// <summary>
        /// Additional information that should be displayed for diagnostic purposes.
        /// </summary/>
        public string? Comments => GetVersionInfoString("Comments");

        /// <summary>
        /// Company that produced the file—for example, "Microsoft Corporation" or
        /// "Standard Microsystems Corporation, Inc." This string is required.
        /// </summary/>
        public string? CompanyName => GetVersionInfoString("CompanyName");

        /// <summary>
        /// "Debug version"
        /// </summary/>
        public string? DebugVersion => GetVersionInfoString("DebugVersion");

        /// <summary>
        /// File description to be presented to users. This string may be displayed in a
        /// list box when the user is choosing files to install—for example, "Keyboard
        /// Driver for AT-Style Keyboards". This string is required.
        /// </summary/>
        public string? FileDescription => GetVersionInfoString("FileDescription");

        /// <summary>
        /// Version number of the file—for example, "3.10" or "5.00.RC2". This string
        /// is required.
        /// </summary/>
        public string? FileVersion => GetVersionInfoString("FileVersion");

        /// <summary>
        /// Internal name of the file, if one exists—for example, a module name if the
        /// file is a dynamic-link library. If the file has no internal name, this
        /// string should be the original filename, without extension. This string is required.
        /// </summary/>
        public string? InternalName => GetVersionInfoString(key: "InternalName");

        /// <summary>
        /// Copyright notices that apply to the file. This should include the full text of
        /// all notices, legal symbols, copyright dates, and so on. This string is optional.
        /// </summary/>
        public string? LegalCopyright => GetVersionInfoString(key: "LegalCopyright");

        /// <summary>
        /// Trademarks and registered trademarks that apply to the file. This should include
        /// the full text of all notices, legal symbols, trademark numbers, and so on. This
        /// string is optional.
        /// </summary/>
        public string? LegalTrademarks => GetVersionInfoString(key: "LegalTrademarks");

        /// <summary>
        /// Original name of the file, not including a path. This information enables an
        /// application to determine whether a file has been renamed by a user. The format of
        /// the name depends on the file system for which the file was created. This string
        /// is required.
        /// </summary/>
        public string? OriginalFilename => GetVersionInfoString(key: "OriginalFilename");

        /// <summary>
        /// Information about a private version of the file—for example, "Built by TESTER1 on
        /// \TESTBED". This string should be present only if VS_FF_PRIVATEBUILD is specified in
        /// the fileflags parameter of the root block.
        /// </summary/>
        public string? PrivateBuild => GetVersionInfoString(key: "PrivateBuild");

        /// <summary>
        /// "Product GUID"
        /// </summary/>
        public string? ProductGuid => GetVersionInfoString("ProductGuid");

        /// <summary>
        /// Name of the product with which the file is distributed. This string is required.
        /// </summary/>
        public string? ProductName => GetVersionInfoString(key: "ProductName");

        /// <summary>
        /// Version of the product with which the file is distributed—for example, "3.10" or
        /// "5.00.RC2". This string is required.
        /// </summary/>
        public string? ProductVersion => GetVersionInfoString(key: "ProductVersion");

        /// <summary>
        /// Text that specifies how this version of the file differs from the standard
        /// version—for example, "Private build for TESTER1 solving mouse problems on M250 and
        /// M250E computers". This string should be present only if VS_FF_SPECIALBUILD is
        /// specified in the fileflags parameter of the root block.
        /// </summary/>
        public string? SpecialBuild => GetVersionInfoString(key: "SpecialBuild") ?? GetVersionInfoString(key: "Special Build");

        /// <summary>
        /// "Trade name"
        /// </summary/>
        public string? TradeName => GetVersionInfoString(key: "TradeName");

        /// <summary>
        /// Get the internal version as reported by the resources
        /// </summary>
        /// <returns>Version string, null on error</returns>
        /// <remarks>The internal version is either the file version, product version, or assembly version, in that order</remarks>
        public string? GetInternalVersion()
        {
            string? version = FileVersion;
            if (!string.IsNullOrEmpty(version))
                return version!.Replace(", ", ".");

            version = ProductVersion;
            if (!string.IsNullOrEmpty(version))
                return version!.Replace(", ", ".");

            version = AssemblyVersion;
            if (!string.IsNullOrEmpty(version))
                return version;

            return null;
        }

        #endregion

        #region Manifest Information

        /// <summary>
        /// Description as derived from the assembly manifest
        /// </summary>
        public string? AssemblyDescription
        {
            get
            {
                var manifest = GetAssemblyManifest();
                return manifest?
                    .Description?
                    .Value;
            }
        }

        /// <summary>
        /// Name as derived from the assembly manifest
        /// </summary>
        /// <remarks>
        /// If there are multiple identities included in the manifest,
        /// this will only retrieve the value from the first that doesn't
        /// have a null or empty name.
        /// </remarks>
        public string? AssemblyName
        {
            get
            {
                var manifest = GetAssemblyManifest();
                var identities = manifest?.AssemblyIdentities ?? [];
                var nameIdentity = Array.Find(identities, ai => !string.IsNullOrEmpty(ai?.Name));
                return nameIdentity?.Name;
            }
        }

        /// <summary>
        /// Version as derived from the assembly manifest
        /// </summary>
        /// <remarks>
        /// If there are multiple identities included in the manifest,
        /// this will only retrieve the value from the first that doesn't
        /// have a null or empty version.
        /// </remarks>
        public string? AssemblyVersion
        {
            get
            {
                var manifest = GetAssemblyManifest();
                var identities = manifest?.AssemblyIdentities ?? [];
                var versionIdentity = Array.Find(identities, ai => !string.IsNullOrEmpty(ai?.Version));
                return versionIdentity?.Version;
            }
        }

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Cached debug data
        /// </summary>
        private readonly Dictionary<int, object> _debugData = [];

        /// <summary>
        /// Lock object for <see cref="_debugData"/> 
        /// </summary>
        private readonly object _debugDataLock = new();

        /// <summary>
        /// Entry point data, if it exists and isn't aligned to a section
        /// </summary>
        private byte[]? _entryPointData = null;

        /// <summary>
        /// Lock object for <see cref="_entryPointData"/> 
        /// </summary>
        private readonly object _entryPointDataLock = new();

        /// <summary>
        /// Header padding data, if it exists
        /// </summary>
        private byte[]? _headerPaddingData = null;

        /// <summary>
        /// Lock object for <see cref="_headerPaddingData"/> 
        /// </summary>
        private readonly object _headerPaddingDataLock = new();

        /// <summary>
        /// Header padding strings, if they exist
        /// </summary>
        private List<string>? _headerPaddingStrings = null;

        /// <summary>
        /// Lock object for <see cref="_headerPaddingStrings"/> 
        /// </summary>
        private readonly object _headerPaddingStringsLock = new();

        /// <summary>
        /// Matroschka Package wrapper, if it exists
        /// </summary>
        private SecuROMMatroschkaPackage? _matroschkaPackage = null;

        /// <summary>
        /// Lock object for <see cref="_matroschkaPackage"/> 
        /// </summary>
        private readonly object _matroschkaPackageLock = new();

        /// <summary>
        /// Cached attempt at creation for <see cref="_matroschkaPackage"/> 
        /// </summary>
        private bool _matroschkaPackageFailed = false;

        /// <summary>
        /// Address of the overlay, if it exists
        /// </summary>
        private int? _overlayAddress = null;

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
        /// Size of the overlay data, if it exists
        /// </summary>
        private long _overlaySize = -1;

        /// <summary>
        /// Lock object for <see cref="_overlaySize"/> 
        /// </summary>
        private readonly object _overlaySizeLock = new();

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
        private List<string>? _overlayStrings = null;

        /// <summary>
        /// Lock object for <see cref="_overlayStrings"/> 
        /// </summary>
        private readonly object _overlayStringsLock = new();

        /// <summary>
        /// Cached resource data
        /// </summary>
        private readonly Dictionary<string, object?> _resourceData = [];

        /// <summary>
        /// Lock object for <see cref="_resourceData"/> 
        /// </summary>
        private readonly object _resourceDataLock = new();

        /// <summary>
        /// Sanitized section names
        /// </summary>
        private string[]? _sectionNames = null;

        /// <summary>
        /// Lock object for <see cref="_sectionNames"/> 
        /// </summary>
        private readonly object _sectionNamesLock = new();

        /// <summary>
        /// Cached raw section data
        /// </summary>
        private byte[][]? _sectionData = null;

        /// <summary>
        /// Cached found string data in sections
        /// </summary>
        private List<string>?[]? _sectionStringData = null;

        /// <summary>
        /// Lock object for <see cref="_sectionStringData"/> 
        /// </summary>
        private readonly object _sectionStringDataLock = new();

        /// <summary>
        /// Data after the section table, if it exists
        /// </summary>
        private byte[]? _sectionTableTrailerData = null;

        /// <summary>
        /// Lock object for <see cref="_sectionTableTrailerData"/> 
        /// </summary>
        private readonly object _sectionTableTrailerDataLock = new();

        /// <summary>
        /// Stub executable data, if it exists
        /// </summary>
        private byte[]? _stubExecutableData = null;

        /// <summary>
        /// Lock object for <see cref="_stubExecutableData"/> 
        /// </summary>
        private readonly object _stubExecutableDataLock = new();

        /// <summary>
        /// Cached raw table data
        /// </summary>
        private readonly byte[][] _tableData = new byte[16][];

        /// <summary>
        /// Cached found string data in tables
        /// </summary>
        private readonly List<string>?[] _tableStringData = new List<string>?[16];

        /// <summary>
        /// Wise section wrapper, if it exists
        /// </summary>
        private WiseSectionHeader? _wiseSectionHeader = null;

        /// <summary>
        /// Lock object for <see cref="_wiseSectionHeader"/> 
        /// </summary>
        private readonly object _wiseSectionHeaderLock = new();

        /// <summary>
        /// Indicates if <see cref="_wiseSectionHeader"/> cannot be found 
        /// </summary>
        private bool _wiseSectionHeaderMissing = false;

        #region Version Information

        /// <summary>
        /// Cached version info data
        /// </summary>
        private VersionInfo? _versionInfo = null;

        #endregion

        #region Manifest Information

        /// <summary>
        /// Cached assembly manifest data
        /// </summary>
        private AssemblyManifest? _assemblyManifest = null;

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public PortableExecutable(Executable model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public PortableExecutable(Executable model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PortableExecutable(Executable model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public PortableExecutable(Executable model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public PortableExecutable(Executable model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public PortableExecutable(Executable model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a PE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
        public static PortableExecutable? Create(byte[]? data, int offset)
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
        /// Create a PE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
        public static PortableExecutable? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.PortableExecutable().Deserialize(data);
                if (model == null)
                    return null;

                return new PortableExecutable(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Data

        // TODO: Cache all certificate objects

        /// <summary>
        /// Get the version info string associated with a key, if possible
        /// </summary>
        /// <param name="key">Case-insensitive key to find in the version info</param>
        /// <returns>String representing the data, null on error</returns>
        /// <remarks>
        /// This code does not take into account the locale and will find and return
        /// the first available value. This may not actually matter for version info,
        /// but it is worth mentioning.
        /// </remarks>
        public string? GetVersionInfoString(string key)
        {
            // If we have an invalid key, we can't do anything
            if (string.IsNullOrEmpty(key))
                return null;

            // Ensure the resource table has been parsed
            if (ResourceData == null)
                return null;

            // If we don't have string version info in this executable
            var stringTable = _versionInfo?.StringFileInfo?.Children;
            if (stringTable == null || stringTable.Length == 0)
                return null;

            // Try to find a key that matches
            StringData? match = null;
            foreach (var st in stringTable)
            {
                if (st.Children == null || st.Length == 0)
                    continue;

                // Return the match if found
                match = Array.Find(st.Children, sd => key.Equals(sd.Key, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    return match.Value?.TrimEnd('\0');
            }

            return null;
        }

        /// <summary>
        /// Get the assembly manifest, if possible
        /// </summary>
        /// <returns>Assembly manifest object, null on error</returns>
        private AssemblyManifest? GetAssemblyManifest()
        {
            // Use the cached data if possible
            if (_assemblyManifest != null)
                return _assemblyManifest;

            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return null;

            // Return the now-cached assembly manifest
            return _assemblyManifest;
        }

        #endregion

        #region Debug Data

        /// <summary>
        /// Find CodeView debug data by path
        /// </summary>
        /// <param name="path">Partial path to check for</param>
        /// <returns>List of matching debug data</returns>
        public List<object?> FindCodeViewDebugTableByPath(string path)
        {
            // Cache the debug data for easier reading
            var debugData = DebugData;
            if (debugData.Count == 0)
                return [];

            var debugFound = new List<object?>();
            foreach (var data in debugData.Values)
            {
                if (data == null)
                    continue;

                if (data is Models.PortableExecutable.DebugData.NB10ProgramDatabase n)
                {
                    if (n.PdbFileName == null || !n.PdbFileName.Contains(path))
                        continue;

                    debugFound.Add(n);
                }
                else if (data is Models.PortableExecutable.DebugData.RSDSProgramDatabase r)
                {
                    if (r.PathAndFileName == null || !r.PathAndFileName.Contains(path))
                        continue;

                    debugFound.Add(r);
                }
            }

            return debugFound;
        }

        /// <summary>
        /// Find unparsed debug data by string value
        /// </summary>
        /// <param name="value">String value to check for</param>
        /// <returns>List of matching debug data</returns>
        public List<byte[]?> FindGenericDebugTableByValue(string value)
        {
            // Cache the debug data for easier reading
            var debugData = DebugData;
            if (debugData.Count == 0)
                return [];

            var table = new List<byte[]?>();
            foreach (var data in debugData.Values)
            {
                if (data == null)
                    continue;
                if (data is not byte[] b || b == null)
                    continue;

                try
                {
                    string? arrayAsASCII = Encoding.ASCII.GetString(b);
                    if (arrayAsASCII.Contains(value))
                    {
                        table.Add(b);
                        continue;
                    }
                }
                catch { }

                try
                {
                    string? arrayAsUTF8 = Encoding.UTF8.GetString(b);
                    if (arrayAsUTF8.Contains(value))
                    {
                        table.Add(b);
                        continue;
                    }
                }
                catch { }

                try
                {
                    string? arrayAsUnicode = Encoding.Unicode.GetString(b);
                    if (arrayAsUnicode.Contains(value))
                    {
                        table.Add(b);
                        continue;
                    }
                }
                catch { }
            }

            return table;
        }

        #endregion

        #region Debug Parsing

        /// <summary>
        /// Parse the debug directory table information
        /// </summary>
        private void ParseDebugTable()
        {
            // If there is no debug table
            if (DebugDirectoryTable == null || DebugDirectoryTable.Length == 0)
                return;

            // Loop through all debug table entries
            for (int i = 0; i < DebugDirectoryTable.Length; i++)
            {
                var entry = DebugDirectoryTable[i];
                uint address = entry.PointerToRawData;
                uint size = entry.SizeOfData;

                // Read the entry data until we have the end of the stream
                byte[]? entryData;
                try
                {
                    entryData = ReadRangeFromSource((int)address, (int)size);
                    if (entryData.Length < 4)
                        continue;
                }
                catch (EndOfStreamException)
                {
                    return;
                }

                // If we have CodeView debug data, try to parse it
                if (entry.DebugType == DebugType.IMAGE_DEBUG_TYPE_CODEVIEW)
                {
                    // Read the signature
                    int offset = 0;
                    uint signature = entryData.ReadUInt32LittleEndian(ref offset);

                    // Reset the offset
                    offset = 0;

                    // NB10
                    if (signature == 0x3031424E)
                    {
                        var nb10ProgramDatabase = entryData.ParseNB10ProgramDatabase(ref offset);
                        if (nb10ProgramDatabase != null)
                        {
                            _debugData[i] = nb10ProgramDatabase;
                            continue;
                        }
                    }

                    // RSDS
                    else if (signature == 0x53445352)
                    {
                        var rsdsProgramDatabase = entryData.ParseRSDSProgramDatabase(ref offset);
                        if (rsdsProgramDatabase != null)
                        {
                            _debugData[i] = rsdsProgramDatabase;
                            continue;
                        }
                    }
                }
                else
                {
                    _debugData[i] = entryData;
                }
            }
        }

        #endregion

        #region Resource Data

        /// <summary>
        /// Find dialog box resources by title
        /// </summary>
        /// <param name="title">Dialog box title to check for</param>
        /// <returns>List of matching resources</returns>
        public List<DialogBoxResource?> FindDialogByTitle(string title)
        {
            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return [];

            var resources = new List<DialogBoxResource?>();
            foreach (var resource in resourceData.Values)
            {
                if (resource == null)
                    continue;
                if (resource is not DialogBoxResource dbr || dbr == null)
                    continue;

                if (dbr.DialogTemplate?.TitleResource?.Contains(title) ?? false)
                    resources.Add(dbr);
                else if (dbr.ExtendedDialogTemplate?.TitleResource?.Contains(title) ?? false)
                    resources.Add(dbr);
            }

            return resources;
        }

        /// <summary>
        /// Find dialog box resources by contained item title
        /// </summary>
        /// <param name="title">Dialog box item title to check for</param>
        /// <returns>List of matching resources</returns>
        public List<DialogBoxResource?> FindDialogBoxByItemTitle(string title)
        {
            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return [];

            var resources = new List<DialogBoxResource?>();
            foreach (var resource in resourceData.Values)
            {
                if (resource == null)
                    continue;
                if (resource is not DialogBoxResource dbr || dbr == null)
                    continue;

                if (dbr.DialogItemTemplates != null)
                {
                    var templates = Array.FindAll(dbr.DialogItemTemplates, dit => dit?.TitleResource != null);
                    if (Array.FindIndex(templates, dit => dit?.TitleResource?.Contains(title) == true) > -1)
                        resources.Add(dbr);
                }
                else if (dbr.ExtendedDialogItemTemplates != null)
                {
                    var templates = Array.FindAll(dbr.ExtendedDialogItemTemplates, edit => edit?.TitleResource != null);
                    if (Array.FindIndex(templates, edit => edit?.TitleResource?.Contains(title) == true) > -1)
                        resources.Add(dbr);
                }
            }

            return resources;
        }

        /// <summary>
        /// Find string table resources by contained string entry
        /// </summary>
        /// <param name="entry">String entry to check for</param>
        /// <returns>List of matching resources</returns>
        public List<Dictionary<int, string?>?> FindStringTableByEntry(string entry)
        {
            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return [];

            var stringTables = new List<Dictionary<int, string?>?>();
            foreach (var resource in resourceData.Values)
            {
                if (resource == null)
                    continue;
                if (resource is not Dictionary<int, string?> st || st == null)
                    continue;

                foreach (string? s in st.Values)
                {
#if NETFRAMEWORK || NETSTANDARD
                    if (s == null || !s.Contains(entry))
#else
                    if (s == null || !s.Contains(entry, StringComparison.OrdinalIgnoreCase))
#endif
                        continue;

                    stringTables.Add(st);
                    break;
                }
            }

            return stringTables;
        }

        /// <summary>
        /// Find unparsed resources by type name
        /// </summary>
        /// <param name="typeName">Type name to check for</param>
        /// <returns>List of matching resources</returns>
        public List<byte[]?> FindResourceByNamedType(string typeName)
        {
            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return [];

            var resources = new List<byte[]?>();
            foreach (var kvp in resourceData)
            {
                if (!kvp.Key.Contains(typeName))
                    continue;
                if (kvp.Value == null || kvp.Value is not byte[] b || b == null)
                    continue;

                resources.Add(b);
            }

            return resources;
        }

        /// <summary>
        /// Find unparsed resources by string value
        /// </summary>
        /// <param name="value">String value to check for</param>
        /// <returns>List of matching resources</returns>
        public List<byte[]?> FindGenericResource(string value)
        {
            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return [];

            var resources = new List<byte[]?>();
            foreach (var resource in resourceData.Values)
            {
                if (resource == null)
                    continue;
                if (resource is not byte[] b || b == null)
                    continue;

                try
                {
                    string? arrayAsASCII = Encoding.ASCII.GetString(b!);
                    if (arrayAsASCII.Contains(value))
                    {
                        resources.Add(b);
                        continue;
                    }
                }
                catch { }

                try
                {
                    string? arrayAsUTF8 = Encoding.UTF8.GetString(b!);
                    if (arrayAsUTF8.Contains(value))
                    {
                        resources.Add(b);
                        continue;
                    }
                }
                catch { }

                try
                {
                    string? arrayAsUnicode = Encoding.Unicode.GetString(b!);
                    if (arrayAsUnicode.Contains(value))
                    {
                        resources.Add(b);
                        continue;
                    }
                }
                catch { }
            }

            return resources;
        }

        /// <summary>
        /// Find the location of a Wise overlay header, if it exists
        /// </summary>
        /// <returns>Offset to the overlay header on success, -1 otherwise</returns>
        public long FindWiseOverlayHeader()
        {
            // Get the overlay offset
            long overlayOffset = OverlayAddress;

            lock (_dataSourceLock)
            {
                // Attempt to get the overlay header
                if (overlayOffset >= 0 && overlayOffset < Length)
                {
                    _dataSource.Seek(overlayOffset, SeekOrigin.Begin);
                    var header = WiseOverlayHeader.Create(_dataSource);
                    if (header != null)
                        return overlayOffset;
                }

                // Check section data
                foreach (var section in SectionTable ?? [])
                {
                    string sectionName = Encoding.ASCII.GetString(section.Name ?? []).TrimEnd('\0');
                    long sectionOffset = section.VirtualAddress.ConvertVirtualAddress(SectionTable);
                    _dataSource.Seek(sectionOffset, SeekOrigin.Begin);

                    var header = WiseOverlayHeader.Create(_dataSource);
                    if (header != null)
                        return sectionOffset;

                    // Check after the resource table
                    if (sectionName == ".rsrc")
                    {
                        // Data immediately following
                        long afterResourceOffset = sectionOffset + section.SizeOfRawData;
                        _dataSource.Seek(afterResourceOffset, SeekOrigin.Begin);

                        header = WiseOverlayHeader.Create(_dataSource);
                        if (header != null)
                            return afterResourceOffset;

                        // Data following padding data
                        _dataSource.Seek(afterResourceOffset, SeekOrigin.Begin);
                        _ = _dataSource.ReadNullTerminatedAnsiString();

                        afterResourceOffset = _dataSource.Position;
                        header = WiseOverlayHeader.Create(_dataSource);
                        if (header != null)
                            return afterResourceOffset;
                    }
                }
            }

            // If there are no resources
            if (OptionalHeader?.ResourceTable == null)
                return -1;

            // Cache the resource data for easier reading
            var resourceData = ResourceData;
            if (resourceData.Count == 0)
                return -1;

            // Get the resources that have an executable signature
            bool exeResources = false;
            foreach (var kvp in resourceData)
            {
                if (kvp.Value == null || kvp.Value is not byte[] ba)
                    continue;
                if (!ba.StartsWith(Models.MSDOS.Constants.SignatureBytes))
                    continue;

                exeResources = true;
                break;
            }

            // If there are no executable resources
            if (!exeResources)
                return -1;

            // Get the raw resource table offset
            long resourceTableOffset = OptionalHeader.ResourceTable.VirtualAddress.ConvertVirtualAddress(SectionTable);
            if (resourceTableOffset <= 0)
                return -1;

            lock (_dataSourceLock)
            {
                // Search the resource table data for the offset
                long resourceOffset = -1;
                _dataSource.Seek(resourceTableOffset, SeekOrigin.Begin);
                while (_dataSource.Position < resourceTableOffset + OptionalHeader.ResourceTable.Size && _dataSource.Position < _dataSource.Length)
                {
                    ushort possibleSignature = _dataSource.ReadUInt16();
                    if (possibleSignature == Models.MSDOS.Constants.SignatureUInt16)
                    {
                        resourceOffset = _dataSource.Position - 2;
                        break;
                    }

                    _dataSource.Seek(-1, SeekOrigin.Current);
                }

                // If there was no valid offset, somehow
                if (resourceOffset == -1)
                    return -1;

                // Parse the executable and recurse
                _dataSource.Seek(resourceOffset, SeekOrigin.Begin);
                var resourceExe = WrapperFactory.CreateExecutableWrapper(_dataSource);
                if (resourceExe is not PortableExecutable resourcePex)
                    return -1;

                return resourcePex.FindWiseOverlayHeader();
            }
        }

        #endregion

        #region Resource Parsing

        /// <summary>
        /// Parse the resource directory table information
        /// </summary>
        private void ParseResourceDirectoryTable(Models.PortableExecutable.Resource.DirectoryTable table, List<object> types)
        {
            if (table?.Entries == null)
                return;

            for (int i = 0; i < table.Entries.Length; i++)
            {
                var entry = table.Entries[i];
                var newTypes = new List<object>(types ?? []);

                if (entry.Name?.UnicodeString != null)
                    newTypes.Add(Encoding.Unicode.GetString(entry.Name.UnicodeString));
                else
                    newTypes.Add(entry.IntegerID);

                ParseResourceDirectoryEntry(entry, newTypes);
            }
        }

        /// <summary>
        /// Parse the name resource directory entry information
        /// </summary>
        private void ParseResourceDirectoryEntry(Models.PortableExecutable.Resource.DirectoryEntry entry, List<object> types)
        {
            if (entry.DataEntry != null)
                ParseResourceDataEntry(entry.DataEntry, types);
            else if (entry.Subdirectory != null)
                ParseResourceDirectoryTable(entry.Subdirectory, types);
        }

        /// <summary>
        /// Parse the resource data entry information
        /// </summary>
        /// <remarks>
        /// When caching the version information and assembly manifest, this code assumes that there is only one of each
        /// of those resources in the entire exectuable. This means that only the last found version or manifest will
        /// ever be cached.
        /// </remarks>
        private void ParseResourceDataEntry(Models.PortableExecutable.Resource.DataEntry entry, List<object> types)
        {
            // Create the key and value objects
            string key = types == null
                ? $"UNKNOWN_{Guid.NewGuid()}"
                : string.Join(", ", Array.ConvertAll([.. types], t => t.ToString()));

            object? value = entry.Data;

            // If we have a known resource type
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                try
                {
                    switch ((ResourceType)resourceType)
                    {
                        case ResourceType.RT_CURSOR:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_BITMAP:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_ICON:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_MENU:
                            value = entry.AsMenu();
                            break;
                        case ResourceType.RT_DIALOG:
                            value = entry.AsDialogBox();
                            break;
                        case ResourceType.RT_STRING:
                            value = entry.AsStringTable();
                            break;
                        case ResourceType.RT_FONTDIR:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_FONT:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_ACCELERATOR:
                            value = entry.AsAcceleratorTableResource();
                            break;
                        case ResourceType.RT_RCDATA:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_MESSAGETABLE:
                            value = entry.AsMessageResourceData();
                            break;
                        case ResourceType.RT_GROUP_CURSOR:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_GROUP_ICON:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_VERSION:
                            _versionInfo = entry.AsVersionInfo();
                            value = _versionInfo;
                            break;
                        case ResourceType.RT_DLGINCLUDE:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_PLUGPLAY:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_VXD:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_ANICURSOR:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_ANIICON:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_HTML:
                            value = entry.Data;
                            break;
                        case ResourceType.RT_MANIFEST:
                            _assemblyManifest = entry.AsAssemblyManifest();
                            value = _assemblyManifest;
                            break;
                        default:
                            value = entry.Data;
                            break;
                    }
                }
                catch
                {
                    // Fall back on byte array data for malformed items
                    value = entry.Data;
                }
            }

            // If we have a custom resource type
            else if (types != null && types.Count > 0 && types[0] is string)
            {
                value = entry.Data;
            }

            // Add the key and value to the cache
            _resourceData[key] = value;
        }

        #endregion

        #region Sections

        /// <summary>
        /// Determine if a section is contained within the section table
        /// </summary>
        /// <param name="sectionName">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>True if the section is in the executable, false otherwise</returns>
        public bool ContainsSection(string? sectionName, bool exact = false)
        {
            // If no section name is provided
            if (sectionName == null)
                return false;

            // Get all section names first
            if (SectionNames.Length == 0)
                return false;

            // If we're checking exactly, return only exact matches
            if (exact)
                return Array.FindIndex(SectionNames, n => n.Equals(sectionName)) > -1;

            // Otherwise, check if section name starts with the value
            else
                return Array.FindIndex(SectionNames, n => n.StartsWith(sectionName)) > -1;
        }

        /// <summary>
        /// Get the section index corresponding to the entry point, if possible
        /// </summary>
        /// <returns>Section index on success, null on error</returns>
        public int FindEntryPointSectionIndex()
        {
            // If the section table is missing
            if (SectionTable == null)
                return -1;

            // If the address is missing
            if (OptionalHeader?.AddressOfEntryPoint == null)
                return -1;

            // If we don't have an entry point
            if (OptionalHeader.AddressOfEntryPoint.ConvertVirtualAddress(SectionTable) == 0)
                return -1;

            // Otherwise, find the section it exists within
            return OptionalHeader.AddressOfEntryPoint.ContainingSectionIndex(SectionTable);
        }

        /// <summary>
        /// Get the first section based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader? GetFirstSection(string? name, bool exact = false)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the first index of the section
            int index = Array.IndexOf(SectionNames, name);
            if (index == -1)
                return null;

            // Return the section
            return SectionTable[index];
        }

        /// <summary>
        /// Get the last section based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader? GetLastSection(string? name, bool exact = false)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the last index of the section
            int index = Array.LastIndexOf(SectionNames, name);
            if (index == -1)
                return null;

            // Return the section
            return SectionTable[index];
        }

        /// <summary>
        /// Get the section based on index, if possible
        /// </summary>
        /// <param name="index">Index of the section to check for</param>
        /// <returns>Section data on success, null on error</returns>
        public SectionHeader? GetSection(int index)
        {
            // If we have no sections
            if (SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Return the section
            return SectionTable[index];
        }

        /// <summary>
        /// Get the first section data based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public byte[]? GetFirstSectionData(string? name, bool exact = false)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the first index of the section
            int index = Array.IndexOf(SectionNames, name);
            return GetSectionData(index);
        }

        /// <summary>
        /// Get the last section data based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section data on success, null on error</returns>
        public byte[]? GetLastSectionData(string? name, bool exact = false)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the last index of the section
            int index = Array.LastIndexOf(SectionNames, name);
            return GetSectionData(index);
        }

        /// <summary>
        /// Get the section data based on index, if possible
        /// </summary>
        /// <param name="index">Index of the section to check for</param>
        /// <returns>Section data on success, null on error</returns>
        public byte[]? GetSectionData(int index)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            // Get the section data from the table
            var section = SectionTable[index];
            if (section == null)
                return null;

            uint address = section.VirtualAddress.ConvertVirtualAddress(SectionTable);
            if (address == 0)
                return null;

            // Set the section size
            uint size = section.SizeOfRawData;

            // Create the section data array if we have to
            _sectionData ??= new byte[SectionNames.Length][];

            // If we already have cached data, just use that immediately
            if (_sectionData[index] != null && _sectionData[index].Length > 0)
                return _sectionData[index];

            // Populate the raw section data based on the source
            var sectionData = ReadRangeFromSource((int)address, (int)size);

            // Cache and return the section data
            _sectionData[index] = sectionData;
            return sectionData;
        }

        /// <summary>
        /// Get the first section strings based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section strings on success, null on error</returns>
        public List<string>? GetFirstSectionStrings(string? name, bool exact = false)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the first index of the section
            int index = Array.IndexOf(SectionNames, name);
            return GetSectionStrings(index);
        }

        /// <summary>
        /// Get the last section strings based on name, if possible
        /// </summary>
        /// <param name="name">Name of the section to check for</param>
        /// <param name="exact">True to enable exact matching of names, false for starts-with</param>
        /// <returns>Section strings on success, null on error</returns>
        public List<string>? GetLastSectionStrings(string? name, bool exact = false)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (!ContainsSection(name, exact))
                return null;

            // Get the last index of the section
            int index = Array.LastIndexOf(SectionNames, name);
            return GetSectionStrings(index);
        }

        /// <summary>
        /// Get the section strings based on index, if possible
        /// </summary>
        /// <param name="index">Index of the section to check for</param>
        /// <returns>Section strings on success, null on error</returns>
        public List<string>? GetSectionStrings(int index)
        {
            // If we have no sections
            if (SectionNames.Length == 0 || SectionTable == null || SectionTable.Length == 0)
                return null;

            // If the section doesn't exist
            if (index < 0 || index >= SectionTable.Length)
                return null;

            lock (_sectionStringDataLock)
            {
                // Create the section string array if we have to
                _sectionStringData ??= new List<string>?[SectionNames.Length];

                // If we already have cached data, just use that immediately
                if (_sectionStringData[index] != null)
                    return _sectionStringData[index];

                // Get the section data, if possible
                byte[]? sectionData = GetSectionData(index);
                if (sectionData == null || sectionData.Length == 0)
                {
                    _sectionStringData[index] = [];
                    return _sectionStringData[index];
                }

                // Otherwise, cache and return the strings
                _sectionStringData[index] = sectionData.ReadStringsFrom(charLimit: 3) ?? [];
                return _sectionStringData[index];
            }
        }

        #endregion

        #region Tables

        /// <summary>
        /// Get the table based on index, if possible
        /// </summary>
        /// <param name="index">Index of the table to check for</param>
        /// <returns>Table on success, null on error</returns>
        public DataDirectory? GetTable(int index)
        {
            // If the table doesn't exist
            if (OptionalHeader == null || index < 0 || index > 16)
                return null;

            return index switch
            {
                1 => OptionalHeader.ExportTable,
                2 => OptionalHeader.ImportTable,
                3 => OptionalHeader.ResourceTable,
                4 => OptionalHeader.ExceptionTable,
                5 => OptionalHeader.CertificateTable,
                6 => OptionalHeader.BaseRelocationTable,
                7 => OptionalHeader.Debug,
                8 => null, // Architecture Table
                9 => OptionalHeader.GlobalPtr,
                10 => OptionalHeader.ThreadLocalStorageTable,
                11 => OptionalHeader.LoadConfigTable,
                12 => OptionalHeader.BoundImport,
                13 => OptionalHeader.ImportAddressTable,
                14 => OptionalHeader.DelayImportDescriptor,
                15 => OptionalHeader.CLRRuntimeHeader,
                16 => null, // Reserved

                // Should never be possible
                _ => null,
            };
        }

        /// <summary>
        /// Get the table data based on index, if possible
        /// </summary>
        /// <param name="index">Index of the table to check for</param>
        /// <returns>Table data on success, null on error</returns>
        public byte[]? GetTableData(int index)
        {
            // If the table doesn't exist
            if (OptionalHeader == null || index < 0 || index > 16)
                return null;

            // If we already have cached data, just use that immediately
            if (_tableData[index] != null && _tableData[index].Length > 0)
                return _tableData[index];

            // Get the table from the optional header
            var table = GetTable(index);

            // Get the virtual address and size from the entries
            uint virtualAddress = table?.VirtualAddress ?? 0;
            uint size = table?.Size ?? 0;

            // If there is  no section table
            if (SectionTable == null)
                return null;

            // Get the physical address from the virtual one
            uint address = virtualAddress.ConvertVirtualAddress(SectionTable);
            if (address == 0 || size == 0)
                return null;

            // Populate the raw table data based on the source
            var tableData = ReadRangeFromSource((int)address, (int)size);

            // Cache and return the table data
            _tableData[index] = tableData;
            return tableData;
        }

        /// <summary>
        /// Get the table strings based on index, if possible
        /// </summary>
        /// <param name="index">Index of the table to check for</param>
        /// <returns>Table strings on success, null on error</returns>
        public List<string>? GetTableStrings(int index)
        {
            // If the table doesn't exist
            if (index < 0 || index > 16)
                return null;

            // If we already have cached data, just use that immediately
            if (_tableStringData[index] != null)
                return _tableStringData[index];

            // Get the table data, if possible
            byte[]? tableData = GetTableData(index);
            if (tableData == null || tableData.Length == 0)
            {
                _tableStringData[index] = [];
                return _tableStringData[index];
            }

            // Otherwise, cache and return the strings
            _tableStringData[index] = tableData.ReadStringsFrom(charLimit: 5) ?? [];
            return _tableStringData[index];
        }

        #endregion
    }
}
