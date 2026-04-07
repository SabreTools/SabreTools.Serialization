using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public static class Constants
    {
        /// <summary>
        /// STFS LIVE magic number ("LIVE")
        /// </summary>
        public static readonly byte[] MagicBytesLIVE = [0x4C, 0x49, 0x56, 0x45];

        /// <summary>
        /// STFS LIVE magic string ("LIVE")
        /// </summary>
        public const string MagicStringLIVE = "LIVE";

        /// <summary>
        /// STFS PIRS magic number ("PIRS")
        /// </summary>
        public static readonly byte[] MagicBytesPIRS = [0x50, 0x49, 0x52, 0x53];

        /// <summary>
        /// STFS PIRS magic string ("PIRS")
        /// </summary>
        public const string MagicStringPIRS = "PIRS";

        /// <summary>
        /// STFS CON magic number ("CON ")
        /// </summary>
        public static readonly byte[] MagicBytesCON = [0x43, 0x4F, 0x4E, 0x20];

        /// <summary>
        /// STFS CON magic string ("CON")
        /// </summary>
        public const string MagicStringCON = "CON ";

        /// <summary>
        /// Standard length of an STFS header
        /// </summary>
        public const uint StandardHeaderSize = 0xB000;
    }
}
