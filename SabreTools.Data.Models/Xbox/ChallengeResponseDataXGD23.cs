namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD2/3 challenge response data
    /// </summary>
    public class ChallengeResponseDataXGD23
    {
        /// <summary>
        /// Challenge data
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] ChallengeData { get; set; } = new byte[4]; // 0-3

        /// <summary>
        /// Angle
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public short Angle { get; set; }

        /// <summary>
        /// Unknown response data
        /// </summary>
        public byte Unknown06 { get; set; }

        /// <summary>
        /// Angle2
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public short Angle2 { get; set; }
    }
}
