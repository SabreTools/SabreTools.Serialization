namespace SabreTools.Data.Models.XboxExecutable
{
    /// <summary>
    /// XBox Executable format
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    public class Executable
    {
        /// <summary>
        /// XBE header
        /// </summary>
        public Header? Header { get; set; }

        /// <summary>
        /// Certificate structure pointed to by <see cref="Header.CertificateAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public Certificate? Certificate { get; set; }

        /// <summary>
        /// Section headers pointed to by <see cref="Header.SectionHeadersAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public SectionHeader[] SectionHeaders { get; set; } = [];

        /// <summary>
        /// Thread Local Storage (TLS) structure pointed to by <see cref="Header.TLSAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public ThreadLocalStorage? ThreadLocalStorage { get; set; }

        /// <summary>
        /// Library versions pointed to by <see cref="Header.LibraryVersionsAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public LibraryVersion[] LibraryVersions { get; set; } = [];

        /// <summary>
        /// Kernel library version pointed to by <see cref="Header.KernelLibraryVersionAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public LibraryVersion? KernelLibraryVersion { get; set; }

        /// <summary>
        /// XAPI library version pointed to by <see cref="Header.XAPILibraryVersionAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public LibraryVersion? XAPILibraryVersion { get; set; }
    }
}
