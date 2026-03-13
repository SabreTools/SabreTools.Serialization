namespace SabreTools.Data.Models.XboxExecutable
{
    /// <summary>
    /// XBox Executable certificate
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    public class Certificate
    {
        /// <summary>
        /// Number of bytes that should be reserved for this certificate.
        /// </summary>
        public uint SizeOfCertificate { get; set; }

        /// <summary>
        /// Time and Date when this certificate was created. Standard windows format.
        /// </summary>
        public uint TimeDate { get; set; }

        /// <summary>
        /// Title ID for this application. This field doesn't appear to matter with
        /// unsigned code, so it can be set to zero.
        /// </summary>
        public uint TitleID { get; set; }

        /// <summary>
        /// Title name for this application (i.e. L"The Simpsons Road Rage").
        /// This buffer contains enough room for 40 Unicode characters.
        /// </summary>
        public byte[] TitleName { get; set; } = new byte[0x50];

        /// <summary>
        /// Alternate Title IDs (16 4-byte DWORDs) for this certificate. These do not appear
        /// to matter with unsigned code (or signed code, for that matter), so they can all
        /// be set to zero.
        /// </summary>
        public uint[] AlternativeTitleIDs { get; set; } = new uint[16];

        /// <summary>
        /// Allowed media types for this .XBE.
        /// </summary>
        public AllowedMediaTypes AllowedMediaTypes { get; set; }

        /// <summary>
        /// Game region for this .XBE.
        /// </summary>
        public GameRegion GameRegion { get; set; }

        /// <summary>
        /// Game ratings for this .XBE. It is typically safe to set this to 0xFFFFFFFF.
        /// </summary>
        public uint GameRatings { get; set; }

        /// <summary>
        /// Disk Number. Typically zero.
        /// </summary>
        public uint DiskNumber { get; set; }

        /// <summary>
        /// Certificate Version.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// 16-byte LAN Key. An unsigned .XBE can just zero these out.
        /// </summary>
        public byte[] LANKey { get; set; } = new byte[16];

        /// <summary>
        /// 16-byte Signature Key. An unsigned .XBE can just zero these out.
        /// </summary>
        public byte[] SignatureKey { get; set; } = new byte[16];

        /// <summary>
        /// 16 x 16-byte Signature Keys. An unsigned .XBE can just zero these out.
        /// </summary>
        public byte[][] AlternateSignatureKeys { get; set; } = new byte[16][];
    }
}
