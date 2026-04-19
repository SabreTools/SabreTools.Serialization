namespace SabreTools.Data.Models.XRD
{
    public static class Constants
    {
        /// <summary>
        /// Magic string at start of XRD file
        /// </summary>
        public static readonly byte[] MagicBytes = [0x58, 0x52, 0x44, 0xFF, 0x00];
    }
}
