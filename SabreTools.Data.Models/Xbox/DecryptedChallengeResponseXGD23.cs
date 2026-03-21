namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD2/3 decrypted challenge response table entry
    /// </summary>
    /// <remarks>Decrypted form of <see cref="EncryptedChallengeResponseXGD23"/></remarks>
    public class DecryptedChallengeResponseXGD23
    {
        /// <summary>
        /// Challenge Type
        /// </summary>
        public byte ChallengeType { get; set; }

        /// <summary>
        /// Challenge ID
        /// </summary>
        public byte ChallengeID { get; set; }

        /// <summary>
        /// Tolerance
        /// </summary>
        public byte Tolerance { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        /// TODO: Is there an enum for this?
        public byte Type { get; set; }

        /// <summary>
        /// Challenge data
        /// </summary>
        public byte[] ChallengeData { get; set; } = new byte[4];

        /// <summary>
        /// Unknown response data
        /// </summary>
        /// <remarks>2 bytes</remarks>
        public byte[] Unknown08 { get; set; } = new byte[2];

        /// <summary>
        /// Angle
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public short Angle { get; set; }
    }
}
