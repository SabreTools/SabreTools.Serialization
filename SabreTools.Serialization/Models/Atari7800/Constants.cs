namespace SabreTools.Data.Models.Atari7800
{
    public static class Constants
    {
        /// <summary>
        /// A78 magic string (null-padded)
        /// </summary>
        public static readonly byte[] MagicBytesWithNull =
        [
            0x41, 0x54, 0x41, 0x52, 0x49, 0x37, 0x38, 0x30,
            0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        ];

        /// <summary>
        /// A78 magic string (null-padded)
        /// </summary>
        public const string MagicStringWithNull = "ATARI7800\0\0\0\0\0\0\0";

        /// <summary>
        /// A78 magic string (space-padded)
        /// </summary>
        public static readonly byte[] MagicBytesWithSpace =
        [
            0x41, 0x54, 0x41, 0x52, 0x49, 0x37, 0x38, 0x30,
            0x30, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
        ];

        /// <summary>
        /// A78 magic string (space-padded)
        /// </summary>
        public const string MagicStringWithSpace = "ATARI7800       ";

        /// <summary>
        /// A78 header end magic text
        /// </summary>
        public static readonly byte[] HeaderEndMagicBytes =
        [
            0x41, 0x43, 0x54, 0x55, 0x41, 0x4C, 0x20, 0x43,
            0x41, 0x52, 0x54, 0x20, 0x44, 0x41, 0x54, 0x41,
            0x20, 0x53, 0x54, 0x41, 0x52, 0x54, 0x53, 0x20,
            0x48, 0x45, 0x52, 0x45,
        ];

        /// <summary>
        /// A78 header end magic text
        /// </summary>
        public const string HeaderEndMagicString = "ACTUAL CART DATA STARTS HERE";
    }
}
