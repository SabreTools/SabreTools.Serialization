namespace SabreTools.Data.Models.NES
{
    public static class Constants
    {
        /// <summary>
        /// NES<EOF>
        /// </summary>
        public static readonly byte[] CartSignatureBytes = [0x4E, 0x45, 0x53, 0x1A];

        /// <summary>
        /// NES<EOF>
        /// </summary>
        public static readonly string CartSignatureString = "NES" + (char)0x1A;
    }
}
