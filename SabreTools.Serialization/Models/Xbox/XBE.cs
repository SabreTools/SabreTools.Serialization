namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XBox Executable format
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    public class XBE
    {
        /// <summary>
        /// XBE header
        /// </summary>
        public XBEHeader? Header { get; set; }

        /// <summary>
        /// Certificate structure pointed to by <see cref="XBEHeader.CertificateAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public XBECertificate? Certificate { get; set; }

        /// <summary>
        /// Section headers pointed to by <see cref="XBEHeader.SectionHeadersAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public XBESectionHeader[] SectionHeaders { get; set; } = [];

        /// <summary>
        /// Thread Local Storage (TLS) structure pointed to by <see cref="XBEHeader.TLSAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public XBEThreadLocalStorage? ThreadLocalStorage { get; set; }

        /// <summary>
        /// Library versions pointed to by <see cref="XBEHeader.LibraryVersionsAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public XBELibraryVersion[] LibraryVersions { get; set; } = [];

        /// <summary>
        /// Kernel library version pointed to by <see cref="XBEHeader.KernelLibraryVersionAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public XBELibraryVersion? KernelLibraryVersion { get; set; }

        /// <summary>
        /// XAPI library version pointed to by <see cref="XBEHeader.XAPILibraryVersionAddress"/>
        /// </summary>
        /// TODO: Determine if address is real or virtual
        public XBELibraryVersion? XAPILibraryVersion { get; set; }
    }
}
