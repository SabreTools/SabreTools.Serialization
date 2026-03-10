namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD1 encrypted challenge response
    /// </summary>
    /// <remarks>Decrypted form is undocumented</remarks>
    public class EncryptedChallengeResponseXGD1
    {
        /// <summary>
        /// Encrypted response data
        /// </summary>
        /// <remarks>11 bytes</remarks>
        public byte[] EncryptedData { get; set; } = new byte[11];
    }
}
