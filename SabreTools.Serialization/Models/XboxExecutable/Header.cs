namespace SabreTools.Data.Models.XboxExecutable
{
    /// <summary>
    /// XBox Executable format header
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    /// <see href="https://github.com/Cxbx-Reloaded/Cxbx-Reloaded/blob/master/src/common/xbe/Xbe.h"/>
    public class Header
    {
        /// <summary>
        /// "XBEH"
        /// </summary>
        public byte[] MagicNumber { get; set; } = new byte[4];

        /// <summary>
        /// This is where a game is signed. Only on officially signed games is this field worthwhile.
        /// </summary>
        /// <remarks>256 bytes</remarks>
        public byte[] DigitalSignature { get; set; } = new byte[256];

        /// <summary>
        /// Address at which to load this .XBE. Typically this will be 0x00010000.
        /// </summary>
        public uint BaseAddress { get; set; }

        /// <summary>
        /// Number of bytes that should be reserved for headers.
        /// </summary>
        public uint SizeOfHeaders { get; set; }

        /// <summary>
        /// Number of bytes that should be reserved for this image.
        /// </summary>
        public uint SizeOfImage { get; set; }

        /// <summary>
        /// Number of bytes that should be reserved for image header.
        /// </summary>
        public uint SizeOfImageHeader { get; set; }

        /// <summary>
        /// Time and Date when this image was created. Standard windows format.
        /// </summary>
        public uint TimeDate { get; set; }

        /// <summary>
        /// Address to a Certificate structure, after the .XBE is loaded into memory.
        /// </summary>
        public uint CertificateAddress { get; set; }

        /// <summary>
        /// Number of sections contained in this .XBE.
        /// </summary>
        public uint NumberOfSections { get; set; }

        /// <summary>
        /// Address to an array of SectionHeader structures, after the .XBE is loaded into memory.
        /// </summary>
        public uint SectionHeadersAddress { get; set; }

        /// <summary>
        /// Various flags for this .XBE file.
        /// </summary>
        public InitializationFlags InitializationFlags { get; set; }

        /// <summary>
        /// Address to the Image entry point, after the .XBE is loaded into memory. This is where execution starts.
        /// </summary>
        /// <remarks>
        /// This value is encoded with an XOR key. Considering this is far too weak to be considered security,
        /// I assume this XOR is a clever method for discerning between Debug/Retail .XBE files without adding
        /// another field to the .XBE header. The XOR key is dependant on the build:
        ///
        ///   Debug = 0x94859D4B, Retail = 0xA8FC57AB
        ///
        /// To decode an entry point, you XOR with the debug key, then check if it is a valid entry point.
        /// If it is not, then you try again with the retail key.
        ///
        /// To decode an entry point, you XOR with the debug key, then check if it is a valid entry point.
        /// If it is not, then you try again with the retail key.
        ///
        /// Note: The Kernel Image Thunk Address member of this header must also be encoded as described later
        /// in this document.
        /// </remarks>
        public uint EntryPoint { get; set; }

        /// <summary>
        /// Address to a TLS (Thread Local Storage) structure.
        /// </summary>
        public uint TLSAddress { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PEStackCommit { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PEHeapReserve { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PEHeapCommit { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PEBaseAddress { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PESizeOfImage { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PEChecksum { get; set; }

        /// <summary>
        /// Copied from the PE file this .XBE was created from.
        /// </summary>
        public uint PETimeDate { get; set; }

        /// <summary>
        /// Address to the debug pathname
        /// (i.e. "D:\Nightlybuilds\011026.0\code\build\xbox\Release\simpsons.exe")
        /// </summary>
        public uint DebugPathNameAddress { get; set; }

        /// <summary>
        /// Address to the debug filename (i.e. "simpsons.exe")
        /// </summary>
        public uint DebugFileNameAddress { get; set; }

        /// <summary>
        /// Address to the debug unicode filename (i.e. L"simpsons.exe")
        /// </summary>
        public uint DebugUnicodeFileNameAddress { get; set; }

        /// <summary>
        /// Address to the Kernel Image Thunk Table, after the.XBE is loaded into memory.
        /// This is how .XBE files import kernel functions and data.
        /// </summary>
        /// <remarks>
        /// This value is encoded with an XOR key. Considering this is far too weak to be considered security,
        /// I assume this XOR is a clever method for discerning between Debug/Retail .XBE files without adding
        /// another field to the .XBE header. The XOR key is dependant on the build:
        ///
        ///   Debug = 0xEFB1F152, Retail = 0x5B6D40B6
        ///
        /// To encode a kernel thunk address, you simply XOR the real address with either Debug or Retail key,
        /// depending on if you want the XBox to see this as a Debug or Retail executable.
        ///
        /// To decode a kernel thunk address, you XOR with the debug key, then check if it is a valid address.
        /// If it is not, then you try again with the retail key.
        ///
        /// The Kernel Thunk Table itself is simply an array of pointers to Kernel imports. There are 366 possible
        /// imports, and the table is terminated with a zero dword (0x00000000). Typically the values in this table
        /// can be generated with the following formula:
        ///
        ///   KernelThunkTable[v] = ImportThunk + 0x80000000;
        ///
        /// so, for example, the import PsCreateSystemThreadEx, which has a thunk value of 255(0xFF) would be...
        ///
        ///   KernelThunkTable[v] = 0xFF + 0x80000000; // (0x800000FF)
        ///
        /// When the.XBE is loaded by the OS (or the CXBX Emulator), all kernel imports are replaced by a valid
        /// function or data type address.In the case of CXBX, the import table entry at which
        /// (KernelThunkTable[v] & 0x1FF == 0xFF) will be replaced by &cxbx_PsCreateSystemThreadEx (which is
        /// a wrapper function).
        ///
        /// Note: The Entry Point member of this header must also be encoded as described earlier in this document.
        /// </remarks>
        public uint KernelImageThunkAddress { get; set; }

        /// <summary>
        /// Address to the Non-Kernel Import Directory. It is typically safe to set this to zero.
        /// </summary>
        public uint NonKernelImportDirectoryAddress { get; set; }

        /// <summary>
        /// Number of Library Versions pointed to by Library Versions Address.
        /// </summary>
        public uint NumberOfLibraryVersions { get; set; }

        /// <summary>
        /// Address to an array of LibraryVersion structures, after the .XBE is loaded into memory.
        /// </summary>
        public uint LibraryVersionsAddress { get; set; }

        /// <summary>
        /// Address to a LibraryVersion structure, after the .XBE is loaded into memory.
        /// </summary>
        public uint KernelLibraryVersionAddress { get; set; }

        /// <summary>
        /// Address to a LibraryVersion structure, after the .XBE is loaded into memory.
        /// </summary>
        public uint XAPILibraryVersionAddress { get; set; }

        /// <summary>
        /// Address to the Logo Bitmap (Typically a "Microsoft" logo). This field can be set
        /// to zero, meaning there is no bitmap present.
        /// </summary>
        public uint LogoBitmapAddress { get; set; }

        /// <summary>
        /// Size (in bytes) of the Logo Bitmap data.
        /// </summary>
        public uint LogoBitmapSize { get; set; }
    }
}
