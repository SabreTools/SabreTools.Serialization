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
        public Certificate? Certificate { get; set; }

        /// <summary>
        /// Section headers pointed to by <see cref="Header.SectionHeadersAddress"/>
        /// </summary>
        public SectionHeader[] SectionHeaders { get; set; } = [];

        /// <summary>
        /// Thread Local Storage (TLS) structure pointed to by <see cref="Header.TLSAddress"/>
        /// </summary>
        public ThreadLocalStorage? ThreadLocalStorage { get; set; }

        /// <summary>
        /// Library versions pointed to by <see cref="Header.LibraryVersionsAddress"/>
        /// </summary>
        public LibraryVersion[] LibraryVersions { get; set; } = [];

        /// <summary>
        /// Kernel library version pointed to by <see cref="Header.KernelLibraryVersionAddress"/>
        /// </summary>
        public LibraryVersion? KernelLibraryVersion { get; set; }

        /// <summary>
        /// XAPI library version pointed to by <see cref="Header.XAPILibraryVersionAddress"/>
        /// </summary>
        public LibraryVersion? XAPILibraryVersion { get; set; }
    }
}
