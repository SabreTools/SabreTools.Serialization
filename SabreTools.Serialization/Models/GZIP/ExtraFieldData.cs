namespace SabreTools.Data.Models.GZIP
{
    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    public sealed class ExtraFieldData
    {
        /// <summary>
        /// SI1 and SI2 provide a subfield ID, typically two ASCII letters
        /// with some mnemonic value.
        /// </summary>
        public byte SubfieldID1 { get; set; }

        /// <summary>
        /// SI1 and SI2 provide a subfield ID, typically two ASCII letters
        /// with some mnemonic value.
        /// </summary>
        public byte SubfieldID2 { get; set; }

        /// <summary>
        /// LEN gives the length of the subfield data, excluding the 4
        /// initial bytes.
        /// </summary>
        public ushort Length { get; set; }

        public byte[]? Data { get; set; }
    }
}