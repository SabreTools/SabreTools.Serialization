using SabreTools.Data.Attributes;

namespace SabreTools.Serialization.Models.ClrMamePro
{
    /// <remarks>rom</remarks>
    public class Rom
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>size, Numeric</remarks>
        [Required]
        public string? Size { get; set; }

        /// <remarks>crc</remarks>
        public string? CRC { get; set; }

        /// <remarks>md5</remarks>
        public string? MD5 { get; set; }

        /// <remarks>sha1</remarks>
        public string? SHA1 { get; set; }

        /// <remarks>merge</remarks>
        public string? Merge { get; set; }

        /// <remarks>status</remarks>
        public string? Status { get; set; }

        /// <remarks>flags</remarks>
        public string? Flags { get; set; }

        /// <remarks>date</remarks>
        public string? Date { get; set; }

        #region Hash Extensions

        /// <remarks>md2; Appears after CRC</remarks>
        public string? MD2 { get; set; }

        /// <remarks>md4; Appears after MD2</remarks>
        public string? MD4 { get; set; }

        /// <remarks>ripemd128; Appears after MD5</remarks>
        public string? RIPEMD128 { get; set; }

        /// <remarks>ripemd160; Appears after RIPEMD128</remarks>
        public string? RIPEMD160 { get; set; }

        /// <remarks>sha256; Also in No-Intro spec; Appears after SHA1</remarks>
        public string? SHA256 { get; set; }

        /// <remarks>sha384; Appears after SHA256</remarks>
        public string? SHA384 { get; set; }

        /// <remarks>sha512; Appears after SHA384</remarks>
        public string? SHA512 { get; set; }

        /// <remarks>spamsum; Appears after SHA512</remarks>
        public string? SpamSum { get; set; }

        #endregion

        #region DiscImgeCreator Extensions

        /// <remarks>xxh3_64; Appears after SpamSum</remarks>
        public string? xxHash364 { get; set; }

        /// <remarks>xxh3_128; Appears after xxHash364</remarks>
        public string? xxHash3128 { get; set; }

        #endregion

        #region MAME Extensions

        /// <remarks>region; Appears after Status</remarks>
        public string? Region { get; set; }

        /// <remarks>offs; Appears after Flags</remarks>
        public string? Offs { get; set; }

        #endregion

        #region No-Intro Extensions

        /// <remarks>serial; Appears after Offs</remarks>
        public string? Serial { get; set; }

        /// <remarks>header; Appears after Serial</remarks>
        public string? Header { get; set; }

        #endregion

        #region RomVault Extensions

        /// <remarks>inverted; Boolean; Appears after Date</remarks>
        public string? Inverted { get; set; }

        /// <remarks>mia; Boolean; Appears after Inverted</remarks>
        public string? MIA { get; set; }

        #endregion
    }
}