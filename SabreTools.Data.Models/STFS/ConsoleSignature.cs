namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// Signature signed by console, for "CON " format STFS files
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class ConsoleSignature : Signature
    {
        /// <summary>
        /// Public Key Certificate Size
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ushort CertificateSize { get; set; }

        /// <summary>
        /// Certificate Owner Console ID
        /// </summary>
        /// <remarks>5 bytes</remarks>
        public byte[] ConsoleID { get; set; } = new byte[5];

        /// <summary>
        /// Certificate Owner Console Part Number
        /// </summary>
        /// <remarks>20 bytes, ASCII string</remarks>
        public byte[] PartNumber { get; set; } = new byte[20];

        /// <summary>
        /// Certificate Owner Console Type (1 for devkit, 2 for retail)
        /// </summary>
        public byte ConsoleType { get; set; }

        /// <summary>
        /// Certificate Date of Generation
        /// </summary>
        /// <remarks>8 bytes, ASCII string</remarks>
        public byte[] CertificateDate { get; set; } = new byte[8];

        /// <summary>
        /// Public Exponent
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] PublicExponent { get; set; } = new byte[4];

        /// <summary>
        /// Public Modulus
        /// </summary>
        /// <remarks>128 bytes</remarks>
        public byte[] PublicModulus { get; set; } = new byte[128];

        /// <summary>
        /// Certificate Signature
        /// </summary>
        /// <remarks>256 bytes</remarks>
        public byte[] CertificateSignature { get; set; } = new byte[256];

        /// <summary>
        /// Signature
        /// </summary>
        /// <remarks>128 bytes</remarks>
        public byte[] Signature { get; set; } = new byte[128];
    }
}
