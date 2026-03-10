namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD1/2/3 drive entry data
    /// </summary>
    public class DriveEntryData
    {
        /// <summary>
        /// Response type
        /// </summary>
        /// TODO: Is there an enum for this?
        public byte ResponseType { get; set; }

        /// <summary>
        /// Challenge ID
        /// </summary>
        public byte ChallengeID { get; set; }

        /// <summary>
        /// Mod
        /// </summary>
        public byte Mod { get; set; }

        /// <summary>
        /// Drive response data
        /// </summary>
        /// <remarks>6 bytes; Includes SS ranges</remarks>
        public byte[] Data { get; set; } = new byte[6];
    }
}
