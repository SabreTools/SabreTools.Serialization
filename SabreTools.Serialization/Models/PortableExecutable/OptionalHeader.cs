namespace SabreTools.Data.Models.PortableExecutable
{
    /// <summary>
    /// Every image file has an optional header that provides information to the loader.
    /// This header is optional in the sense that some files (specifically, object files)
    /// do not have it. For image files, this header is required. An object file can have
    /// an optional header, but generally this header has no function in an object file
    /// except to increase its size.
    /// 
    /// Note that the size of the optional header is not fixed. The SizeOfOptionalHeader
    /// field in the COFF header must be used to validate that a probe into the file for
    /// a particular data directory does not go beyond SizeOfOptionalHeader.
    /// 
    /// The NumberOfRvaAndSizes field of the optional header should also be used to ensure
    /// that no probe for a particular data directory entry goes beyond the optional header.
    /// In addition, it is important to validate the optional header magic number for format
    /// compatibility.
    /// 
    /// The optional header magic number determines whether an image is a PE32 or
    /// PE32+ executable.
    /// 
    /// PE32+ images allow for a 64-bit address space while limiting the image size to
    /// 2 gigabytes. Other PE32+ modifications are addressed in their respective sections.
    /// 
    /// The first eight fields of the optional header are standard fields that are defined
    /// for every implementation of COFF. These fields contain general information that is
    /// useful for loading and running an executable file. They are unchanged for the
    /// PE32+ format.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class OptionalHeader : COFF.OptionalHeader
    {
        /// <summary>
        /// The preferred address of the first byte of image when loaded into memory { get; set; }
        /// must be a multiple of 64 K. The default for DLLs is 0x10000000. The default
        /// for Windows CE EXEs is 0x00010000. The default for Windows NT, Windows 2000,
        /// Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong ImageBase { get; set; }

        /// <summary>
        /// The alignment (in bytes) of sections when they are loaded into memory. It must
        /// be greater than or equal to FileAlignment. The default is the page size for
        /// the architecture.
        /// </summary>
        public uint SectionAlignment { get; set; }

        /// <summary>
        /// The alignment factor (in bytes) that is used to align the raw data of sections
        /// in the image file. The value should be a power of 2 between 512 and 64 K,
        /// inclusive. The default is 512. If the SectionAlignment is less than the
        /// architecture's page size, then FileAlignment must match SectionAlignment.
        /// </summary>
        public uint FileAlignment { get; set; }

        /// <summary>
        /// The major version number of the required operating system.
        /// </summary>
        public ushort MajorOperatingSystemVersion { get; set; }

        /// <summary>
        /// The minor version number of the required operating system.
        /// </summary>
        public ushort MinorOperatingSystemVersion { get; set; }

        /// <summary>
        /// The major version number of the image.
        /// </summary>
        public ushort MajorImageVersion { get; set; }

        /// <summary>
        /// The minor version number of the image.
        /// </summary>
        public ushort MinorImageVersion { get; set; }

        /// <summary>
        /// The major version number of the subsystem.
        /// </summary>
        public ushort MajorSubsystemVersion { get; set; }

        /// <summary>
        /// The minor version number of the subsystem.
        /// </summary>
        public ushort MinorSubsystemVersion { get; set; }

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint Win32VersionValue { get; set; }

        /// <summary>
        /// The size (in bytes) of the image, including all headers, as the image
        /// is loaded in memory. It must be a multiple of SectionAlignment.
        /// </summary>
        public uint SizeOfImage { get; set; }

        /// <summary>
        /// The combined size of an MS-DOS stub, PE header, and section headers rounded
        /// up to a multiple of FileAlignment.
        /// </summary>
        public uint SizeOfHeaders { get; set; }

        /// <summary>
        /// The image file checksum. The algorithm for computing the checksum is
        /// incorporated into IMAGHELP.DLL. The following are checked for validation at
        /// load time: all drivers, any DLL loaded at boot time, and any DLL that is
        /// loaded into a critical Windows process.
        /// </summary>
        public uint CheckSum { get; set; }

        /// <summary>
        /// The subsystem that is required to run this image.
        /// </summary>
        public WindowsSubsystem Subsystem { get; set; }

        /// <summary>
        /// DLL characteristics
        /// </summary>
        public DllCharacteristics DllCharacteristics { get; set; }

        /// <summary>
        /// The size of the stack to reserve. Only SizeOfStackCommit is committed; the rest
        /// is made available one page at a time until the reserve size is reached.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SizeOfStackReserve { get; set; }

        /// <summary>
        /// The size of the stack to commit. 
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SizeOfStackCommit { get; set; }

        /// <summary>
        /// The size of the local heap space to reserve. Only SizeOfHeapCommit is
        /// committed; the rest is made available one page at a time until the reserve
        /// size is reached.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SizeOfHeapReserve { get; set; }

        /// <summary>
        /// The size of the local heap space to commit.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SizeOfHeapCommit { get; set; }

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint LoaderFlags { get; set; }

        /// <summary>
        /// The number of data-directory entries in the remainder of the optional header.
        /// Each describes a location and size.
        /// </summary>
        public uint NumberOfRvaAndSizes { get; set; }

        #region Data Directories (Image Only)

        /// <summary>
        /// The export table address and size.
        /// </summary>
        public DataDirectory? ExportTable { get; set; }

        /// <summary>
        /// The import table address and size.
        /// </summary>
        public DataDirectory? ImportTable { get; set; }

        /// <summary>
        /// The resource table address and size.
        /// </summary>
        public DataDirectory? ResourceTable { get; set; }

        /// <summary>
        /// The exception table address and size.
        /// </summary>
        public DataDirectory? ExceptionTable { get; set; }

        /// <summary>
        /// The attribute certificate table address and size.
        /// </summary>
        public DataDirectory? CertificateTable { get; set; }

        /// <summary>
        /// The base relocation table address and size.
        /// </summary>
        public DataDirectory? BaseRelocationTable { get; set; }

        /// <summary>
        /// The debug data starting address and size.
        /// </summary>
        public DataDirectory? Debug { get; set; }

        /// <summary>
        /// Reserved, must be 0
        /// </summary>
        public ulong Architecture { get; set; }

        /// <summary>
        /// The RVA of the value to be stored in the global pointer register.
        /// The size member of this structure must be set to zero.
        /// </summary>
        public DataDirectory? GlobalPtr { get; set; }

        /// <summary>
        /// The thread local storage (TLS) table address and size.
        /// </summary>
        public DataDirectory? ThreadLocalStorageTable { get; set; }

        /// <summary>
        /// The load configuration table address and size.
        /// </summary>
        public DataDirectory? LoadConfigTable { get; set; }

        /// <summary>
        /// The bound import table address and size.
        /// </summary>
        public DataDirectory? BoundImport { get; set; }

        /// <summary>
        /// The import address table address and size
        /// </summary>
        public DataDirectory? ImportAddressTable { get; set; }

        /// <summary>
        /// The delay import descriptor address and size.
        /// </summary>
        public DataDirectory? DelayImportDescriptor { get; set; }

        /// <summary>
        /// The CLR runtime header address and size.
        /// </summary>
        public DataDirectory? CLRRuntimeHeader { get; set; }

        /// <summary>
        /// Reserved, must be zero
        /// </summary>
        public ulong Reserved { get; set; }

        #endregion
    }
}
