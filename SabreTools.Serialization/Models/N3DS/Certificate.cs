namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// Certificates contain cryptography information for verifying Signatures.
    /// These certificates are also signed. The parent/child relationship between
    /// certificates, makes all the certificates effectively signed by 'Root',
    /// the public key for which is stored in NATIVE_FIRM.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/Certificates"/>
    public sealed class Certificate
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
        public byte[] Signature { get; set; }

        /// <summary>
        /// Padding to align next data to 0x40 bytes
        /// </summary>
        public byte[] Padding { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Key Type
        /// </summary>
        public PublicKeyType KeyType { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Expiration time as UNIX Timestamp, used at least for CTCert
        /// </summary>
        public uint ExpirationTime { get; set; }

        // This contains the Public Key (i.e. Modulus & Public Exponent)
        #region RSA-4096 and RSA-2048

        /// <summary>
        /// Modulus
        /// </summary>
        public byte[] RSAModulus { get; set; }

        /// <summary>
        /// Public Exponent
        /// </summary>
        public uint RSAPublicExponent { get; set; }

        /// <summary>
        /// Padding
        /// </summary>
        public byte[] RSAPadding { get; set; }

        #endregion

        // This contains the ECC public key, and is as follows:
        #region ECC

        /// <summary>
        /// Public Key
        /// </summary>
        public byte[] ECCPublicKey { get; set; }

        /// <summary>
        /// Padding
        /// </summary>
        public byte[] ECCPadding { get; set; }

        #endregion
    }
}
