namespace SabreTools.Data.Models.COFF
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
    public class OptionalHeader
    {
        /// <summary>
        /// The unsigned integer that identifies the state of the image file. The most
        /// common number is 0x10B, which identifies it as a normal executable file.
        /// 0x107 identifies it as a ROM image, and 0x20B identifies it as a PE32+ executable.
        /// </summary>
        public OptionalHeaderMagicNumber Magic { get; set; }

        /// <summary>
        /// The linker major version number.
        /// </summary>
        public byte MajorLinkerVersion { get; set; }

        /// <summary>
        /// The linker minor version number.
        /// </summary>
        public byte MinorLinkerVersion { get; set; }

        /// <summary>
        /// The size of the code (text) section, or the sum of all code sections if there
        /// are multiple sections.
        /// </summary>
        public uint SizeOfCode { get; set; }

        /// <summary>
        /// The size of the initialized data section, or the sum of all such sections if
        /// there are multiple data sections.
        /// </summary>
        public uint SizeOfInitializedData { get; set; }

        /// <summary>
        /// The size of the uninitialized data section (BSS), or the sum of all such sections
        /// if there are multiple BSS sections.
        /// </summary>
        public uint SizeOfUninitializedData { get; set; }

        /// <summary>
        /// The address of the entry point relative to the image base when the executable file
        /// is loaded into memory. For program images, this is the starting address. For
        /// device drivers, this is the address of the initialization function. An entry point
        /// is optional for DLLs. When no entry point is present, this field must be zero.
        /// </summary>
        public uint AddressOfEntryPoint { get; set; }

        /// <summary>
        /// The address that is relative to the image base of the beginning-of-code section when
        /// it is loaded into memory.
        /// </summary>
        public uint BaseOfCode { get; set; }

        /// <summary>
        /// The address that is relative to the image base of the beginning-of-data section when
        /// it is loaded into memory.
        /// </summary>
        public uint BaseOfData { get; set; }
    }
}
