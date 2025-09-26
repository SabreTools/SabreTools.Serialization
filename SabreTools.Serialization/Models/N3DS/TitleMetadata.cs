namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// A format used to store information about a title (installed title, DLC, etc.)
    /// and all its installed contents, including which contents they consist of and
    /// their SHA256 hashes. 
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/Title_metadata"/>
    public sealed class TitleMetadata
    {
        /// <summary>
        /// Signature Type
        /// </summary>
        public SignatureType SignatureType { get; set; }

        /// <summary>
        /// Signature size
        /// </summary>
        public ushort SignatureSize { get; set; }

        /// <summary>
        /// Padding size
        /// </summary>
        public byte PaddingSize { get; set; }

        /// <summary>
        /// Signature
        /// </summary>
        public byte[]? Signature { get; set; }

        /// <summary>
        /// Padding
        /// </summary>
        public byte[]? Padding1 { get; set; }

        /// <summary>
        /// Signature Issuer
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public byte Version { get; set; }
        
        /// <summary>
        /// CaCrlVersion
        /// </summary>
        public byte CaCrlVersion { get; set; }

        /// <summary>
        /// SignerCrlVersion
        /// </summary>
        public byte SignerCrlVersion { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved1 { get; set; }

        /// <summary>
        /// System Version
        /// </summary>
        public ulong SystemVersion { get; set; }

        /// <summary>
        /// TitleID
        /// </summary>
        public ulong TitleID { get; set; }

        /// <summary>
        /// Title Type
        /// </summary>
        public uint TitleType { get; set; }

        /// <summary>
        /// Group ID
        /// </summary>
        public ushort GroupID { get; set; }

        /// <summary>
        /// Save Data Size in Little Endian (Bytes) (Also SRL Public Save Data Size)
        /// </summary>
        public uint SaveDataSize { get; set; }

        /// <summary>
        /// SRL Private Save Data Size in Little Endian (Bytes)
        /// </summary>
        public uint SRLPrivateSaveDataSize { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[]? Reserved2 { get; set; }

        /// <summary>
        /// SRL Flag
        /// </summary>
        public byte SRLFlag { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[]? Reserved3 { get; set; }

        /// <summary>
        /// Access Rights
        /// </summary>
        public uint AccessRights { get; set; }

        /// <summary>
        /// Title Version
        /// </summary>
        public ushort TitleVersion { get; set; }

        /// <summary>
        /// Content Count (big-endian)
        /// </summary>
        public ushort ContentCount { get; set; }

        /// <summary>
        /// Boot Content
        /// </summary>
        public ushort BootContent { get; set; }

        /// <summary>
        /// Padding
        /// </summary>
        public byte[]? Padding2 { get; set; }

        /// <summary>
        /// SHA-256 Hash of the Content Info Records
        /// </summary>
        public byte[]? SHA256HashContentInfoRecords { get; set; }

        /// <summary>
        /// There are 64 of these records, usually only the first is used.
        /// </summary>
        public ContentInfoRecord[]? ContentInfoRecords { get; set; }

        /// <summary>
        /// There is one of these for each content contained in this title.
        /// (Determined by "Content Count" in the TMD Header).
        /// </summary>
        public ContentChunkRecord[]? ContentChunkRecords { get; set; }

        /// <summary>
        /// Certificate chain
        /// </summary>
        /// <remarks>
        /// https://www.3dbrew.org/wiki/Title_metadata#Certificate_Chain
        /// </remarks>
        public Certificate[]? CertificateChain { get; set; }
    }
}