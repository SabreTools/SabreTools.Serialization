namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// Digital signature
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class DigitalSignature
    {
        /// <summary>
        /// Header signature (0x05054B50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Data length
        /// </summary>
        public ushort DataLength { get; set; }

        /// <summary>
        /// Signature data (variable size)
        /// </summary>
        public byte[]? SignatureData { get; set; }
    }
}
