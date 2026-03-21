namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// AS/400 Extra Field (0x0065) Attribute [APPENDIX A]
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class AS400ExtraFieldAttribute : ExtensibleDataField
    {
        /// <summary>
        /// Field length including length
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ushort FieldLength { get; set; }

        /// <summary>
        /// Field code
        /// </summary>
        public AS400ExtraFieldAttributeFieldCode FieldCode { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        /// <remarks>Variable byte length based on field code</remarks>
        public byte[] Data { get; set; } = [];
    }
}
