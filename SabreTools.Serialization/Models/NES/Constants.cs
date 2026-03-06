namespace SabreTools.Data.Models.NES
{
    public static class Constants
    {
        /// <summary>
        /// NES<EOF>
        /// </summary>
        public static readonly byte[] SignatureBytes = [0x4E, 0x45, 0x53, 0x1A];

        /// <summary>
        /// NES<EOF>
        /// </summary>
        public static readonly string SignatureString = "NES" + (char)0x1A;
    }
}
