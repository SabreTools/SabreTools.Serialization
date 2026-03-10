namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD2/3 encrypted challenge response
    /// </summary>
    /// <remarks>Encrypted form of <see cref="DecryptedChallengeResponseXGD23"/></remarks>
    public class EncryptedChallengeResponseXGD23
    {
        /// <summary>
        /// Encrypted response data
        /// </summary>
        /// <remarks>12 bytes</remarks>
        public byte[] EncryptedData { get; set; } = new byte[12];
    }
}
